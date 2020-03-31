using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine.UI;

public abstract class PlayerBase : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public GameObject model;
    public Transform shootingPoint;
    public GameObject projectilePrefab;
    public Text debugStatusTxt;
    CharacterController ctrl;

    [Header("Move Values")]
    public float speed;
    public int magazineBullets;
    public int magazineCapacity;
    public int ammoCarrying;

    [Header("Other Values")]
    public bool isGrounded;
    float gravity = -9.81f;
    Vector3 velocity;
    Vector3 movePos;
    float fallJumpMultiplier = 2.5f;
    float groundDistance = 0.4f;

    float hInput;
    float vInput;

    public float playerShootSpeed;
    public float playerReloadTimer;
    bool reloading=false;

    float shootCooldown = 0f;
    float reloadTimer = 0f;

    private PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        ctrl = GetComponent<CharacterController>();
    }

    private void Start()
    {
        StartLocalVariables();
    }

    public abstract void StartLocalVariables();


    public void LeaveTheGame()
    {
        Zoolatry.PANEL_TO_BE_LOADED = 1;
        PhotonNetwork.LeaveRoom();
    }
    

    [PunRPC]
    public void Shoot(Vector3 pos,Quaternion rot,PhotonMessageInfo info)
    {
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);
        ShootProjectiles(pos,model.transform.forward,lag);
    }

    [PunRPC]
    public void DebugStatusText(string message)
    {
        debugStatusTxt.text = message;
    }

    public abstract void ShootProjectiles(Vector3 pos,Vector3 dir,float lag);

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);
        if (velocity.y < 0)
        {
            if (isGrounded)
                velocity.y = -.5f; // 0 would possibly make it feel like it's floating a bit
            else
            {
                velocity += Vector3.up * Physics.gravity.y * (fallJumpMultiplier - 1) * Time.deltaTime;
            }
        }


        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        movePos = transform.right * hInput + transform.forward * vInput;
        movePos = transform.TransformDirection(movePos);

        if (reloading)
        {
            if (reloadTimer > 0)
                reloadTimer -= Time.deltaTime;
            else
                ReloadGun();
        }
        if (shootCooldown > 0)
            shootCooldown -= Time.deltaTime;

        if (!PhotonNetwork.InRoom)
        { return; }

        if (Input.GetKeyDown(KeyCode.Space) && shootCooldown <= 0 && magazineBullets > 0 && !reloading)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Shooting...");

            photonView.RPC("Shoot",RpcTarget.AllViaServer,shootingPoint.position,model.transform.rotation);
            magazineBullets--;
            shootCooldown = Time.deltaTime / playerShootSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && magazineBullets <= 0 && reloadTimer <= 0 && ammoCarrying > 0)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Reloading...");
            reloadTimer = playerReloadTimer;
            reloading = true;
        }
        else if (!reloading)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Idle...");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Reloading...");
            reloadTimer = playerReloadTimer;
            reloading = true;
        }

        if (movePos.magnitude != 0)
        {
            Quaternion rot = Quaternion.LookRotation(movePos);
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,rot,.4f);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            LeaveTheGame();
        }
    }


    public void ReloadGun()
    {
        photonView.RPC("DebugStatusText",RpcTarget.All,"Idle...");

        reloading = false;

        int toBeReloaded = magazineCapacity - magazineBullets;
        if (toBeReloaded > ammoCarrying)
            toBeReloaded = magazineCapacity;

        ammoCarrying -= magazineBullets = toBeReloaded;
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        ctrl.Move(movePos.normalized * speed);
        velocity.y += gravity * Time.deltaTime;
        ctrl.Move(velocity * Time.deltaTime);
    }
}
