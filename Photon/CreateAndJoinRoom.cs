using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    private PhotonManager pm;
    public InputField createInput;
    public InputField joinInput;
    

    public void Start()
    {
        pm = GameObject.Find("pm").GetComponent<PhotonManager>();
        
    }

    public void CreateRoom()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = pm.maxPlayersPerRoom;
        PhotonNetwork.CreateRoom(createInput.text, options);
    }

    public void JoinRoom()
    {   
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("WaitRoom");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
