using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;

public class Latency : MonoBehaviour
{
    PhotonView photonView;

    string path;

    private void Start()
    {
        path = Application.persistentDataPath + "/" + System.DateTime.Now.Hour.ToString() + "_" + System.DateTime.Now.Minute.ToString() + "_" + System.DateTime.Now.Second.ToString() + ".txt";
        CreateTxtFile();

        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("RPCMeasure", 0f, 10f);
        }
    }

    void CreateTxtFile()
    {
        File.WriteAllText(path, "Server Ping | Timestamp\n");
    }

    void RPCMeasure()
    {
        GetComponent<PhotonView>().RPC("MeasureAndWrite", RpcTarget.All);
    }

    [PunRPC]
    void MeasureAndWrite()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            string timestamp = System.DateTime.Now.Hour.ToString() + ":" + System.DateTime.Now.Minute.ToString() + ":" + System.DateTime.Now.Second.ToString() + "." + System.DateTime.Now.Millisecond.ToString();
            File.AppendAllText(path, PhotonNetwork.GetPing().ToString() + "  |  " + timestamp + "\n");
        }
    }

}
