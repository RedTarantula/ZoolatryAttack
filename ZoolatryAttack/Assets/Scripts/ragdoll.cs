using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdoll : MonoBehaviour
{
    public bool ragdollBool = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetKinematic(ragdollBool);
    }

    void SetKinematic(bool newValue)
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = newValue;
        }
    }
}