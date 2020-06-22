using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using static Zoolatry;
using UnityEngine.AI;
using System.Runtime.CompilerServices;

public class GuardBase : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public GameObject model;
    public Transform shootingPoint;
    public GameObject projectilePrefab;
    public ZoolatryManager zm;
    public Text statusTxt;
    public Image hpBar;
    NavMeshAgent nma;
    CharacterController ctrl;

    [Header("Guard")]
    public GUARD_CHARACTER guard;
    [SerializeField] public GuardVariables gVars;

    [Header("Movement")]
    Vector3 velocity;
    public Vector3 movePos;
    float scoutDist = 0f;

    [Header("Timers")]
    float shootCooldown = 0f;
    float reloadTimer = 0f;
    float guardTimer = 0f;

    [Header("States")]
    public bool isGrounded = false;
    public bool holdingCage = false;
    bool reloading = false;

    private void Awake()
    {
        ctrl = GetComponent<CharacterController>();
        nma = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        gVars = new GuardVariables(GUARD_CHARACTER.Base);
        nma.speed = gVars.moveSpeed*10;

        SetState(GUARD_STATE.Scouting);
        StartScouting();

        UpdateUI();
    }
    private void Update()
    {
        EnemyAI();
        UpdateUI();

    }

    void UpdateUI()
    {
        statusTxt.text = gVars.state.ToString();
        hpBar.fillAmount = gVars.healthCurrent / gVars.healthMax;
    }

    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(this.beingHeld);
        //}
        //else
        //{
        //    this.beingHeld = (bool)stream.ReceiveNext();
        //}
    }

    public void EnemyAI()
    {
        switch (gVars.state)
        {
            case GUARD_STATE.Scouting:
                if (scoutDist <= gVars.moveSpeed*1.5f || nma.remainingDistance < 2f)
                {
                    Debug.Log("Reached point");
                    SetState(GUARD_STATE.Guarding);
                    guardTimer = gVars.guardDuration;
                    break;
                }
                FindClosestPlayer();
                ScoutToDirection();
                break;
                //=------------------=

            case GUARD_STATE.Following:
                if (gVars.target == null) return;
                if (gVars.TgtDistance(transform) <= gVars.tgtDistShoot)
                {
                    //gVars.state = GUARD_STATE.Shooting;
                }
                break;
                //=------------------=

            case GUARD_STATE.Guarding:
                if (guardTimer <= 0)
                {
                    SetState(GUARD_STATE.Scouting);
                    StartScouting();
                    break;
                }
                //FindClosestPlayer();
                guardTimer -= Time.deltaTime;
                break;
                //=------------------=

            case GUARD_STATE.Shooting:
                if (gVars.target == null) return;

                if (gVars.TgtDistance(transform) >= gVars.tgtDistFollow)
                {
                    //gVars.state = GUARD_STATE.Following;
                }
                break;
                //=------------------=

            case GUARD_STATE.Capturing:
                break;
                //=------------------=

            default:
                break;
        }

        if (gVars.target == null)
            return;
     
        if(gVars.TgtDistance(transform) > gVars.tgtDistAggro) // When too far from the player, lose it as a target and start scouting
        {
            gVars.target = null;
            gVars.state = GUARD_STATE.Scouting;
            return;
        }

        
    }
    public void FindClosestPlayer()
    {
        // If finds a player within range, change to following
        
        Collider[] playersInRange = Physics.OverlapSphere(transform.position,gVars.tgtDistAggro,10);
        if(playersInRange.Length <= 0)
        { return; }

        GameObject newTgt = playersInRange[0].gameObject;
        if(playersInRange.Length > 1)
        {
            if(Vector3.Distance(transform.position,playersInRange[1].transform.position) > Vector3.Distance(transform.position,playersInRange[0].transform.position))
            {
                newTgt = playersInRange[1].gameObject;
            }
        }
        gVars.target = newTgt;
           // gVars.state = GUARD_STATE.Following;

    }
    public void ApproachTarget()
    {

    }
    public void ShootTarget()
    {

    }
    public void ReloadGun()
    {

    }

    public void StartScouting()
    {
        Debug.Log("Starting scouting");

        movePos = transform.position + Random.insideUnitSphere * gVars.scoutDist;
        NavMeshHit hit;
        bool b = NavMesh.SamplePosition(movePos,out hit,.5f,NavMesh.AllAreas);

        Debug.Log($"{b} - {movePos} to {hit.position}");
        // nma.SetDestination(hit.position);
        if (b)
        {
            movePos = hit.position;
        }
        
            nma.SetDestination(movePos);
            scoutDist = Vector3.Distance(transform.position,nma.destination);
        Debug.Log(nma.destination);
    }

    public void ScoutToDirection()
    {
        MoveGuard();
    }

    public void GuardPosition()
    {
        guardTimer -= Time.deltaTime;
    }

    public void MoveGuard()
    {
        //nma.updatePosition = false;
        //nma.updateRotation = false;

        //Quaternion rot = Quaternion.LookRotation(nma.destination);
        //model.transform.rotation = Quaternion.Lerp(model.transform.rotation,rot,.4f);
        //ctrl.Move(nma.destination.normalized * gVars.moveSpeed * Time.deltaTime*10);
        
            scoutDist = Vector3.Distance(transform.position,nma.destination);
        
        //Quaternion rot = Quaternion.LookRotation(-nma.destination);
        //model.transform.rotation = Quaternion.Lerp(model.transform.rotation,rot,.4f);

        //Debug.Log($"Setting scoutDist to {scoutDist}");

    }

    public void SetState(GUARD_STATE s)
    {
        //Debug.Log($"Setting guard's state to {s}");
        gVars.state = s;
    }

}
