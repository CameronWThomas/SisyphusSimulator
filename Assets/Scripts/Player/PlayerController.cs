using Assets.Scripts;
using System;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float ConstantForceModifier = 750f;

    public float MoveForceMultiplier = 50f;
    public float RotationSpeed = 5f;
    public float MaxSpeed = 6f;

    public float BoulderForceModifier = 5f;

    public float MaxCorrectiveVelocity = .45f;
    public float OvercorrectionCounterStrength = 10f;

    public float MinResistiveForceApproachSpeed = .1f;
    public float ResistiveForceModifer = 20f;

    //TODO probably don't want to do this, but for now...
    public GameObject LeftHand;
    public GameObject RightHand;

    private Vector3 m_MoveDir;

    private Rigidbody m_Rb;

    private GameObject m_Boulder;
    private Rigidbody m_BoulderRb;

    private BoulderDetector m_BoulderDetector;

    private CameraReference cameraReference;
    private Quaternion planarRotation2 => cameraReference.PlanarRotation2;

    private float m_Height => GetComponent<CapsuleCollider>().height;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_Rb = GetComponent<Rigidbody>();
        cameraReference = FindObjectOfType<CameraReference>();

        m_Boulder = GameObject.FindGameObjectsWithTag("Boulder").First();
        m_BoulderRb = m_Boulder.GetComponent<Rigidbody>();
        m_BoulderDetector = GetComponent<BoulderDetector>();

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

        var leftHandPress = Input.GetKey(KeyCode.Q);
        var rightHandPress = Input.GetKey(KeyCode.E);
        LeftHand.SetActive(leftHandPress);
        RightHand.SetActive(rightHandPress);

        UpdateBoulderMovementInfo(leftHandPress, rightHandPress);
        if (m_BoulderMovementInfo.IsPushing)
        {
            BoulderCorrection();
            BoulderResistance();
        }

        if (m_MoveDir != Vector3.zero)
        {
            Movement();
            Rotation();
        }        
    }

    private void BoulderCorrection()
    {
        var movingAwayFromCenter = m_BoulderMovementInfo.CorrectionVelocity * m_BoulderMovementInfo.CorrectionModifier < 0f;
        if (!(movingAwayFromCenter || m_BoulderMovementInfo.CorrectionVelocity < MaxCorrectiveVelocity))
        {
            return;
        }

        var correctionModifier = movingAwayFromCenter ? OvercorrectionCounterStrength : 1f;

        var correctionForce = correctionModifier * ConstantForceModifier * m_BoulderMovementInfo.CorrectionModifier * Time.fixedDeltaTime * transform.right;
        m_BoulderRb.AddForce(correctionForce, ForceMode.Impulse);
    }

    private void BoulderResistance()
    {
        Debug.Log($"Boulder vel={m_BoulderMovementInfo.Resistance.magnitude}");
        if (m_BoulderMovementInfo.Resistance.magnitude < MinResistiveForceApproachSpeed)
        {
            return;
        }

        m_BoulderRb.AddForce(ResistiveForceModifer * m_BoulderMovementInfo.Resistance, ForceMode.Impulse);
    }

    private BoulderMovementInfo m_BoulderMovementInfo = new();
    private void UpdateBoulderMovementInfo(bool leftHandPress, bool rightHandPress)
    {
        m_BoulderMovementInfo.IsPushing = m_BoulderDetector.IsInRange
            && ((m_BoulderDetector.BoulderOnLeft && leftHandPress) || (!m_BoulderDetector.BoulderOnLeft && rightHandPress));

        if (!m_BoulderMovementInfo.IsPushing)
        {
            return;
        }

        m_BoulderMovementInfo.BoulderOnLeft = m_BoulderDetector.BoulderOnLeft;

        var sign = m_BoulderDetector.BoulderOnLeft ? 1 : -1;
        m_BoulderMovementInfo.CorrectionModifier = Mathf.Pow(m_BoulderDetector.ContactAnglePercent, 1.5f) * sign;
        m_BoulderMovementInfo.LeftHand = leftHandPress;
        m_BoulderMovementInfo.RightHand = rightHandPress;

        var localBoulderVelocity = transform.InverseTransformDirection(m_BoulderRb.velocity);
        m_BoulderMovementInfo.CorrectionVelocity = localBoulderVelocity.x;
        m_BoulderMovementInfo.Resistance = -1 * m_BoulderDetector.ApproachingVelocity;
    }

    private class BoulderMovementInfo
    {
        public bool IsPushing;
        public float CorrectionModifier;
        public bool BoulderOnLeft;
        public bool LeftHand;
        public bool RightHand;

        public float CorrectionVelocity;
        public Vector3 Resistance;
    }

    private void Movement()
    {
        var moveDir = m_MoveDir;
        if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, m_Height)) //TODO check for ground tag?
        {
            var angle = -Mathf.Abs(90f - Vector3.Angle(transform.forward, hitInfo.normal));
            moveDir = Quaternion.Euler(angle, 0f, 0f) * moveDir;
        }

        var moveForce = ConstantForceModifier * MoveForceMultiplier * Time.fixedDeltaTime * moveDir;

        if (m_BoulderMovementInfo.IsPushing)
        {
            m_BoulderRb.AddForce(BoulderForceModifier * moveForce, ForceMode.Force);
        }
        m_Rb.AddForce(moveForce, ForceMode.Force);
        if (m_Rb.velocity.magnitude > MaxSpeed)
        {
            m_Rb.velocity = m_Rb.velocity.normalized * MaxSpeed;
        }
    }

    private void Rotation()
    {
        var lookRotation = Quaternion.LookRotation(m_MoveDir);
        var targetRotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.fixedDeltaTime * RotationSpeed * 180);
        transform.rotation = targetRotation;
    }
}
