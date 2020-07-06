using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using static Zoolatry;

public abstract class PlayerBase : MonoBehaviourPunCallbacks
{
    [Header("Points")]
    public Transform groundCheck;
    public Transform shootingPoint;
    public Transform cageHoldPos;

    [Header("Basic")]
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


    [Header("Movement")]
    Vector3 velocity;
    Vector3 movePos;
    float hInput;
    float vInput;
    public Animator anim;

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
        ctrl = GetComponent<CharacterController>();
    }
    private void Start()
    {
        StartLocalVariables();
        hud = GameObject.Find("PlayerInfoHUD").GetComponent<PlayerInfoHUD>();
        if (photonView.IsMine)
        {
            hud.UpdateHudHP(pVars.healthCurrent,pVars.healthMax);
            hud.UpdateHudAmmo(pVars.ammoLoaded,pVars.ammoMagazineSize,pVars.ammoCarrying);
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
        if(hInput != 0 || vInput !=0){
            anim.SetBool("Reloading", false);
            anim.SetBool("Running", true);
            anim.SetBool("Walking", false);
            anim.SetBool("Shooting", false);
            anim.SetBool("Idle", false);
        }
        else
        {
            anim.SetBool("Reloading", false);
            anim.SetBool("Running", false);
            anim.SetBool("Walking", false);
            anim.SetBool("Shooting", false);
            anim.SetBool("Idle", true);
        }
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
                photonView.RPC("DebugStatusText",RpcTarget.All,"Carrying cage...");
                PickupCage();
            }
            else if (holdingCage)
            {
                photonView.RPC("DebugStatusText",RpcTarget.All,"Releasing cage...");
                ReleaseCage();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && shootCooldown <= 0 && pVars.ammoLoaded > 0 && !reloading)
        {
           anim.SetTrigger("Shooting");
            photonView.RPC("DebugStatusText",RpcTarget.All,"Shooting...");
            photonView.RPC("Shoot",RpcTarget.AllViaServer,shootingPoint.position,model.transform.rotation);
            pVars.ammoLoaded--;
            shootCooldown = Time.deltaTime / pVars.shootSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && pVars.ammoLoaded <= 0 && reloadTimer <= 0 && pVars.ammoCarrying > 0)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Reloading...");
            reloadTimer = pVars.reloadSpeed;
            reloading = true;
        }
        else if (!reloading)
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Idle...");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            photonView.RPC("DebugStatusText",RpcTarget.All,"Reloading...");
            reloadTimer = pVars.reloadSpeed;
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
            hud.UpdateHudAmmo(pVars.ammoLoaded,pVars.ammoMagazineSize,pVars.ammoCarrying);
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
        pVars.ammoCarrying = ammount;
        if (photonView.IsMine)
        {
            hud.UpdateHudAmmo(pVars.ammoLoaded,pVars.ammoMagazineSize,pVars.ammoCarrying);
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
        anim.SetTrigger("Reloading");
        photonView.RPC("DebugStatusText",RpcTarget.All,"Idle...");
        reloading = false;
        int toBeReloaded = pVars.ammoMagazineSize - pVars.ammoLoaded;
        if (toBeReloaded > pVars.ammoCarrying)
            toBeReloaded = pVars.ammoCarrying;
        pVars.ammoCarrying -= pVars.ammoLoaded += toBeReloaded;
        if (photonView.IsMine)
        {
            hud.UpdateHudAmmo(pVars.ammoLoaded,pVars.ammoMagazineSize,pVars.ammoCarrying);
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
