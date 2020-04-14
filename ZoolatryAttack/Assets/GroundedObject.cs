﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GroundedObject : MonoBehaviourPunCallbacks
{
    public bool isGrounded;
    float gravity = -9.81f;
    Vector3 velocity;
    public Transform groundCheck;
    public LayerMask groundMask;
    float fallJumpMultiplier = 2.5f;
    float groundDistance = 0.05f;
    public bool fall = true;
    private void Update()
    {
        if (!fall)
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

}
