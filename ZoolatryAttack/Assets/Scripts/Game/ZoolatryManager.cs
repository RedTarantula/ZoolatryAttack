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

    public float chickyHP;
    public float piggyHP;


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

        chickyHP = Zoolatry.CHICKY_HP_MAX;
        piggyHP = Zoolatry.PIGGY_HP_MAX;
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }


    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }


    [PunRPC]
    public void DamagePlayer(Zoolatry.PLAYER_CHARACTER character,float damage)
    {
        if (character == Zoolatry.PLAYER_CHARACTER.Chicky)
        {
            chickyHP -= damage;
        }
        else if (character == Zoolatry.PLAYER_CHARACTER.Piggy)
        {
            piggyHP -= damage;
        }
    }
    [PunRPC]
    public void HealPlayerPercentage(Zoolatry.PLAYER_CHARACTER character,float percentage)
    {
        if (character == Zoolatry.PLAYER_CHARACTER.Chicky)
        {
            chickyHP += Zoolatry.CHICKY_HP_MAX * percentage;
            if (chickyHP > Zoolatry.CHICKY_HP_MAX)
            {
                chickyHP = Zoolatry.CHICKY_HP_MAX;
            }
        }
        else if (character == Zoolatry.PLAYER_CHARACTER.Piggy)
        {
            piggyHP += Zoolatry.PIGGY_HP_MAX * percentage;
            if (piggyHP > Zoolatry.PIGGY_HP_MAX)
            {
                piggyHP = Zoolatry.PIGGY_HP_MAX;
            }
        }
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

        void StartGame() { SpawnPlayer(); 
    }

    void SpawnPlayer()
    {
        //if(PhotonNetwork.LocalPlayer.ActorNumber == 1)
        //{
        //}
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            PhotonNetwork.Instantiate("PiggyPlayer",spawnerPlayer1.position,Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("ChickyPlayer",spawnerPlayer2.position,Quaternion.identity);
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
