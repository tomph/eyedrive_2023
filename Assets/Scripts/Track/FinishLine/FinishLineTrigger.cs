using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FinishLineTrigger : MonoBehaviour
{
    public Signal ON_TRIGGERED = new Signal();

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<BaseFollower>() != null)
        {
            ON_TRIGGERED.Dispatch();
        }
    }
}
