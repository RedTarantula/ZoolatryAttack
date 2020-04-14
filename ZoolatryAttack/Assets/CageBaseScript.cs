using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CageBaseScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool beingHeld = false;
    
    Vector3 velocity;
    public Transform groundCheck;
    public LayerMask groundMask;
    float fallJumpMultiplier = 2.5f;
    float groundDistance = 0.05f;
    public bool isGrounded;


    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(this.beingHeld);
        }
        else
        {
            this.beingHeld = (bool)stream.ReceiveNext();
        }
    }
    public void ChangeBeingHeld(bool b)
    {
        beingHeld = b;
    }

    [PunRPC]
    public void Deliver()
    {
            Destroy(gameObject);
    }

    private void Update()
    {
        if (beingHeld)
            return;

        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);

        if (!isGrounded)
        {
            velocity += Vector3.up * Physics.gravity.y * (fallJumpMultiplier - 1) * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        transform.Translate(velocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(photonView.ViewID);
        if (!this.photonView.IsMine)
            return;

        if (!beingHeld && other.gameObject.CompareTag("DeliveryPoint"))
        {
            photonView.RPC("Deliver",RpcTarget.All);
        }
    }
}
