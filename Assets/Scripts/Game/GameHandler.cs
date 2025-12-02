using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Xml;
using TMPro;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{

    MatchingHandler matchingHandler;
    Score score;
    CanvasUpdate canvasUpdate;
    PreviousGameQuestions previousGameQuestions;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private AudioSource victoryAudioSource;
    [SerializeField] private AudioSource gameMusicAudioSource;
    [SerializeField] private AudioSource rightAnsAudioSource;
    [SerializeField] private AudioSource wrongAnsAudioSource;
    int nextBundleIndex;

    bool gameStarting = false, gameStarted = false, bundleSet = false, p2netStop = false;

    void Start()
    {
        matchingHandler = gameObject.GetComponent<MatchingHandler>();
        score = gameObject.GetComponent<Score>();
        canvasUpdate = gameObject.GetComponent<CanvasUpdate>();

        previousGameQuestions = gameObject.GetComponent<PreviousGameQuestions>();
        previousGameQuestions.LoadData();
        previousGameQuestions.EmptyFile();
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Check if both players are connected
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !gameStarting)
            {
                GetComponent<PhotonView>().RPC("StartingSequenceUtil", RpcTarget.All);
            }

            if (gameStarted)
            {
                //Check if either player has reached winning score
                if (score.player1score == 10 || score.player2score == 10)
                {
                    GetComponent<PhotonView>().RPC("EndingSequenceUtil", RpcTarget.All);
                    gameStarted = false;
                    bundleSet = true;
                }

                //Set up next matches and question if the previous has been answered
                if (!bundleSet)
                {
                    bundleSet = true;

                    GetComponent<PhotonView>().RPC("ChangeP2stop", RpcTarget.Others, false);

                    nextBundleIndex = matchingHandler.GetRandomMatchingIndex();
                    GetComponent<PhotonView>().RPC("ChangeNextBundleIndex", RpcTarget.Others, nextBundleIndex);
                    GetComponent<PhotonView>().RPC("RemoveBundleFromList", RpcTarget.Others);

                    matchingHandler.SetMatchingPairs(nextBundleIndex);

                    StartCoroutine(matchingHandler.ResetPaperPositionCoroutine(0, 7));
                    GetComponent<PhotonView>().RPC("ResetPaperPositionsUtil", RpcTarget.Others, 0, 7);

                    canvasUpdate.GetTexts();
                    GetComponent<PhotonView>().RPC("UpdateTextsUtil", RpcTarget.Others, canvasUpdate.allTexts);

                    canvasUpdate.GetImages();
                    GetComponent<PhotonView>().RPC("UpdateImagesUtil", RpcTarget.Others, canvasUpdate.allImageNames);
                }

                //Check if player 1 has placed all papers in the sockets
                if (matchingHandler.CheckIfSocketsAreFilled(0, 3))
                {
                    //Check if player 1 has placed the papers correctly
                    if (matchingHandler.CheckIfMatchesAreCorrect(nextBundleIndex, 0, 3))
                    {
                        score.PointToP1();
                        StartCoroutine(matchingHandler.ResetPaperPositionCoroutine(0, 7));
                        GetComponent<PhotonView>().RPC("ResetPaperPositionsUtil", RpcTarget.Others, 0, 7);
                        bundleSet = false;
                        rightAnsAudioSource.Play();

                        previousGameQuestions.AddToList(nextBundleIndex, "yes");
                        GetComponent<PhotonView>().RPC("AddToList", RpcTarget.Others, nextBundleIndex, "no");

                    }
                    else if (!matchingHandler.CheckIfMatchesAreCorrect(nextBundleIndex, 0, 3))
                    {
                        GetComponent<PhotonView>().RPC("ResetPaperPositionsUtil", RpcTarget.All, 0, 3);
                        wrongAnsAudioSource.Play();
                    }
                }

            }
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            //Check if player 2 has placed all papers in the sockets
            if (matchingHandler.CheckIfSocketsAreFilled(4, 7))
            {
                //Check if player 2 has placed the papers correctly
                if (matchingHandler.CheckIfMatchesAreCorrect(nextBundleIndex, 4, 7) && p2netStop == false)
                {
                    p2netStop = true;
                    StartCoroutine(matchingHandler.ResetPaperPositionCoroutine(0, 7));
                    GetComponent<PhotonView>().RPC("ResetPaperPositionsUtil", RpcTarget.Others, 0, 7);
                    GetComponent<PhotonView>().RPC("ChangeBundleSetBool", RpcTarget.MasterClient, false);
                    GetComponent<PhotonView>().RPC("PointToP2", RpcTarget.MasterClient);
                    rightAnsAudioSource.Play();

                    previousGameQuestions.AddToList(nextBundleIndex, "yes");
                    GetComponent<PhotonView>().RPC("AddToList", RpcTarget.Others, nextBundleIndex, "no");
                }
                else if (!matchingHandler.CheckIfMatchesAreCorrect(nextBundleIndex, 4, 7))
                {
                    GetComponent<PhotonView>().RPC("ResetPaperPositionsUtil", RpcTarget.All, 4, 7);
                    wrongAnsAudioSource.Play();
                }
            }
        }

    }

    [PunRPC]
    public void StartingSequenceUtil()
    {
        StartCoroutine(StartingSequenceCoroutine());
    }

    IEnumerator StartingSequenceCoroutine()
    {
        gameStarting = true;

        countdownText.text = "Το παιχνίδι θα αρχίσει σύντομα!";
        yield return new WaitForSeconds(20);
        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        gameMusicAudioSource.Play();

        gameStarted = true;

    }

    [PunRPC]
    public void EndingSequenceUtil()
    {
        StartCoroutine(EndingSequenceCoroutine());
    }

    IEnumerator EndingSequenceCoroutine()
    {
        previousGameQuestions.SaveToXML();

        if(score.player1score == 10)
        {
            questionText.text = "Ο παίκτης 1 κέρδισε!";
        }
        else if (score.player2score == 10)
        {
            questionText.text = "Ο παίκτης 2 κέρδισε!";
        }

        if (PhotonNetwork.IsMasterClient)
        {
            canvasUpdate.GetTexts();
            GetComponent<PhotonView>().RPC("UpdateTextsUtil", RpcTarget.Others, canvasUpdate.allTexts);
        }

        victoryAudioSource.Play();

        yield return new WaitForSeconds(10);

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("StartingMenu");
    }

    [PunRPC]
    void ChangeBundleSetBool(bool _bool)
    {
        bundleSet = _bool;
    }

    [PunRPC]
    void ChangeNextBundleIndex(int index)
    {
        nextBundleIndex = index;
    }

    [PunRPC]
    void RemoveBundleFromList()
    {
        matchingHandler.usedMatches.Add(matchingHandler.matches[nextBundleIndex]);
    }

    [PunRPC]
    void ChangeP2stop(bool _bool)
    {
        p2netStop = _bool;
    }
}
