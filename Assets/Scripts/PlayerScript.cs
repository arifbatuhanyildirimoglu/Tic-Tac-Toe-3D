using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    private float speed = 2f;
    public bool hasStone;
    private bool canInsert;

    public GameObject carryingStone;
    public GameObject insertedSlot;

    public String carryingStoneType;

    // Start is called before the first frame update
    void Start()
    {
        hasStone = false;
        canInsert = false;
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
            //transform.DetachChildren();
            hasStone = false;
            canInsert = false;
        }

        if (Input.GetKey(KeyCode.I))
        {
            Debug.Log(hasStone);
            Debug.Log(canInsert);
        }

        transform.Translate(movement * speed * Time.deltaTime);

        if (!hasStone && !GameManager.Instance.isBeginning)
        {
            
            Debug.Log("Is O EXIST: " + GameManager.Instance.isOExist);
            Debug.Log("Is O SPAWNABLE: " + GameManager.Instance.isOSpawnable);
            
            if (carryingStoneType.Contains("X") && !GameManager.Instance.isXExist && GameManager.Instance.isXSpawnable)
                GameManager.Instance.SpawnX();
            if (carryingStoneType.Contains("O") && !GameManager.Instance.isOExist &&
                GameManager.Instance.isOSpawnable)
                GameManager.Instance.SpawnO();
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
}