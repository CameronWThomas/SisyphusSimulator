using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawner : MonoBehaviour
{

    public Vector2 size = new Vector2(-200, 200);
    public float interval = 5f;
    public float snowLine = 200f;
    public GameObject GrassPrefab;
    public LayerMask spawnLayermask;

    private void OnEnable()
    {
        //itterate through grid and spawn
        for(float i = size.x; i < size.y; i += interval)
        {
            for (float j = size.x; j < size.y; j += interval)
            {
                Vector3 pos = new Vector3(
                    transform.position.x + i, 
                    transform.position.y + snowLine, 
                    transform.position.z + j);

                //raycast from up high
                RaycastHit hit;
                Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, spawnLayermask);
                if (hit.collider != null)
                {
                    float y = hit.point.y;
                    pos.y = y;
                    Instantiate(GrassPrefab, pos, Quaternion.identity);
                }
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

    }
}
