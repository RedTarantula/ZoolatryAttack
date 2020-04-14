using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform pos;
    public Vector3 offset;
    // Update is called once per frame
    void Update()
    {
        if (pos != null)
        {
            transform.position = new Vector3(pos.position.x +offset.x,transform.position.y,pos.position.z +offset.z);
        }
    }
}
