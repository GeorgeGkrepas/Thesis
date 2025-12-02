using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ServerHandling : MonoBehaviourPunCallbacks
{

    [SerializeField] private TMP_InputField createInput;
    [SerializeField] private TMP_InputField joinInput;

    [SerializeField] private GameObject CreateRoomMenu, JoinRoomMenu, LoadingScreen;
    [SerializeField] private Button ConfirmCreate, ConfirmJoin;

    private void Start()
    {
        ConfirmCreate.onClick.AddListener(StartConnecting);
        ConfirmJoin.onClick.AddListener(StartConnecting);
    }

    public void StartConnecting()
    {

        CreateRoomMenu.SetActive(false);
        JoinRoomMenu.SetActive(false);
        LoadingScreen.SetActive(true);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if(createInput.text != "")
        {
            CreateRoom();
        }
        else if(joinInput.text != "")
        {
            JoinRoom();
        }

    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.PlayerTtl = 0;
        PhotonNetwork.CreateRoom(createInput.text, roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MultiplayerGame");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        JoinRoomMenu.SetActive(true);
        LoadingScreen.SetActive(false);
        PhotonNetwork.Disconnect();
        createInput.text = "";
        joinInput.text = "";
    }
    
}
