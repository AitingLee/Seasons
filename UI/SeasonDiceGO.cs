using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SeasonDiceGO : MonoBehaviour
{
    public Rigidbody diceRB;
    public bool hasValue = false;   //If dice has stopped and save value
    public int rollingNo;
    public PhotonView dicePV;
    public SeasonDice thisDice;

    private float timer;
    private int clickCount = 0;
    private Player player;
    private static DiceManager dm;
    

    private void Awake()
    {
        dicePV = this.GetComponent<PhotonView>();
        diceRB = this.GetComponent<Rigidbody>();
        player = GameManager.GM.myPlayer;
        thisDice = this.GetComponent<SeasonDice>();
        dm = DiceManager.DM;
        //object name = SeasonDiceXXXXX(Clone)
        string seasonAndNo = this.name.Remove(0, 10);   //object name = XXXXX(Clone)
        seasonAndNo = seasonAndNo.Remove(seasonAndNo.Length - 7, 7);        //object name = XXXXX
        string sSeason = seasonAndNo.Remove(seasonAndNo.Length - 1, 1);
        string sNo = seasonAndNo.Substring(seasonAndNo.Length - 1, 1);
        switch (sSeason)
        {
            case "Winter":
                thisDice.diceSeason = GameManager.Season.Winter;
                break;
            case "Spring":
                thisDice.diceSeason = GameManager.Season.Spring;
                break;
            case "Summer":
                thisDice.diceSeason = GameManager.Season.Summer;
                break;
            case "Fall":
                thisDice.diceSeason = GameManager.Season.Fall;
                break;
            default:
                Debug.LogError("Dice Season not found");
                break;
        }
        int no = 0;
        int.TryParse(sNo, out no);
        thisDice.diceNo = no;
        if (thisDice.diceNo == 0)
        {
            Debug.LogError("Dice No not found");
        }
    }

    private void Update()
    {
        if (diceRB.IsSleeping() && hasValue == false)
        {
            if (dicePV.IsMine)
            {
                dicePV.RPC("RPC_SetValue", RpcTarget.All, rollingNo, GetDiceSide());
            }
        }
    }

    public int GetDiceSide()
    {
        var minAngle = Vector3.Angle(transform.forward, Vector3.up);
        int side = 1;
        Vector3[] testVectors = new Vector3[5] { -transform.up, -transform.right, transform.right , transform.up , -transform.forward };
        float testAngle;
        for (int i = 0; i < 5; i++)
        {
            testAngle = Vector3.Angle(testVectors[i], Vector3.up);
            if (testAngle < minAngle)
            {
                minAngle = testAngle;
                side = i + 2;
            }
        }
        return side;
    }

    public void OnMouseDown()
    {
        if (hasValue)
        {
            if (clickCount == 1)
            {
                if (Time.time - timer <= 0.5f)
                {
                    if (GameManager.GM.whoseTurn == player.myInfo.playerNo)
                    {
                        player.ChooseDice(thisDice);
                        //計時器及計次歸零
                        timer = 0;
                        clickCount = 0;
                        dicePV.RPC("RPC_DestroyThisDice", RpcTarget.MasterClient);
                    }
                }
                else
                {
                    //視為第一次點擊
                    timer = Time.time;
                    clickCount = 1;
                }
            }
            else if (clickCount == 0)
            {
                timer = Time.time;
                clickCount = 1;
            }
        }
    }
    // 要master call pv.ismine 來刪除選取的骰子 
    [PunRPC]
    void RPC_SetRollingNo(int toSetNo)
    {
        this.rollingNo = toSetNo;
        dm.rollingDices[toSetNo] = this.gameObject;
    }

    [PunRPC]
    void RPC_SetValue(int rollingNo, int value)
    {
        var diceGO = dm.rollingDices[rollingNo].GetComponent<SeasonDiceGO>();
        diceGO.hasValue = true;
        diceGO.thisDice.sideValue = value;
    }

    [PunRPC]
    void RPC_DestroyThisDice()
    {
        if (dicePV.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            Debug.LogError("dice pv owner is not master client");
        }
    }
}
