using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
//using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerWait : MonoBehaviourPunCallbacks
{
    public Text Player1NameText;
    public Text Player2NameText;
    public Text Player3NameText;
    public Text Player4NameText;
    public Text Player1StateText;
    public Text Player2StateText;
    public Text Player3StateText;
    public Text Player4StateText;
    public Button StartGameButton;
    public PhotonView pv;

    [HideInInspector]
    private Text[] PlayerNameTexts;
    private string thisName;



    public void Awake()
    {
        PlayerNameTexts = new Text[4] { Player1NameText, Player2NameText, Player3NameText, Player4NameText };
        thisName = PhotonNetwork.LocalPlayer.NickName;
        pv = PhotonView.Get(this);
    }

    public void Start()
    {

        if (PhotonNetwork.CurrentRoom.Players.Count > 1)
        {
            pv.RPC("RPC_EveryoneUpdateNameList", RpcTarget.All);
        }
        else
        {
            Player1NameText.text = thisName;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.gameObject.SetActive(true);
        }

    }

    public void OnReadyButtonClick()
    {
        pv.RPC("RPC_ClickReady", RpcTarget.All, thisName);
    }

    public void OnStartButtonClick()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            StartCoroutine(WaitAllReady());
        }
    }

    private IEnumerator WaitAllReady()
    {
        yield return new WaitUntil(() => AllPlayersReady);

        PhotonNetwork.LoadLevel("Game");
    }

    private bool AllPlayersReady
    {
        get
        {
            //if (Player1StateText.gameObject.activeSelf == false 
            //    || Player2StateText.gameObject.activeSelf == false
            //    || Player3StateText.gameObject.activeSelf == false
            //    || Player4StateText.gameObject.activeSelf == false)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
            return true; //待修改 僅測試用
        }
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        pv.RPC("RPC_EveryoneUpdateNameList", RpcTarget.All);
    }


    [PunRPC]
    void RPC_EveryoneUpdateNameList()
    {
        Player1NameText.text = "等候玩家加入";
        Player2NameText.text = "等候玩家加入";
        Player3NameText.text = "等候玩家加入";
        Player4NameText.text = "等候玩家加入";
        int count = 1;
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            switch (count)
            {
                case 1:
                    Player1NameText.text = player.NickName;
                    break;
                case 2:
                    Player2NameText.text = player.NickName;
                    break;
                case 3:
                    Player3NameText.text = player.NickName;
                    break;
                case 4:
                    Player4NameText.text = player.NickName;
                    break;
            }
            count++;
        }
    }

    [PunRPC]
    void RPC_ClickReady(string name)
    {
        if (Player1NameText.text == name)
        {
            Player1StateText.gameObject.SetActive(!Player1StateText.gameObject.activeSelf);
        }
        else if (Player2NameText.text == name)
        {
            Player2StateText.gameObject.SetActive(!Player2StateText.gameObject.activeSelf);
        }
        else if (Player3NameText.text == name)
        {
            Player3StateText.gameObject.SetActive(!Player3StateText.gameObject.activeSelf);
        }
        else if (Player4NameText.text == name)
        {
            Player4StateText.gameObject.SetActive(!Player4StateText.gameObject.activeSelf);
        }
    }

}
