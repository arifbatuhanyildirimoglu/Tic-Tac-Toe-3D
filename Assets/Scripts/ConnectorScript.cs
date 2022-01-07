using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ConnectorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
    }
}