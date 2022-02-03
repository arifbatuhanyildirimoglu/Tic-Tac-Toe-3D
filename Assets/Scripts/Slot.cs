using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool isEmpty;
    public String stoneType;

    // Start is called before the first frame update
    void Start()
    {
        isEmpty = true;
        stoneType = "Nothing";
    }

    // Update is called once per frame
    void Update()
    {
    }
}