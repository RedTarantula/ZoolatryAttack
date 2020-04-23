using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CageBaseScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool beingHeld = false;
    GroundedObject gobj;
    public Zoolatry.CAGE_TYPE cagetype;

    private void Awake()
    {
            gobj = GetComponent<GroundedObject>();
        gobj.groundMask = LayerMask.GetMask("Ground");
    }

    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if (stream.IsWriting)
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

    private void Update()
    {
        if (!beingHeld)
        {
            gobj.Fall();
        }
    }

    [PunRPC]
    public void Deliver()
    {
        Destroy(gameObject);
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
