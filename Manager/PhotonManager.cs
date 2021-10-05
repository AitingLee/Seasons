using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager pm;
    public InputField nameInput;
    public byte maxPlayersPerRoom;
    

    void Awake()
    {
        if (pm != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        pm = this;

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        maxPlayersPerRoom = 4;
    }

    public void OnStartButtonClick()
    {
        if (nameInput.text != null)
        {
            PhotonNetwork.LocalPlayer.NickName = nameInput.text;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Name input is null");
        }
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN 呼叫 OnDisconnected() {0}.", cause);
    }


}