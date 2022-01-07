using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public void OnStartButtonClicked()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void OnCreateRoomButtonClicked()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;

        PhotonNetwork.CreateRoom("Room1", options);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void OnJoinRoomButtonClicked()
    {
        PhotonNetwork.JoinRoom("Room1");
    }
}