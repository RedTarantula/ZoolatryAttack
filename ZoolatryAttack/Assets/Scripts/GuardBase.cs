using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    [Header("Move Values")]
    public float speed;
    public int magazineBullets;
    public int magazineCapacity;
    public int ammoCarrying;
    public float health;

    [Header("Other Values")]
    public bool isGrounded;
    float gravity = -9.81f;
    Vector3 velocity;
    Vector3 movePos;
    float fallJumpMultiplier = 2.5f;
    float groundDistance = 0.4f;

    bool reloading=false;
    float shootCooldown = 0f;
    float reloadTimer = 0f;

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
