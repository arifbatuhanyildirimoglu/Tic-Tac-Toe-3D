using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
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

    public void OnRestartButtonClicked()
    {
        gameObject.GetComponent<PhotonView>().RPC("OnRestartButtonClickedRPC",RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void OnRestartButtonClickedRPC()
    {
        GameManager.Instance.ReloadScene();
        GameManager.Instance._clientgameObject.GetComponent<PlayerScript>().score = 0;
        GameManager.Instance._mastergameObject.GetComponent<PlayerScript>().score = 0;
        ScoreManager.Instance.clientScore = 0;
        ScoreManager.Instance.masterScore = 0;
        GameManager.Instance.gameOverPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
    
}