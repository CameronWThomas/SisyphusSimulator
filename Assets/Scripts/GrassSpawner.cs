using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    public float xSize = 400;
    public float zSize = 400;
    public float interval = 5f;
    public float perlinIntensity = 30;
    public float snowLine = 200f;
    public GameObject SpawnPrefab;
    public Transform SpawnParent;
    public LayerMask spawnLayermask;


    private void OnEnable()
    {
        ////itterate through grid and spawn
        //for(float i = size.x; i < size.y; i += interval)
        //{
        //    for (float j = size.x; j < size.y; j += interval)
        //    {
        //        Vector3 pos = new Vector3(
        //            transform.position.x + i, 
        //            transform.position.y + snowLine, 
        //            transform.position.z + j);

        //        //raycast from up high
        //        RaycastHit hit;
        //        Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, spawnLayermask);
        //        if (hit.collider != null)
        //        {
        //            float y = hit.point.y;
        //            pos.y = y;
        //            Instantiate(GrassPrefab, pos, Quaternion.identity);
        //        }
        //    }
        //}

    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public void GridSpawn()
    {

        float PerlinNoise = Mathf.PerlinNoise(xSize, zSize);

        Debug.Log("Starting Spawn");
        for (float i = 0 - (xSize / 2); i < xSize / 2; i += interval)
        {
            for (float j = 0 - (zSize / 2); j < zSize / 2; j += interval)
            {
                float perlinNoiseOffset = Mathf.PerlinNoise(transform.position.x + i, transform.position.z + j);
                perlinNoiseOffset *= perlinIntensity;
                Vector3 pos = new Vector3(
                    transform.position.x + i + perlinNoiseOffset,
                    transform.position.y + snowLine,
                    transform.position.z + j + perlinNoiseOffset);

                //raycast from up high
                RaycastHit hit;
                Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, spawnLayermask);
                Debug.DrawRay(pos, Vector3.down, Color.green, 10f * Time.deltaTime);
                if (hit.collider != null)
                {
                    float y = hit.point.y;
                    pos.y = y;
                    Instantiate(SpawnPrefab, pos, Quaternion.identity, SpawnParent);
                }
            }
        }
        Debug.Log("Done!");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(xSize, snowLine, zSize));

    }
}
