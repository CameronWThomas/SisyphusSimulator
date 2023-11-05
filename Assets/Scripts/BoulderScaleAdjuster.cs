using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderScaleAdjuster : MonoBehaviour
{
    Rigidbody rb;
    float StartingY;
    Vector3 StartingScale;
    float StartingMass;
    float StartingDrag;
    [SerializeField]
    public float scaleSize = 1f;
    [SerializeField]
    private float massScale = 1f;
    [SerializeField]
    private float dragScale = 1f;
    public float scaleFactor = 0.03f;
    public float massScaleFactor = 0.1f;
    public float dragScaleFactor = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartingY = transform.position.y;
        StartingScale = transform.localScale;
        StartingMass = rb.mass;
        StartingDrag = rb.drag; 

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y > StartingY)
        {
            //at minimum. Scale is 1... right?
            scaleSize = ((transform.position.y - StartingY) * scaleFactor) + 1;

            transform.localScale = StartingScale * scaleSize;

            massScale = ((transform.position.y - StartingY) * massScaleFactor) + 1;
            rb.mass = StartingMass * massScale;

            dragScale = ((transform.position.y - StartingY) * dragScaleFactor) + 1;
            rb.drag = StartingDrag * dragScale;
        }
    }
}
