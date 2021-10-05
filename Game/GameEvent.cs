using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class GameEvent : MonoBehaviour
{
    Player player;
    Button RollDiceButton;
    public GameObject BonusCheck;

    Text bounsCheckText;

    private void Start()
    {
        player = GameManager.GM.myPlayer;
    }


    public void ReturnButtonOnClick()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void LogButtonOnClick()
    {

    }

    public void ChatButtonOnClick()
    {


    }

    public void MyDeckButtonOnClick()
    {

    }

    public void ConstructingCheckButtonOnClick()
    {
        player.FinishConstructing();
    }

    public void RollDiceButtonOnClick()
    {
        player.pv.RPC("RPC_MasterRollDice", RpcTarget.MasterClient);
        player.RollButton.SetActive(false);
    }

    public void FinishActionButtonOnClick()
    {
        player.FinishAction();
    }

    string intendBonus;
    public void DrawBonusButtonOnClick()
    {
        player.DrawCheck.SetActive(false);
        player.bonusButtons[3].interactable = false;
        player.tempBlockAction = true;
        BonusCheck.SetActive(true);
        bounsCheckText = BonusCheck.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        bounsCheckText.text = "抽一張牌改為抽兩張並選擇一張保留。";
        intendBonus = "Draw";
    }

    public void GaugeBonusButtonOnClick()
    {
        player.tempBlockAction = true;
        BonusCheck.SetActive(true);
        bounsCheckText = BonusCheck.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        bounsCheckText.text = "召喚等級提升1級(不得超過15級)。";
        intendBonus = "Gauge";
    }

    public void TransmuteBonusButtonOnClick()
    {
        player.tempBlockAction = true;
        BonusCheck.SetActive(true);
        bounsCheckText = BonusCheck.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        bounsCheckText.text = "本回合你可以轉化。";
        intendBonus = "Transmute";
    }

    public void TradeBonusButtonOnClick()
    {
        player.tempBlockAction = true;
        bounsCheckText.text = "選擇2個持有元素交換為任意元素。";
        BonusCheck.SetActive(true);
        intendBonus = "Trade";
    }

    public void BonusCheckButtonOnClick()
    {
        player.tempBlockAction = false;
        BonusCheck.SetActive(false);
        switch (intendBonus)
        {
            case "Draw":
                player.BonusDrawTwoCard();
                break;
                
        }
    }
    public void BonusCancelButtonOnClick()
    {
        player.tempBlockAction = false;
        BonusCheck.SetActive(false);
    }

    public void DrawCheckOnClick()
    {
        player.bonusButtons[3].interactable = false;
        player.DrawCard();
        player.DrawCheck.SetActive(false);
    }
}
