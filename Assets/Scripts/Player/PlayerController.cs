using Assets.Scripts;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float ForceMultiplier = 10f;
    public float RotationSpeed = 2f;
    public float MaxSpeed = 5.0f;

    private Rigidbody rb;
    private CameraReference cameraReference;
    private Quaternion planarRotation2 => cameraReference.PlanarRotation2;

    private Vector3 moveDir;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        cameraReference = FindObjectOfType<CameraReference>();
    }

    // Update is called once per frames
    void FixedUpdate()
    {
        var inputDir = GetComponent<PlayerInputBus>().moveInput;
        var moveInput = new Vector3(inputDir.x, 0, inputDir.y).normalized;
        if (moveInput == Vector3.zero)
        {
            return;
        }

        moveDir = planarRotation2 * moveInput;

        Move();
        FaceDirection();
    }

    private void Move()
    {
        var moveForce = 750 * ForceMultiplier * Time.fixedDeltaTime * moveDir;
        rb.AddForce(moveForce, ForceMode.Force);
        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }
    }

    private void FaceDirection()
    {
        var lookRotation = Quaternion.LookRotation(moveDir);
        var targetRotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.fixedDeltaTime * RotationSpeed * 180);
        transform.rotation = targetRotation;
    }
}
