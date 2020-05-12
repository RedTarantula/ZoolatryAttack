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

    [Header("Timers")]
    float shootCooldown = 0f;
    float reloadTimer = 0f;

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
}
