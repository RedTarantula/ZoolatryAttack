using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviourPunCallbacks
{

    public void Restartlevel()
    {
        PhotonNetwork.LoadLevel("Farm2");
    }

    public void MainMenu()
    {
        Zoolatry.PANEL_TO_BE_LOADED = 1;
        PhotonNetwork.LeaveRoom();
    }


}
