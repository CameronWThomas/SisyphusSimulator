using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderDropper : MonoBehaviour
{
    public GameObject boulderGroup;
    public float timeTillReset = 60f;

    private Rigidbody[] boulders;
    private List<Vector3> originalBoulderPositions;
    private bool triggered = false;
    private float timeOfTrigger = 0f;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        boulders = boulderGroup.GetComponentsInChildren<Rigidbody>();

        originalBoulderPositions = new List<Vector3>();
        foreach (var boulder in boulders)
        {
            originalBoulderPositions.Add(boulder.position);
            boulder.transform.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void Update()
    {
        if (triggered && (Time.time - timeOfTrigger > timeTillReset))
        {
            triggered = false;
            
            for (int i = 0; i < boulders.Length; i++)
            {
                boulders[i].transform.position = originalBoulderPositions[i];
                boulders[i].isKinematic = true;
                boulders[i].transform.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.transform.CompareTag("Player"))
        {
            return;
        }

        foreach (var boulder in boulders)
        {
            boulder.transform.GetComponent<MeshRenderer>().enabled = true;
            boulder.isKinematic = false;
        }

        timeOfTrigger = Time.time;
        triggered = true;
    }
}
