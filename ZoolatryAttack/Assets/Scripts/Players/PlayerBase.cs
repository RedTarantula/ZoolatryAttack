using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using static Zoolatry;

public abstract class PlayerBase : MonoBehaviour
{
    [Header("Points")]
    public Transform groundCheck;
    public Transform shootingPoint;
    public Transform cageHoldPos;

    [Header("Basic")]
    PhotonView photonView;
    ZoolatryManager zm;
    PlayerInfoHUD hud;
    CharacterController ctrl;
    public LayerMask groundMask;
    public GameObject model;
    public GameObject projectilePrefab;
    public GameObject touchingCage;

    [Header("UI")]
    public Text debugStatusTxt;
    public Image healthBar;

    [Header("Character")]
    public PLAYER_CHARACTER character;
    public PlayerVariables pVars;
    public PlayerGun pGun;

    [Header("Movement")]
    Vector3 velocity;
    Vector3 movePos;
    float hInput;
    float vInput;

    [Header("Timers")]
    float shootCooldown = 0f;
    float reloadTimer = 0f;

    [Header("States")]
    public bool isGrounded = false;
    public bool holdingCage = false;
    bool reloading = false;


    #region Unity Calls
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        ctrl = GetComponent<CharacterController>();
    }
    private void Start()
    {
        StartLocalVariables();
        hud = GameObject.Find("PlayerInfoHUD").GetComponent<PlayerInfoHUD>();
        if (photonView.IsMine)
        {
            hud.UpdateHudHP(pVars.healthCurrent,pVars.healthMax);
            hud.UpdateHudAmmo(pGun.ammoLoaded,pGun.ammoMagazineSize,pGun.ammoCarrying);
        }
        if (photonView.IsMine)
        {
            Camera.main.GetComponent<FollowPlayer>().pos = transform;
        }
    }
    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        isGrounded = Physics.CheckSphere(groundCheck.position,GAME_GROUND_DISTANCE_TEST,groundMask);
        if (velocity.y < 0)
        {
            if (isGrounded)
                velocity.y = -.5f; // 0 would possibly make it feel like it's floating a bit
            else
            {
                velocity += Vector3.up * Physics.gravity.y * (GAME_FALL_MULTIPLIER - 1) * Time.deltaTime;
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holdingCage && touchingCage != null)
            {
                PickupCage();
            }
            else if (holdingCage)
            {
                ReleaseCage();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && shootCooldown <= 0 && pGun.ammoLoaded > 0 && !reloading)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Shooting...");
            photonView.RPC("Shoot",RpcTarget.AllViaServer,shootingPoint.position,model.transform.rotation);
            pGun.ammoLoaded--;
            shootCooldown = Time.deltaTime / pGun.shootSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && pGun.ammoLoaded <= 0 && reloadTimer <= 0 && pGun.ammoCarrying > 0)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Reloading...");
            reloadTimer = pGun.reloadSpeed;
            reloading = true;
        }
        else if (!reloading)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Idle...");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Reloading...");
            reloadTimer = pGun.reloadSpeed;
            reloading = true;
        }
        if (movePos.magnitude != 0)
        {
            Quaternion rot = Quaternion.LookRotation(movePos);
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation,rot,.4f);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LeaveTheGame();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (touchingCage != null || holdingCage)
        { return; }
        if (other.gameObject.CompareTag("CagePickup"))
        {
            touchingCage = other.gameObject.transform.root.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (touchingCage == null || holdingCage)
        { return; }
        Debug.Log("Exited trigger with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("CagePickup"))
        {
            touchingCage = null;
        }
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        ctrl.Move(movePos.normalized * pVars.moveSpeed);
        velocity.y += GAME_GRAVITY * Time.deltaTime;
        ctrl.Move(velocity * Time.deltaTime);
    }
    #endregion
    #region RPCs
    [PunRPC]
    public void Shoot(Vector3 pos,Quaternion rot,PhotonMessageInfo info)
    {
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);
        ShootProjectiles(pos,model.transform.forward,lag);
        if (photonView.IsMine)
        {
            hud.UpdateHudAmmo(pGun.ammoLoaded,pGun.ammoMagazineSize,pGun.ammoCarrying);
        }
    }
    [PunRPC]
    public void SetHealth(float h)
    {
        pVars.healthCurrent = h;
        healthBar.fillAmount = pVars.healthCurrent / pVars.healthMax;
        healthBar.fillAmount = pVars.healthCurrent / pVars.healthMax;
        if (photonView.IsMine)
        {
            hud.UpdateHudHP(pVars.healthCurrent,pVars.healthMax);
        }
    }
    [PunRPC]
    public void DebugStatusText(string message)
    {
        debugStatusTxt.text = message;
    }
    #endregion
    #region  Pun Calls
    public void LeaveTheGame()
    {
        Zoolatry.PANEL_TO_BE_LOADED = 1;
        PhotonNetwork.LeaveRoom();
    }
    #endregion
    #region Variables Management
    public abstract void StartLocalVariables();
    public void SetAmmoInv(int ammount)
    {
        pGun.ammoCarrying = ammount;
        if (photonView.IsMine)
        {
            hud.UpdateHudAmmo(pGun.ammoLoaded,pGun.ammoMagazineSize,pGun.ammoCarrying);
        }
    }
    public void HealPercent(float h)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        float newHealth = pVars.healthCurrent + (h*pVars.healthMax);
        if (newHealth > pVars.healthMax)
        {
            newHealth = pVars.healthMax;
        }
        photonView.RPC("SetHealth",RpcTarget.All,newHealth);
    }
    public void DamageFloat(float d)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        float newHealth = pVars.healthCurrent - d;
        photonView.RPC("SetHealth",RpcTarget.All,newHealth);
    }
    public void Initialize(ZoolatryManager zManager)
    {
        zm = zManager;
    }
    #endregion
    #region Actions
    public abstract void ShootProjectiles(Vector3 pos,Vector3 dir,float lag);
    public abstract void PickupReaction(Zoolatry.PICKUP_TYPE pickupType);
    public void ReloadGun()
    {
        photonView.RPC("DebugStatusText",RpcTarget.All,"Idle...");
        reloading = false;
        int toBeReloaded = pGun.ammoMagazineSize - pGun.ammoLoaded;
        if (toBeReloaded > pGun.ammoCarrying)
            toBeReloaded = pGun.ammoCarrying;
        pGun.ammoCarrying -= pGun.ammoLoaded += toBeReloaded;
        if (photonView.IsMine)
        {
            hud.UpdateHudAmmo(pGun.ammoLoaded,pGun.ammoMagazineSize,pGun.ammoCarrying);
        }
    }
    public void PickupCage()
    {
        CageBaseScript cage = touchingCage.GetComponent<CageBaseScript>();
        if ((character == Zoolatry.PLAYER_CHARACTER.Chicky && touchingCage.GetComponent<CageBaseScript>().cagetype == Zoolatry.CAGE_TYPE.Medium) || touchingCage.GetComponent<CageBaseScript>().cagetype == Zoolatry.CAGE_TYPE.Large)
        {
            return;
        }
        Debug.Log($"Picked up a {cage.cagetype} cage");
        holdingCage = true;
        touchingCage.transform.SetParent(cageHoldPos);
        touchingCage.transform.localRotation = Quaternion.Euler(0,180,0);
        touchingCage.transform.localPosition = new Vector3(0,0,0);
        touchingCage.GetComponent<CageBaseScript>().beingHeld = true;
    }
    public void ReleaseCage()
    {
        Debug.Log("Released cage");
        touchingCage.GetComponent<CageBaseScript>().beingHeld = false;
        touchingCage.transform.SetParent(null);
        touchingCage = null;
        holdingCage = false;
    }
    #endregion
}
