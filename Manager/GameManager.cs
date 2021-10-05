using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public Text ProgressText;

    public Player myPlayer;
    private static CardManager cm;
    private static GameManager gm;
    private static DiceManager dm;

    [HideInInspector]
    public bool inFirstDrawing, inConstructingDeck, startRound, inRollPhase, hasRolled, inActionPhase;
    public int firstDrawRound;
    public int roundCount, roundStartFrom, whoseTurn;
    public int year, month;
    public enum Season { Winter, Spring, Summer, Fall};
    public Season season;
    public SeasonWheel seasonWheel;

    int playerCountInRoom;

    #region Delegate
    public delegate void DoBeforePerformDice();
    public DoBeforePerformDice doBeforePerformDice;
    public delegate void PerformDice();
    public PerformDice performDice;
    #endregion

    public static GameManager GM
    {
        get
        {
            if (gm == null)
            {
                gm = FindObjectOfType(typeof(GameManager)) as GameManager;
                if (gm == null)
                {
                    GameObject go = new GameObject("gm");
                    gm = go.AddComponent<GameManager>();
                }
            }
            return gm;
        }
    }
    private void Awake()
    {
        cm = CardManager.CM;
        dm = DiceManager.DM;
        myPlayer = FindObjectOfType(typeof(Player)) as Player;
        startRound = false;
        playerCountInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void Start()
    {
        dm.InitSeasonDice();
    }

    private void Update()
    {
        if (startRound)
        {
            if (inRollPhase)
            {
                RollPhase();
            }
            else if (inActionPhase)
            {
                ActionPhase();
            }
        }
    }

    public void ChangeTurn()
    {
        if (whoseTurn < playerCountInRoom-1)
        {
            whoseTurn++;
        }
        else
        {
            whoseTurn = 0; 
        }
        if (whoseTurn == roundStartFrom)
        {
            SwitchPhase();
        }
    }

    public void SwitchPhase()
    {
        if (inRollPhase)
        {
            inRollPhase = false;
            inActionPhase = true;
            dm.SetLeftDicePos();
        }
        else if (inActionPhase)
        {
            inActionPhase = false;
            inRollPhase = true;
            ChangeRound();
        }
    }

    void ChangeRound()
    {
        myPlayer.myInfo.hasChosenDice = false;
        if (roundStartFrom < playerCountInRoom - 1)
        {
            roundStartFrom++;
        }
        else
        {
            roundStartFrom = 0;
        }
        whoseTurn = roundStartFrom;
        roundCount++;
        seasonWheel.ChangMonth(ref month ,dm.UseLeftDiceAndMove());
        // 待修改: 遊戲歷程Print round count出來
    }


    void RollPhase()
    {
        if (whoseTurn == myPlayer.myInfo.playerNo)
        {
            if (!hasRolled)
            {
                if (roundStartFrom == myPlayer.myInfo.playerNo)
                {
                    myPlayer.RollButton.SetActive(true);
                }
            }
            ChangeProgressText(true, "RollTurn");
        }
        else
        {
            ChangeProgressText(false, "RollTurn");
        }
    }
    private bool hasDoBeforePerformDice;

    void ActionPhase()
    {
        if (whoseTurn == myPlayer.myInfo.playerNo)
        {
            myPlayer.FinishActionButton.SetActive(true);
            myPlayer.myAction = true;
            ChangeProgressText(true, "Action");
            //doBeforePerformDice();
            myPlayer.CallDiceFunction(myPlayer.myInfo.chosenDice);
            //測試用
            performDice += PerformDiceLog;
            performDice();
        }
        else
        {
            ChangeProgressText(false, "Action");
        }
    }

    void PerformDiceLog()
    {
        Debug.Log("PerformDiceLog");
    }

    public bool IsChangeSeason ()
    {
        return false;
    }


    public void ChangeProgressText(bool myTurn, string s)
    {
        string showText = "";

        if (myTurn)
        {
            showText += "輪到我了！";
        }
        else
        {
            foreach (PlayerInfo info in myPlayer.allInfos)
            {
                if (whoseTurn == info.playerNo)
                {
                    showText += $"{info.playerName}正在";
                }
            }
        }
        switch(s)
        {
            case "FirstDraw":
                showText = "選擇一張魔法牌保留。";
                break;
            case "Constructing":
                showText = "分別配置 3 張魔法牌到第一、二、三年藏牌。";
                break;
            case "RollTurn":
                showText += "選擇一顆季節骰。";
                break;
            case "Action":
                showText += "選擇執行動作。";
                break;

        }

        ProgressText.text = showText;
    }
}
