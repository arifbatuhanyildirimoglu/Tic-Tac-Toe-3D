using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    public GameObject waitingPanel;
    public Transform spawnPosTop;
    public Transform spawnPosBottom;

    public GameObject _mastergameObject;
    public GameObject _clientgameObject;

    public List<GameObject> slots;
    public bool isXExist;
    public bool isOExist;
    public bool isXSpawnable;
    public bool isOSpawnable;

    public Player masterPlayer;
    public Player clientPlayer;

    public GameObject gameOverPanel;
    public GameObject gameOverPanelWonText;
    public GameObject gameOverPanelScoreText;

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
            SetClientGameObject(_clientgameObject.GetComponent<PhotonView>().ViewID);
        }

        isXExist = true;
        isOExist = true;
        isXSpawnable = false;
        isOSpawnable = false;
    }

    int counter = 0;
    private int counter2 = 0;

    public bool isBeginning = true;

    private float nextActionTime = 0f;
    private float period = 8f;

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                if (counter2 == 0)
                {
                    waitingPanel.SetActive(true);
                    _mastergameObject = PhotonNetwork.Instantiate("Player",
                        new Vector3(spawnPosBottom.position.x, spawnPosBottom.position.y, spawnPosBottom.position.z),
                        Quaternion.identity);
                    _mastergameObject.tag = "Master";
                    SetMasterGameObject(_mastergameObject.GetComponent<PhotonView>().ViewID);
                    counter2++;
                }
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

        if (GameObject.FindGameObjectsWithTag("X").Length > 0 || GameObject.FindGameObjectsWithTag("O").Length > 0)
        {
            isBeginning = false;
        }

        if (GameObject.FindGameObjectsWithTag("X").Length >= 5)
        {
            isXSpawnable = false;
        }
        else
        {
            isXSpawnable = true;
        }

        if (GameObject.FindGameObjectsWithTag("O").Length >= 5)
        {
            isOSpawnable = false;
        }
        else
        {
            isOSpawnable = true;
        }

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("X").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("X")[i].GetComponent<XScript>().enabled)
            {
                isXExist = true;
                break;
            }
            else
            {
                isXExist = false;
            }
        }

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("O").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("O")[i].GetComponent<OScript>().enabled)
            {
                isOExist = true;
                break;
            }
            else
            {
                isOExist = false;
            }
        }

        //TODO:KAZANAN BELİRLEME
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
        GameObject slot1 = slots[0];
        GameObject slot2 = slots[1];
        GameObject slot3 = slots[2];
        GameObject slot4 = slots[3];
        GameObject slot5 = slots[4];
        GameObject slot6 = slots[5];
        GameObject slot7 = slots[6];
        GameObject slot8 = slots[7];
        GameObject slot9 = slots[8];

        if (!slot1.gameObject.GetComponent<Slot>().isEmpty)
        {
            //1-2-3
            //1-4-7
            //1-5-9

            //1-2-3
            if (!slot2.gameObject.GetComponent<Slot>().isEmpty && slot1.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot2.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot3.gameObject.GetComponent<Slot>().isEmpty && slot2.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot3.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot3.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }

            //1-4-7
            if (!slot4.gameObject.GetComponent<Slot>().isEmpty && slot1.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot4.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot7.gameObject.GetComponent<Slot>().isEmpty && slot4.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot7.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot7.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }

            //1-5-9
            if (!slot5.gameObject.GetComponent<Slot>().isEmpty && slot1.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot5.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot9.gameObject.GetComponent<Slot>().isEmpty && slot5.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot9.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot9.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }
        }

        //2-5-8
        if (!slot2.gameObject.GetComponent<Slot>().isEmpty)
        {
            if (!slot5.gameObject.GetComponent<Slot>().isEmpty && slot5.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot2.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot8.gameObject.GetComponent<Slot>().isEmpty && slot8.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot5.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot8.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }
        }

        //FOR SLOT3
        if (!slot3.gameObject.GetComponent<Slot>().isEmpty)
        {
            //3-5-7
            if (!slot5.gameObject.GetComponent<Slot>().isEmpty && slot5.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot3.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot7.gameObject.GetComponent<Slot>().isEmpty && slot7.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot5.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot7.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }

            //3-6-9
            if (!slot6.gameObject.GetComponent<Slot>().isEmpty && slot6.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot3.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot9.gameObject.GetComponent<Slot>().isEmpty && slot9.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot6.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot9.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }
        }

        //4-5-6
        if (!slot4.gameObject.GetComponent<Slot>().isEmpty)
        {
            if (!slot5.gameObject.GetComponent<Slot>().isEmpty && slot5.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot4.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot6.gameObject.GetComponent<Slot>().isEmpty && slot6.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot5.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot6.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }
        }

        //7-8-9
        if (!slot7.gameObject.GetComponent<Slot>().isEmpty)
        {
            if (!slot8.gameObject.GetComponent<Slot>().isEmpty && slot8.gameObject.GetComponent<Slot>().stoneType
                .Equals(slot7.gameObject.GetComponent<Slot>().stoneType))
            {
                if (!slot9.gameObject.GetComponent<Slot>().isEmpty && slot9.gameObject.GetComponent<Slot>().stoneType
                    .Equals(slot8.gameObject.GetComponent<Slot>().stoneType))
                {
                    if (slot9.gameObject.GetComponent<Slot>().stoneType.Equals("X"))
                    {
                        RoundOver("Master");
                    }
                    else
                    {
                        RoundOver("Client");
                    }
                }
            }
        }

        //DRAW
        if (!slot1.gameObject.GetComponent<Slot>().isEmpty && !slot2.gameObject.GetComponent<Slot>().isEmpty &&
            !slot3.gameObject.GetComponent<Slot>().isEmpty && !slot4.gameObject.GetComponent<Slot>().isEmpty &&
            !slot5.gameObject.GetComponent<Slot>().isEmpty && !slot6.gameObject.GetComponent<Slot>().isEmpty &&
            !slot7.gameObject.GetComponent<Slot>().isEmpty && !slot8.gameObject.GetComponent<Slot>().isEmpty &&
            !slot9.gameObject.GetComponent<Slot>().isEmpty)
        {
            RoundOver("Draw");
        }

        if (Time.time > nextActionTime)
        {
            SpawnRandomPowerUp();
            
            nextActionTime += period;
        }
    }

    public void SetClientGameObject(int clientGameObjectViewID)
    {
        gameObject.GetComponent<PhotonView>()
            .RPC("SetClientGameObjectRPC", RpcTarget.AllBuffered, clientGameObjectViewID);
    }

    [PunRPC]
    public void SetClientGameObjectRPC(int clientGameObjectViewID)
    {
        PhotonView clientGameObjectPhotonView = PhotonView.Find(clientGameObjectViewID);

        _clientgameObject = clientGameObjectPhotonView.gameObject;
    }

    public void SetMasterGameObject(int masterGameObjectViewID)
    {
        gameObject.GetComponent<PhotonView>()
            .RPC("SetMasterGameObjectRPC", RpcTarget.AllBuffered, masterGameObjectViewID);
    }

    [PunRPC]
    public void SetMasterGameObjectRPC(int masterGameObjectViewID)
    {
        PhotonView masterGameObjectPV = PhotonView.Find(masterGameObjectViewID);

        _mastergameObject = masterGameObjectPV.gameObject;
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
        gameObject.GetComponent<PhotonView>().RPC("SpawnXRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void SpawnXRPC()
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
        //client çağırıyor
        //yine client çağıracak fakat instantiate'i sadece master yapacak
        //rpc ile bu methodu master'ın da çağırması sağlanır
        //ve metodun içinde eğer mastersa instantiate yap denir

        gameObject.GetComponent<PhotonView>().RPC("SpawnORPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SpawnORPC()
    {
        if (PhotonNetwork.IsMasterClient && !isOExist && isOSpawnable)
        {
            float randomXAxisForO = Random.Range(-4.8f, 4.8f);
            float randomZAxisForO = Random.Range(2.8f, 4.8f);

            Vector3 randomSpawnPosO = new Vector3(randomXAxisForO, 5, randomZAxisForO);

            PhotonNetwork.Instantiate("O", randomSpawnPosO, Quaternion.identity);
            isOExist = true;
            isOSpawnable = false;
        }
    }

    public void RoundOver(string winnerTag)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (winnerTag.Equals("Master"))
            {
                SetScore("Master");
            }
            else if (winnerTag.Equals("Client"))
            {
                SetScore("Client");
            }
            else
            {
                SetScore("Draw");
            }
        }

        ReloadScene();
    }

    public void SetScore(string tag)
    {
        gameObject.GetComponent<PhotonView>().RPC("SetScoreRPC", RpcTarget.AllBuffered, tag);
    }

    [PunRPC]
    public void SetScoreRPC(string tag)
    {
        if (tag.Equals("Master"))
        {
            _mastergameObject.GetComponent<PlayerScript>().score++;
            ScoreManager.Instance.masterScore = _mastergameObject.GetComponent<PlayerScript>().score;
        }
        else if (tag.Equals("Client"))
        {
            _clientgameObject.GetComponent<PlayerScript>().score++;
            ScoreManager.Instance.clientScore = _clientgameObject.GetComponent<PlayerScript>().score;
        }
        else
        {
            //DRAW
        }
    }

    public void ReloadScene()
    {
        //master ve client ilk yerine
        //tüm x ve o lar silinecek
        //slotların hepsi empty olacak
        //yeni x ve o oluşturulacak

        //if (PhotonNetwork.IsMasterClient)
        _mastergameObject.transform.position = new Vector3(spawnPosBottom.position.x, spawnPosBottom.position.y,
            spawnPosBottom.position.z);
        //if (!PhotonNetwork.IsMasterClient)
        _clientgameObject.transform.position =
            new Vector3(spawnPosTop.position.x, spawnPosTop.position.y, spawnPosTop.position.z);


        if (PhotonNetwork.IsMasterClient)
        {
            List<GameObject> xObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("X"));
            List<GameObject> oObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("O"));

            for (int i = 0; i < xObjects.Count; i++)
            {
                PhotonNetwork.Destroy(xObjects[i]);
            }

            for (int i = 0; i < oObjects.Count; i++)
            {
                PhotonNetwork.Destroy(oObjects[i]);
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<Slot>().isEmpty = true;
            slots[i].GetComponent<Slot>().stoneType = "Nothing";
        }

        //if(PhotonNetwork.IsMasterClient)
        isBeginning = true;
    }

    public void GameOver(string tag)
    {
        //gameover paneli çıkar
        //oyun durur
        //restart ve exit butonları olur
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);
        if (tag.Equals("Master"))
        {
            gameOverPanelWonText.GetComponent<Text>().text = "Master Won";
            gameOverPanelScoreText.GetComponent<Text>().text =
                ScoreManager.Instance.masterScore + " - " + ScoreManager.Instance.clientScore;
        }
        else
        {
            gameOverPanelWonText.GetComponent<Text>().text = "Client Won";
            gameOverPanelScoreText.GetComponent<Text>().text =
                ScoreManager.Instance.masterScore + " - " + ScoreManager.Instance.clientScore;
        }
    }

    public void SpawnRandomPowerUp()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject powerup;
            float randomXAxisForBottom = Random.Range(-4.8f, 4.8f);
            float randomZAxisForBottom = Random.Range(-4.8f, -2.8f);
            float randomXAxisForTop = Random.Range(-4.8f, 4.8f);
            float randomZAxisForTop = Random.Range(2.8f, 4.8f);

            Vector3 randomSpawnPosBottom = new Vector3(randomXAxisForBottom, 5.25f, randomZAxisForBottom);
            Vector3 randomSpawnPosTop = new Vector3(randomXAxisForTop, 5.25f, randomZAxisForTop);

            int powerUpChoice = Random.Range(0, 2);
            int positionChoice = Random.Range(0, 2);

            if (powerUpChoice == 1)
            {
                if (positionChoice == 1)
                {
                    powerup = PhotonNetwork.Instantiate("Speeder", randomSpawnPosBottom, Quaternion.identity);
                    StartCoroutine(DestroyPowerUp(powerup));
                }
                else
                {
                    powerup = PhotonNetwork.Instantiate("Speeder", randomSpawnPosTop, Quaternion.identity);
                    StartCoroutine(DestroyPowerUp(powerup));
                }
            }
            else
            {
                if (positionChoice == 1)
                {
                    powerup = PhotonNetwork.Instantiate("Blocker", randomSpawnPosBottom, Quaternion.identity);
                    StartCoroutine(DestroyPowerUp(powerup));
                }
                else
                {
                    powerup = PhotonNetwork.Instantiate("Blocker", randomSpawnPosTop, Quaternion.identity);
                    StartCoroutine(DestroyPowerUp(powerup));
                }
            }
        }
    }

    IEnumerator DestroyPowerUp(GameObject powerUp)
    {

        yield return new WaitForSeconds(5f);
        PhotonNetwork.Destroy(powerUp);

    }
    
}