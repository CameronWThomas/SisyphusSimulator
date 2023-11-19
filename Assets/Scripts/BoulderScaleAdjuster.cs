using JetBrains.Annotations;
using System;
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
    //[SerializeField]
    //private float dragScale = 1f;
    public float scaleFactor = 0.03f;
    public float massScaleFactor = 0.1f;

    public float dragScaleFactor = 0.01f;
    public float initialDragCoeficient = 0.1f;
    public float dragCoeficient = 1f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartingY = transform.position.y;
        StartingScale = transform.localScale;
        StartingMass = rb.mass;
        StartingDrag = rb.drag;
        rb.drag = 0;
        rb.angularDrag = 0;
        dragCoeficient = initialDragCoeficient;
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

            dragCoeficient= ((transform.position.y - StartingY) * dragScaleFactor) + initialDragCoeficient;
            //dragScale = ((transform.position.y - StartingY) * dragScaleFactor) + 1;
            //rb.drag = StartingDrag * dragScale;
            //rb.angularDrag = StartingDrag * dragScale;
        }

        //ApplyCustomDrag();
    }

    //void ApplyCustomDrag()
    //{
    //    // no y, so we have gravity
    //    Vector3 dragForce =  new Vector3(
    //        rb.velocity.x * dragCoeficient * -1,
    //        0,
    //        rb.velocity.z * dragCoeficient * -1);

    //    rb.AddForce(dragForce);
    //}
}
