using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using System;

public class PlayerLobby : MonoBehaviour
{
    [Header("UI References")]
    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

    private int ownerId;
    private bool isPlayerReady;


    public void Initialize(int playerId,string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
        PlayerNameText.fontStyle = FontStyle.Normal;
    }

    public void Start()
    {
        //if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        //{
        //    PlayerReadyButton.gameObject.SetActive(false);
        //}
        //else
        //{
        //    //Hashtable initialProps = new Hashtable() {{Zoolatry.PLAYER_READY, isPlayerReady}};
        //    //PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

        //    PlayerReadyButton.onClick.AddListener(() =>
        //    {
        //        isPlayerReady = !isPlayerReady;

        //        //Hashtable props = new Hashtable() {{Zoolatry.PLAYER_READY, isPlayerReady}};
        //        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        //        if (PhotonNetwork.IsMasterClient)
        //        {
        //            FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
        //        }
        //    });
        //}
    }
}
