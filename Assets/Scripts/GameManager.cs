using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    public GameObject waitingPanel;
    public Transform spawnPosTop;
    public Transform spawnPosBottom;

    private GameObject _mastergameObject;
    private GameObject _clientgameObject;

    public List<GameObject> slots;
    public bool isXExist;
    public bool isOExist;
    public bool isXSpawnable;
    public bool isOSpawnable;

    /*
     * 1   2   3
     * 4   5   6
     * 7   8   9
     *
     * Possible matches:
     * 1-2-3
     * 1-4-7
     * 1-5-9
     * 2-5-8
     * 3-5-7
     * 3-6-9
     * 4-5-6
     * 7-8-9
     */

    //üst alan z-> 2.8 - 4.8, x-> -4.8 - 4.8
    //alt alan z-> -2.8 - -4.8, x-> -4.8 - 4.8


    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            _clientgameObject = PhotonNetwork.Instantiate("Player",
                new Vector3(spawnPosTop.position.x, spawnPosTop.position.y, spawnPosTop.position.z),
                Quaternion.identity);
            _clientgameObject.tag = "Client";
        }

        isXExist = true;
        isOExist = true;
        isXSpawnable = false;
        isOSpawnable = false;
    }

    int counter = 0;
    private int counter2 = 0;

    public bool isBeginning = true;


    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && counter2 == 0)
            {
                waitingPanel.SetActive(true);
                _mastergameObject = PhotonNetwork.Instantiate("Player",
                    new Vector3(spawnPosBottom.position.x, spawnPosBottom.position.y, spawnPosBottom.position.z),
                    Quaternion.identity);
                _mastergameObject.tag = "Master";
                counter2++;
            }
            else
            {
                if (counter == 0)
                {
                    waitingPanel.SetActive(false);
                    counter++;
                }
            }
        }

        if (PhotonNetwork.IsMasterClient && isBeginning && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            float randomXAxisForX = Random.Range(-4.8f, 4.8f);
            float randomZAxisForX = Random.Range(-4.8f, -2.8f);
            float randomXAxisForO = Random.Range(-4.8f, 4.8f);
            float randomZAxisForO = Random.Range(2.8f, 4.8f);

            Vector3 randomSpawnPosX = new Vector3(randomXAxisForX, 5, randomZAxisForX);
            Vector3 randomSpawnPosO = new Vector3(randomXAxisForO, 5, randomZAxisForO);

            PhotonNetwork.Instantiate("X", randomSpawnPosX, Quaternion.identity);
            PhotonNetwork.Instantiate("O", randomSpawnPosO, Quaternion.identity);

            isBeginning = false;
        }

        Debug.Log("X: " + GameObject.FindGameObjectsWithTag("X").Length);
        Debug.Log("O: " + GameObject.FindGameObjectsWithTag("O").Length);

        if (GameObject.FindGameObjectsWithTag("X").Length >= 2)
        {
            isXSpawnable = false;
        }
        else
        {
            isXSpawnable = true;
        }

        if (GameObject.FindGameObjectsWithTag("O").Length >= 2)
        {
            isOSpawnable = false;
        }
        else
        {
            isOSpawnable = true;
        }


        for (int i = 0; i <GameObject.FindGameObjectsWithTag("X").Length ; i++)
        {
            if (GameObject.FindGameObjectsWithTag("X")[i].GetComponent<XScript>() != null)
            {
                isXExist = true;
                return;
            }
            else
            {
                isXExist = false;
            }
           
        }

        for (int i = 0; i <GameObject.FindGameObjectsWithTag("O").Length ; i++)
        {
            if (GameObject.FindGameObjectsWithTag("O")[i].GetComponent<OScript>() != null)
            {
                isOExist = true;
                return;
            }
            else
            {
                isOExist = false;
            }
           
        }
    }

    public void InsertStone(int stoneViewId, int slotViewId)
    {
        PhotonView stonePhotonView = PhotonView.Find(stoneViewId);
        PhotonView slotPhotonView = PhotonView.Find(slotViewId);

        stonePhotonView.RPC("RPCInsertStone", RpcTarget.AllBuffered, stonePhotonView.ViewID, slotPhotonView.ViewID);
        //slot.gameObject.GetComponent<PhotonView>().RPC("RPCInsertStone",RpcTarget.AllBuffered);
    }

    public void SpawnX()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            float randomXAxisForX = Random.Range(-4.8f, 4.8f);
            float randomZAxisForX = Random.Range(-4.8f, -2.8f);

            Vector3 randomSpawnPosX = new Vector3(randomXAxisForX, 5, randomZAxisForX);

            PhotonNetwork.Instantiate("X", randomSpawnPosX, Quaternion.identity);
        }
    }

    public void SpawnO()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
            float randomXAxisForO = Random.Range(-4.8f, 4.8f);
            float randomZAxisForO = Random.Range(2.8f, 4.8f);

            Vector3 randomSpawnPosO = new Vector3(randomXAxisForO, 5, randomZAxisForO);

            PhotonNetwork.Instantiate("O", randomSpawnPosO, Quaternion.identity);
        //}
    }
}