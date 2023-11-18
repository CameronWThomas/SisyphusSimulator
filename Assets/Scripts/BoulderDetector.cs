using Assets.Scripts.BoulderStuff;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoulderDetector : MonoBehaviour
{
    [Range(0f, 180f)]
    public float DetectionAngle = 90f;
    [Range(0f, 10f)]
    public float DetectionRadius = 1.5f;

    public bool IsInRange { get; private set; }
    public bool BoulderOnLeft { get; private set; }
    public float ContactAnglePercent { get; private set; }
    public Vector3 ApproachingVelocity { get; private set; }

    private GameObject m_Boulder;
    private Rigidbody m_BoulderRb => m_Boulder.GetComponent<Rigidbody>();

    private MovementStateController msc;

    private float m_Height => GetComponent<CharacterController>().height;

    // Start is called before the first frame update
    void Start()
    {
        m_Boulder = GameObject.FindGameObjectsWithTag("Boulder").First();
        msc = GetComponent<MovementStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProperties();

        //TODO I know this is wrong
        var leftHand = Input.GetKey(KeyCode.Q);
        var rightHand = Input.GetKey(KeyCode.E);
        UpdateBoulderMovementInfo(leftHand, rightHand);

        if (pushingChanged)
        {
            msc.ChangeState(boulderMovementInfo.IsPushing ? msc.rolling : msc.onFoot);
        }
    }

    private bool pushingChanged = false;

    public BoulderMovementInfo boulderMovementInfo = new();
    private void UpdateBoulderMovementInfo(bool leftHandPress, bool rightHandPress)
    {
        var isPushing = IsInRange
            && ((BoulderOnLeft && leftHandPress) || (!BoulderOnLeft && rightHandPress));

        pushingChanged = isPushing != boulderMovementInfo.IsPushing;
        boulderMovementInfo.IsPushing = isPushing;


        if (!boulderMovementInfo.IsPushing)
        {
            return;
        }

        boulderMovementInfo.BoulderOnLeft = BoulderOnLeft;

        var sign = BoulderOnLeft ? 1 : -1;
        boulderMovementInfo.CorrectionModifier = Mathf.Pow(ContactAnglePercent, 1.5f) * sign;
        boulderMovementInfo.LeftHand = leftHandPress;
        boulderMovementInfo.RightHand = rightHandPress;

        var localBoulderVelocity = transform.InverseTransformDirection(m_BoulderRb.velocity);
        boulderMovementInfo.CorrectionVelocity = localBoulderVelocity.x;
        boulderMovementInfo.Resistance = -1 * ApproachingVelocity;
    }

    public class BoulderMovementInfo
    {
        public bool IsPushing;
        public float CorrectionModifier;
        public bool BoulderOnLeft;
        public bool LeftHand;
        public bool RightHand;

        public float CorrectionVelocity;
        public Vector3 Resistance;
    }

    private void UpdateProperties()
    { 
        var boulderDirection = (m_Boulder.transform.position - transform.position).normalized;

        if (!Physics.Raycast(transform.position, boulderDirection, out var hitInfo, DetectionRadius) ||
            hitInfo.transform.gameObject != m_Boulder)
        {
            IsInRange = false;
            return;
        }

        var hitDirection = (hitInfo.point - transform.position).normalized;
        var hitAngle = Vector3.SignedAngle(transform.forward, hitDirection, transform.up);
        if (Mathf.Abs(hitAngle) > DetectionAngle / 2)
        {
            var rotation = hitAngle > 0
                ? Quaternion.Euler(0, hitAngle - DetectionAngle / 2, 0)
                : Quaternion.Euler(0, hitAngle + DetectionAngle / 2, 0);
            var raycastDirection = rotation * hitDirection;

            if (!Physics.Raycast(transform.position, raycastDirection, out var secondHitInfo, DetectionRadius) ||
                secondHitInfo.transform.gameObject != m_Boulder)
            {
                IsInRange = false;
                return;
            }

            var secondHitDirection = (secondHitInfo.point - transform.position).normalized;
            hitAngle = Vector3.SignedAngle(transform.forward, secondHitDirection, transform.up);
        }

        IsInRange = true;
        BoulderOnLeft = hitAngle < 0;
        ContactAnglePercent = Mathf.Lerp(0f, 1f, Mathf.Abs(hitAngle) / (DetectionAngle / 2));
        var approachSpeed = Vector3.Dot(-boulderDirection, m_Boulder.GetComponent<Rigidbody>().velocity);
        ApproachingVelocity = approachSpeed <= 0 ? Vector3.zero : -1 * approachSpeed * boulderDirection;
    }

    private void OnDrawGizmos()
    {
        var colorAlpha = .25f;

        Handles.color = new Color(0, 0, 1, colorAlpha);
        if (IsInRange)
        {
            colorAlpha = ContactAnglePercent;
            Handles.color = BoulderOnLeft
                ? new Color(1, 0, 0, colorAlpha)
                : new Color(1, .5f, 0, colorAlpha);
        }

        var forward = transform.forward;
        var up = transform.up;
        var position = transform.position + m_Height / 2 * transform.up;
        if (Physics.Raycast(position, Vector3.down, out var hitInfo, m_Height)) //TODO check for ground tag?
        {
            up = hitInfo.normal;
            var angle = -Mathf.Abs(90f - Vector3.Angle(forward, up));
            forward = Quaternion.Euler(angle, 0f, 0f) * forward;
        }

        var detectionAngleHalf = DetectionAngle / 2f;
        Handles.DrawSolidArc(position, up, forward, detectionAngleHalf, DetectionRadius);
        Handles.DrawSolidArc(position, up, forward, -detectionAngleHalf, DetectionRadius);
        Handles.DrawSolidArc(position, transform.right, forward, -detectionAngleHalf, DetectionRadius);
        Handles.DrawSolidArc(position, transform.right, forward, detectionAngleHalf, DetectionRadius);

        if (!m_Boulder.IsUnityNull())
        {
            Handles.DrawLine(m_Boulder.transform.position, m_Boulder.transform.position + ApproachingVelocity, 4f);
        }
    }
}
