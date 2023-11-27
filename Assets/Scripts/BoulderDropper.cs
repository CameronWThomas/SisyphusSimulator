using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderDropper : MonoBehaviour
{
    public GameObject boulderGroup;

    private Rigidbody[] boulders;
    private bool triggered = false;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        boulders = boulderGroup.GetComponentsInChildren<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.transform.CompareTag("Player"))
        {
            return;
        }

        //TODO randomly drop them?

        foreach (var boulder in boulders)
        {
            boulder.isKinematic = false;
        }

        triggered = true;
    }
}
