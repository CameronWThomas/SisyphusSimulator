using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject boulder;
    public GameObject sisyphus;

    public int spawnLocation;

    public Transform[] spawnLocations;

    private bool updated = false;

    private void LateUpdate()
    {
        if (updated) return;

        var location = spawnLocations[spawnLocation - 1];
        sisyphus.transform.position = location.position;
        boulder.transform.position = location.position + sisyphus.transform.forward * 5f;
        updated = true;
    }
}
