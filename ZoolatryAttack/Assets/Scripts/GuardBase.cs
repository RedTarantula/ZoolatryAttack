using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Zoolatry;

public abstract class GuardBase : MonoBehaviourPunCallbacks, IPunObservable
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
    public GuardVariables gVars;

    [Header("Movement")]
    Vector3 velocity;
    Vector3 movePos;
    float scoutDist = 0f;
    Quaternion scoutDirection;

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
        StartLocalVariables();
    }

    public abstract void StartLocalVariables();
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
        if(gVars.target == null)
        {
            // Search for target
            return;
        }

        float targetDistance = gVars.TgtDistance(transform);
     
        if(targetDistance > gVars.tgtDistAggro) // When too far from the player, lose it as a target and start scouting
        {
            gVars.target = null;
            gVars.state = GUARD_STATE.Scouting;
            return;
        }

        if(gVars.state == GUARD_STATE.Following) // If is following and get the player within shooting range, stop moving and start shooting
        {
            if(targetDistance <= gVars.tgtDistShoot)
            {
                gVars.state = GUARD_STATE.Shooting;
            }
        }

        if(gVars.state == GUARD_STATE.Shooting) // If is shooting and the player gets out of the range, stop shooting and start following
        {
            if(targetDistance >= gVars.tgtDistFollow)
            {
                gVars.state = GUARD_STATE.Following;
            }
        }

        if (gVars.state == GUARD_STATE.Scouting) // If finishes scouting to direction, change to guarding and set the guard timer
        {
            if (scoutDist <= gVars.scoutDist)
            {
                gVars.state = GUARD_STATE.Guarding;
                guardTimer = gVars.guardDuration;
            }
        }

        if(gVars.state == GUARD_STATE.Guarding) // If the guard timer finishes, change to scouting
        {
            if(guardTimer <= 0)
            {
                gVars.state = GUARD_STATE.Scouting;
            }
        }

        switch (gVars.state)
        {
            case GUARD_STATE.Scouting:
                FindClosestPlayer();
                break;
            case GUARD_STATE.Following:
                break;
            case GUARD_STATE.Guarding:
                FindClosestPlayer();
                break;
            case GUARD_STATE.Shooting:
                break;
            case GUARD_STATE.Capturing:
                break;
            default:
                break;
        }

    }
    public void FindClosestPlayer()
    {
        // If finds a player within range, change to following
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

    public void ScoutToDirection()
    {
       
    }

    public void GuardPosition()
    {

    }
}
