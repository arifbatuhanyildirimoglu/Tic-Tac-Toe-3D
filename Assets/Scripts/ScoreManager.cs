using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    public int masterScore;

    public int clientScore;

    public GameObject masterScoreText;
    public GameObject clientScoreText;

    // Start is called before the first frame update
    void Start()
    {
        masterScore = 0;
        clientScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        masterScoreText.GetComponent<Text>().text = masterScore.ToString();
        clientScoreText.GetComponent<Text>().text = clientScore.ToString();

        if (masterScore == 3)
        {
            GameManager.Instance.GameOver("Master");
        }

        if (clientScore == 3)
        {
            GameManager.Instance.GameOver("Client");
        }
        
    }

}