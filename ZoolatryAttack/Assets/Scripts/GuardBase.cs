using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Zoolatry;

public class GuardBase : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public GameObject model;
    public Transform shootingPoint;
    public GameObject projectilePrefab;
    public ZoolatryManager zm;
    CharacterController ctrl;

    [Header("Guard")]
    public GUARD_CHARACTER guard;
    [SerializeField] public GuardVariables gVars;

    [Header("Movement")]
    Vector3 velocity;
    Vector3 movePos;
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
    }
    private void Start()
    {
        gVars.state = GUARD_STATE.Scouting;
    }
    private void Update()
    {
        EnemyAI();
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
                if (scoutDist <= 0)
                {
                    gVars.state = GUARD_STATE.Guarding;
                    guardTimer = gVars.guardDuration;
                }
                FindClosestPlayer();
                return;
                //=------------------=

            case GUARD_STATE.Following:
                if (gVars.target == null) return;
                if (gVars.TgtDistance(transform) <= gVars.tgtDistShoot)
                {
                    gVars.state = GUARD_STATE.Shooting;
                }
                break;
                //=------------------=

            case GUARD_STATE.Guarding:
                if (guardTimer <= 0)
                {
                    gVars.state = GUARD_STATE.Scouting;
                    scoutDist = gVars.scoutDist;
                }
                FindClosestPlayer();
                return;
                //=------------------=

            case GUARD_STATE.Shooting:
                if (gVars.target == null) return;

                if (gVars.TgtDistance(transform) >= gVars.tgtDistFollow)
                {
                    gVars.state = GUARD_STATE.Following;
                }
                break;
                //=------------------=

            case GUARD_STATE.Capturing:
                break;
                //=------------------=

            default:
                break;
        }

        
     
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
            gVars.state = GUARD_STATE.Following;

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

    public void GetScoutDirection()
    {
        movePos = transform.right * Random.Range(-1,2) + transform.forward * Random.Range(-1,2);
        movePos = transform.TransformDirection(movePos);
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
        Quaternion rot = Quaternion.LookRotation(movePos);
        model.transform.rotation = Quaternion.Lerp(model.transform.rotation,rot,.4f);
        ctrl.Move(movePos.normalized * gVars.moveSpeed);
        scoutDist -= Time.deltaTime * gVars.moveSpeed;
    }

}
