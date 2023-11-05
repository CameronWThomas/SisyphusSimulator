using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class BoulderPhysicsMvmntController : MonoBehaviour
{
    public Vector2 inputDir;
    //just for insight
    [SerializeField]
    private Vector3 activeForce;
    public float forceMultiplier = 10f;
    private Rigidbody rb;
    private CameraController camera;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {

        float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
        var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
        var moveDir = camera.PlanarRotation2 * moveInput;

        //TODO: cap speed?
        rb.AddForce(moveDir * forceMultiplier, ForceMode.Acceleration);

        //just for insight
        activeForce = rb.GetAccumulatedForce();

    }
}
