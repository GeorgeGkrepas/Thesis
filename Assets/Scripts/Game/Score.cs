using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text player1ScoreText, player2ScoreText;
    public int player1score, player2score;

    private void Start()
    {
        player1score = 0;
        player2score = 0;
    }

    public void PointToP1()
    {
        player1score++;

        player1ScoreText.text = player1score + "/10";
    }

    [PunRPC]
    public void PointToP2()
    {
        player2score++;

        player2ScoreText.text = player2score + "/10";
    }

}
