using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    public float speed = 2f;
    public bool hasStone;
    private bool canInsert;
    public int score;

    public GameObject carryingStone;
    public GameObject insertedSlot;

    public String carryingStoneType;

    // Start is called before the first frame update
    void Start()
    {
        hasStone = false;
        canInsert = false;
        score = 0;
        carryingStone = new GameObject();
        insertedSlot = new GameObject();
        carryingStoneType = "Nothing";
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;

        Vector3 movement = new Vector3();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x = -1f;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x = 1f;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.z = 1f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.z = -1f;
        }

        if (Input.GetKey(KeyCode.Space) && canInsert)
        {
            GameManager.Instance.InsertStone(carryingStone.GetComponent<PhotonView>().ViewID,
                insertedSlot.GetComponent<PhotonView>().ViewID);
            if (carryingStone.activeSelf)
            {
                hasStone = false;
                canInsert = false;    
            }
            
        }

        transform.Translate(movement * speed * Time.deltaTime);

        if (!hasStone && !GameManager.Instance.isBeginning)
        {
            if (carryingStoneType.Contains("X") && !GameManager.Instance.isXExist && GameManager.Instance.isXSpawnable)
                GameManager.Instance.SpawnX();
            if (carryingStoneType.Contains("O") && !GameManager.Instance.isOExist &&
                GameManager.Instance.isOSpawnable)
                GameManager.Instance.SpawnO();
            
                
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Blocker"))
        {
            //TODO: Blocker Power
            other.gameObject.SetActive(false);
            //rakibin speedini 0 yap
            
            if(gameObject.CompareTag("Master"))
                BlockerPowerUp(gameObject.tag);
            else if(gameObject.CompareTag("Client"))
                BlockerPowerUp(gameObject.tag);
            
        }

        if (other.CompareTag("Speeder"))
        {
            other.gameObject.SetActive(false);
            //TODO: Hızını 5 yap 5 saniye için
            speed = 5f;
            Color defaultColor = Color.gray;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            StartCoroutine(RevertSpeed(this, 5f));
            StartCoroutine(RevertColor(gameObject, defaultColor, 5f));

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Slot") && hasStone)
        {
            canInsert = true;
            insertedSlot = other.gameObject;
        }
    }

    IEnumerator RevertSpeed(PlayerScript playerScript, float waitingSeconds)
    {
        yield return new WaitForSeconds(waitingSeconds);
        playerScript.speed = 2f;
    }

    IEnumerator RevertColor(GameObject gameObject, Color defaultColor, float waitingSeconds)
    {
        yield return new WaitForSeconds(waitingSeconds);
        gameObject.GetComponent<MeshRenderer>().material.color = defaultColor;
    }

    public void BlockerPowerUp(string tag)
    {
        photonView.RPC("BlockerPowerUpRPC", RpcTarget.AllBuffered, tag);
    }

    [PunRPC]
    public void BlockerPowerUpRPC(string tag)
    {
        if (tag.Equals("Master"))
        {
            GameManager.Instance._clientgameObject.GetComponent<PlayerScript>().speed = 0f;
            Color defaultColor = Color.gray;
            GameManager.Instance._clientgameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            StartCoroutine(RevertSpeed(GameManager.Instance._clientgameObject.GetComponent<PlayerScript>(), 2f));
            StartCoroutine(RevertColor(GameManager.Instance._clientgameObject, defaultColor, 2f));
        }
        else if(tag.Equals("Client"))
        {
            GameManager.Instance._mastergameObject.GetComponent<PlayerScript>().speed = 0f;
            Color defaultColor = Color.gray;
            GameManager.Instance._mastergameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            StartCoroutine(RevertSpeed(GameManager.Instance._mastergameObject.GetComponent<PlayerScript>(), 2f));
            StartCoroutine(RevertColor(GameManager.Instance._mastergameObject, defaultColor, 2f));
        }
    }
    
}