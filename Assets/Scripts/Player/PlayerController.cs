using Assets.Scripts;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float ForceMultiplier = 10f;
    public float RotationSpeed = 2f;
    public float MaxSpeed = 5.0f;
    public float m_RayDistance = 2f;
    public float m_PushActivationAngle = 45f;
    public float m_HoldBackAngle = 45f;
    public GameObject Boulder;

    //TODO probably don't want to do this, but for now...
    public GameObject LeftHand;
    public GameObject RightHand;

    private Rigidbody rb;
    private CameraReference cameraReference;
    private Vector3 m_MoveDir;
    private bool m_PushingBoulder = false;

    private Quaternion planarRotation2 => cameraReference.PlanarRotation2;

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        cameraReference = FindObjectOfType<CameraReference>();

        LeftHand.SetActive(false);
        RightHand.SetActive(false);
    }

    // Update is called once per frames
    void FixedUpdate()
    {
        var inputDir = GetComponent<PlayerInputBus>().moveInput;
        var moveInput = new Vector3(inputDir.x, 0, inputDir.y).normalized;
        m_MoveDir = moveInput == Vector3.zero
            ? Vector3.zero 
            : (planarRotation2 * moveInput).normalized;

        BoulderPushing();

        MovementAndRotation();
    }

    private void BoulderPushing()
    {
        // TODO Power of the force should take keys into account
        //  - both hands add together for forward pushing power but decrease corrective power overall
        //  - single hand should have normal pushing power but greater corrective power in that direction
        //  - getting gassed based on how many hands in use and force boulder is applying

        // TODO have state machine for pushign mode like what cam did

        // TODO boulder should be hard to move without using hands. impossible?

        // TODO cleanup and probably remove that cross product part and fix.

        var rightHandPress = Input.GetKey(KeyCode.E);
        var leftHandPress = Input.GetKey(KeyCode.Q);
        RightHand.SetActive(rightHandPress); //TODO probably inefficient
        LeftHand.SetActive(leftHandPress);

        var boulderDirection = (Boulder.transform.position - transform.position).normalized;
        var boulderAngle = Vector3.Angle(transform.forward, boulderDirection);
        var inBoulderPushDirection = boulderAngle < m_PushActivationAngle;
        var boulderOnLeftSide = Vector3.Angle(Vector3.Cross(transform.forward, boulderDirection), transform.up) > 90;
        Debug.Log($"Boulder on left side? {boulderOnLeftSide}");
        //Debug.Log($"Boulder on left side? {boulderOnLeftSide} boulderAngle={boulderAngle}");
        
        if (!inBoulderPushDirection ||
            (!leftHandPress && !rightHandPress) ||
            !(boulderOnLeftSide == leftHandPress || !boulderOnLeftSide == rightHandPress) ||
            !Physics.Raycast(transform.position, boulderDirection, out var hitInfo, m_RayDistance) ||
            !hitInfo.transform.gameObject == Boulder)
        {
            m_PushingBoulder = false;
            Debug.Log($"Pushing boulder? {false}");

            return;
        }
        Debug.Log($"Pushing boulder? {true}");


        m_PushingBoulder = leftHandPress || rightHandPress;

        var correctionDirection = boulderOnLeftSide ? transform.right : -transform.right;
        //TODO make it a slerp?
        var correctionForce = 100 * ForceMultiplier * Time.fixedDeltaTime * correctionDirection * (boulderAngle / m_PushActivationAngle);

        var boulderRb = Boulder.GetComponent<Rigidbody>();
        boulderRb.AddForce(correctionForce, ForceMode.Force);

        var boulderVelocityDirection = boulderRb.velocity.normalized;
        var boulderToPlayer = -boulderDirection;
        var boulderApproaching = Vector3.Angle(boulderToPlayer, boulderVelocityDirection) < m_HoldBackAngle;
        var boulderApproachingLeft = Vector3.Angle(Vector3.Cross(boulderToPlayer, boulderVelocityDirection), transform.up) > 90;

        if (boulderApproaching &&
            (boulderApproachingLeft == leftHandPress || !boulderApproachingLeft == rightHandPress))
        {
            var resistanceForce = 100 * ForceMultiplier * Time.fixedDeltaTime * boulderDirection * boulderRb.velocity.magnitude;
            boulderRb.AddForce(resistanceForce, ForceMode.Force);
        }

        //TODO nudge the boulder towards in front of you

    }

    private void MovementAndRotation()
    {
        if (m_MoveDir == Vector3.zero)
        {
            return;
        }

        var rigidBody = m_PushingBoulder ? Boulder.GetComponent<Rigidbody>() : rb;        

        // Move
        var moveForce = 750 * ForceMultiplier * Time.fixedDeltaTime * m_MoveDir;
        rigidBody.AddForce(moveForce, ForceMode.Force);
        if (!m_PushingBoulder && rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }
        else if (m_PushingBoulder)
        {
            rb.velocity = rigidBody.velocity;
        }

        // Rotate
        var lookRotation = Quaternion.LookRotation(m_MoveDir);
        var targetRotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.fixedDeltaTime * RotationSpeed * 180);
        transform.rotation = targetRotation;
    }
}
