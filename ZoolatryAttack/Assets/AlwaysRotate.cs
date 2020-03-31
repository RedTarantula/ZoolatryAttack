using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysRotate : MonoBehaviour
{
    public Vector3 rotateSpeed;

    private void Update()
    {
        transform.Rotate(rotateSpeed*Time.deltaTime);
    }
}
