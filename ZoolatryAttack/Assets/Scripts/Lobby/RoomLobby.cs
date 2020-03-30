using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomLobby : MonoBehaviour
{
    public Text RoomNameText;
    public Button JoinRoomButton;
    public string roomName;

    public void Start()
    {
        JoinRoomButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(roomName);
        });
    }

    public void Initialize(string name)
    {
        Debug.Log("Initializing room lobby");
        roomName = name;
        RoomNameText.text = name;
    }
}
