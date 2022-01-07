using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class XScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Master"))
        {
            other.GetComponent<PlayerScript>().hasStone = true;
            other.GetComponent<PlayerScript>().carryingStone = gameObject;
            other.GetComponent<PlayerScript>().carryingStoneType = "X";
            //transform.parent = other.gameObject.transform;
            SetActiveFalse();
        }
    }

    private void SetActiveFalse()
    {
        gameObject.GetComponent<PhotonView>().RPC("RPCSetActiveFalse", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPCSetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void RPCInsertStone(int stoneViewId, int slotViewId)
    {
        PhotonView stonePhotonView = PhotonView.Find(stoneViewId);
        PhotonView slotPhotonView = PhotonView.Find(slotViewId);

        stonePhotonView.gameObject.transform.position = new Vector3(slotPhotonView.gameObject.transform.position.x, 5f,
            slotPhotonView.gameObject.transform.position.z);
        stonePhotonView.gameObject.SetActive(true);
        Destroy(GetComponent<SphereCollider>());
        Destroy(this);
    }
}