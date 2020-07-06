using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMainPanel : MonoBehaviourPunCallbacks
{
    // Needs to be organized
    // Needs to lock inputs/buttons on the right time
    // Needs feedback on connecting process

    [Header("Multiplayer Connecting Buttons")]
    public Button[] allButtons;

    [Header("Main Menu Panel")]
    public GameObject MainMenuPanel;
    public MultiplayerStatusPanel mpS;

    [Header("Login Panel")]
    public GameObject LoginPanel;
    public InputField PlayerNameInput;

    [Header("Selection Panel")]
    public GameObject SelectionPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public InputField RoomNameInputField;

    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;

    [Header("Room List Panel")]
    public GameObject RoomListPanel;

    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;

    public GameObject player1Entry;
    public GameObject player2Entry;
    public Button StartGameButton;
    public GameObject PlayerLobbyPrefab;

    [Header("Inside Room Panel")]
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        cachedRoomList = new Dictionary<string,RoomInfo>();
        roomListEntries = new Dictionary<string,GameObject>();

        PlayerNameInput.text = "Player " + UnityEngine.Random.Range(1000,10000);
    }

    private void Start()
    {
        if(Zoolatry.PANEL_TO_BE_LOADED == 1)
        {
            SetActivePanel(SelectionPanel.name);
        }
        else
        {
            SetActivePanel(MainMenuPanel.name);
        }
        Zoolatry.PANEL_TO_BE_LOADED = 0;
    }

    #region On Callbacks
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = null;
        MultiplayerButtonsState(true);

        if (newPlayer.ActorNumber == 1)
        {
            entry = player1Entry;
        }
        else
        {
            entry = player2Entry;
        }

        entry.GetComponent<PlayerLobby>().Initialize(newPlayer.ActorNumber,newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber,entry);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerListEntries[otherPlayer.ActorNumber].GetComponent<PlayerLobby>().PlayerNameText.text = "Connecting";
        playerListEntries[otherPlayer.ActorNumber].GetComponent<PlayerLobby>().PlayerNameText.fontStyle = FontStyle.Italic;
        playerListEntries.Remove(otherPlayer.ActorNumber);
    }
    public override void OnConnectedToMaster()
    {
        mpS.SetStatus("Logged in as " + PhotonNetwork.LocalPlayer.NickName);
        this.SetActivePanel(SelectionPanel.name);
        MultiplayerButtonsState(true);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list changed");
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinRoomFailed(short returnCode,string message)
    {
        MultiplayerButtonsState(true);
    }
    public override void OnJoinedLobby()
    {
        MultiplayerButtonsState(true);
    }
    public override void OnJoinedRoom()
    {
        SetActivePanel(InsideRoomPanel.name);
        MultiplayerButtonsState(true);

        if(PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
        mpS.SetStatus("Created room");
        }
            else
        {
        mpS.SetStatus("Joined room");
        }

        StartGameButton.gameObject.SetActive(PhotonNetwork.LocalPlayer.ActorNumber == 1);

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int,GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = null;

            if (p.ActorNumber == 1)
            {
                entry = player1Entry;
            }
            else
            {
                entry = player2Entry;
            }

            entry.GetComponent<PlayerLobby>().Initialize(p.ActorNumber,p.NickName);

            playerListEntries.Add(p.ActorNumber,entry);
        }

        //StartGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable
            {
                {Zoolatry.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void OnLeaveMultiplayer()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.Disconnect();
        SetActivePanel(MainMenuPanel.name);
        mpS.gameObject.SetActive(false);
    }
    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }
    public override void OnLeftRoom()
    {
        mpS.SetStatus("Left room");
        MultiplayerButtonsState(false);
        SetActivePanel(SelectionPanel.name);

        if (playerListEntries.Count > 0)
        {
            foreach (GameObject entry in playerListEntries.Values)
            {
                entry.GetComponent<PlayerLobby>().PlayerNameText.text = "Connecting";
                entry.GetComponent<PlayerLobby>().PlayerNameText.fontStyle = FontStyle.Italic;
            }
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }
    #endregion

    #region On Fails
    public override void OnJoinRandomFailed(short returnCode,string message)
    {
        mpS.SetStatus("Failed joining random");
        string roomName = PhotonNetwork.LocalPlayer.NickName +"'s Room";
        RoomOptions options = new RoomOptions {MaxPlayers = 2};
        PhotonNetwork.CreateRoom(roomName,options,null);
    }
    public override void OnCreateRoomFailed(short returnCode,string message)
    {
        mpS.SetStatus("Failed creating room");
        MultiplayerButtonsState(true);
    }
    #endregion

    #region Utilities
    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name,info);
            }
        }
    }
    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            Debug.Log(entry.GetComponent<RoomLobby>());
            entry.GetComponent<RoomLobby>().Initialize(info.Name);

            roomListEntries.Add(info.Name,entry);
        }
    }
    private void SetActivePanel(string activePanel)
    {
        MainMenuPanel.SetActive(activePanel.Equals(MainMenuPanel.name));
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
        RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
        InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    }

    void MultiplayerButtonsState(bool state)
    {
        PlayerNameInput.interactable = state;
        foreach (Button b in allButtons)
        {
            b.interactable = state;
        }
    }
    #endregion

    #region On Clicked
    public void OnQuickJoinClicked()
    {
        
        mpS.SetStatus("Joining random lobby");
        MultiplayerButtonsState(false);
        SetActivePanel(JoinRandomRoomPanel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLoginClicked()
    {
        string playerName = PlayerNameInput.text;

        mpS.SetStatus("Logging in");

        MultiplayerButtonsState(false);
        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");
        }
    }
    public void OnCreateRoomClicked()
    {
        mpS.SetStatus("Creating room");
        MultiplayerButtonsState(false);
        string roomName = PhotonNetwork.LocalPlayer.NickName +"'s Room";
        RoomOptions options = new RoomOptions {MaxPlayers = 2};
        PhotonNetwork.CreateRoom(roomName,options,null);
    }
    public void OnMultiplayerClicked()
    {
        mpS.SetStatus("Not logged in");
        SetActivePanel(LoginPanel.name);

        mpS.gameObject.SetActive(true) ;
    }
    public void OnRoomListClicked()
    {
        mpS.SetStatus("Listing rooms");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        SetActivePanel(RoomListPanel.name);
    }
    public void OnLeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnReturnToSelectionPanelClicked()
    {
        SetActivePanel(SelectionPanel.name);
    }

    public void OnStartGameButtonClicked()
    {
        mpS.SetStatus("Starting game");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("Farm2");
    }

    #endregion

}