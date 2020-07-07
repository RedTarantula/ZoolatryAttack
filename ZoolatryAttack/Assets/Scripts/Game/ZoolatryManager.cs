using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public class ZoolatryManager : MonoBehaviourPunCallbacks
{
    public static ZoolatryManager instance = null;
    public Transform spawnerPlayer1;
    public Transform spawnerPlayer2;



    public GameObject pauseMenuObj;

    private void Awake()
    {
        instance = this;
    }
    public override void OnEnable()
    {
        base.OnEnable();
    }
    private void Start()
    {
        Hashtable props = new Hashtable
            {
                {Zoolatry.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (pauseMenuObj == null)
        {
            pauseMenuObj = GameObject.Find("PauseMenu");
        }
        pauseMenuObj.SetActive(false);
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }


    private void Update()
    {
        
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Zoolatry.PANEL_TO_BE_LOADED = 1;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
         if (changedProps.ContainsKey(Zoolatry.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                StartGame();
                }
            }
    }
    //

    void StartGame()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Debug.Log($"Local player: {PhotonNetwork.LocalPlayer.ActorNumber}");
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate("PiggyPlayer",spawnerPlayer1.position,Quaternion.identity).GetComponent<PlayerBase>().Initialize(this);
        }
        else
        {
            PhotonNetwork.Instantiate("ChickyPlayer",spawnerPlayer2.position,Quaternion.identity).GetComponent<PlayerBase>().Initialize(this);
        }
    }

    private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(Zoolatry.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

}
