using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OwnershipTakeover : MonoBehaviourPun
{
    [SerializeField] private List<GameObject> GrabbablePapers = new List<GameObject>();
    private bool ownershipSet = false;

    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.LocalPlayer.ActorNumber == 2 && ownershipSet == false)
        {
            for(int i = 4; i < GrabbablePapers.Count; i++)
            {
                GrabbablePapers[i].GetComponent<PhotonView>().RequestOwnership();
                ownershipSet = true;
            }
        }

    }
}
