using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawnAndSynch : MonoBehaviour
{
    //private PhotonView photonView;

    [SerializeField] private GameObject playerRig;
    [SerializeField] private GameObject playerModel;

    [SerializeField] private Vector3 player1pos, player2pos;
    [SerializeField] private Quaternion player1rot, player2rot;

    private GameObject player1model, player2model, player1rig, player2rig;
    private Transform modelHead, modelLeftHand, modelRightHand, rigCamera, rigLeftController, rigRightController;

    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            player1rig = Instantiate(playerRig, player1pos, player1rot);
            player1model = PhotonNetwork.Instantiate(playerModel.name, player1pos, player1rot);

            modelHead = player1model.transform.Find("Head");
            modelLeftHand = player1model.transform.Find("LeftHand");
            modelRightHand = player1model.transform.Find("RightHand");

            rigCamera = player1rig.transform.Find("Camera Offset/Main Camera");
            rigLeftController = player1rig.transform.Find("Camera Offset/LeftController");
            rigRightController = player1rig.transform.Find("Camera Offset/RightController");
        }
        else if(PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            player2rig = Instantiate(playerRig, player2pos, player2rot);
            player2model = PhotonNetwork.Instantiate(playerModel.name, player2pos, player2rot);

            modelHead = player2model.transform.Find("Head");
            modelLeftHand = player2model.transform.Find("LeftHand");
            modelRightHand = player2model.transform.Find("RightHand");

            rigCamera = player2rig.transform.Find("Camera Offset/Main Camera");
            rigLeftController = player2rig.transform.Find("Camera Offset/LeftController");
            rigRightController = player2rig.transform.Find("Camera Offset/RightController");
        }

            modelHead.gameObject.SetActive(false);
            modelLeftHand.gameObject.SetActive(false);
            modelRightHand.gameObject.SetActive(false);

    }

    private void Update()
    {
            modelHead.position = rigCamera.position;
            modelLeftHand.position = rigLeftController.position;
            modelRightHand.position = rigRightController.position;

            modelHead.rotation = rigCamera.rotation;
            modelLeftHand.rotation = rigLeftController.rotation;
            modelRightHand.rotation = rigRightController.rotation;
    }

}
