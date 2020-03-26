using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public class PlayerBase : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public GameObject model;
    CharacterController ctrl;

    [Header("Move Values")]
    public float speed = .2f;
    float speedR = 300f;

    [Header("Other Values")]
    public bool isGrounded;
    float gravity = -9.81f;
    Vector3 velocity;
    Vector3 movePos;
    float fallJumpMultiplier = 2.5f;
    float groundDistance = 0.4f;

    float hInput;
    float vInput;

    private PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        ctrl = GetComponent<CharacterController>();
    }
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


        if (movePos.magnitude != 0)
        {
            Quaternion rot = Quaternion.LookRotation(movePos);
            model.transform.rotation = Quaternion.Lerp(model.transform.rotation, rot,.4f);
        }
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
