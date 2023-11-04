using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReference : MonoBehaviour
{
    public Quaternion PlanarRotation2 => Quaternion.Euler(0, transform.eulerAngles.y, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
