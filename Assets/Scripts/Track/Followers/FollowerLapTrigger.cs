using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerLapTrigger : MonoBehaviour
{
    public Signal ON_TRIGGER = new Signal();

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb==null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.mass = 0.0000001f;
            rb.useGravity = false;
            rb.drag = rb.angularDrag = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("FinishLine"))
        {
            ON_TRIGGER.Dispatch();
        }
    }
}
