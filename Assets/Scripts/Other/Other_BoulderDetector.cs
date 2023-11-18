using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Other_BoulderDetector : MonoBehaviour
{
    [Range(0f, 180f)]
    public float DetectionAngle = 90f;
    [Range(0f, 10f)]
    public float DetectionRadius = 1.5f;

    public bool IsPushing { get; private set; }
    public bool BoulderOnLeft { get; private set; }
    public bool LeftHand { get; private set; }
    public bool RightHand { get; private set; }
    public float CorrectionModifier { get; private set; }
    public float CorrectionVelocity { get; private set; }
    public Vector3 Resistance { get; private set; }

    private GameObject boulder;
    private BoulderLocationInfo lastBli;
    private float Height => GetComponent<CapsuleCollider>().height * 1.2f;

    void Start()
    {
        boulder = GameObject.FindGameObjectsWithTag("Boulder").First();
    }

    void FixedUpdate()
    {
        lastBli = GetBoulderLocationInfo();

        //TODO I know this is wrong
        var leftHand = Input.GetKey(KeyCode.Q);
        var rightHand = Input.GetKey(KeyCode.E);
        UpdateProperties(lastBli, leftHand, rightHand);
    }

    private void UpdateProperties(BoulderLocationInfo bli, bool leftHandPress, bool rightHandPress)
    {
        if (!bli.isInRange)
        {
            IsPushing = false;
        }

        BoulderOnLeft = bli.hitAngle < 0;
        IsPushing = bli.isInRange && ((BoulderOnLeft && leftHandPress) || (!BoulderOnLeft && rightHandPress));
        if (!IsPushing)
        {
            return;
        }

        var sign = BoulderOnLeft ? 1 : -1;
        var contactAnglePercent = Mathf.Lerp(0f, 1f, Mathf.Abs(bli.hitAngle) / (DetectionAngle / 2));
        CorrectionModifier = Mathf.Pow(contactAnglePercent, 1.5f) * sign;
        LeftHand = leftHandPress;
        RightHand = rightHandPress;

        var boulderRb = boulder.GetComponent<Rigidbody>();
        var localBoulderVelocity = transform.InverseTransformDirection(boulderRb.velocity);
        CorrectionVelocity = localBoulderVelocity.x;

        var approachSpeed = Vector3.Dot(-bli.toBoulderDirection, boulder.GetComponent<Rigidbody>().velocity);
        var approachingVelocity = approachSpeed <= 0 ? Vector3.zero : -1 * approachSpeed * bli.toBoulderDirection;
        Resistance = -1 * approachingVelocity;        
    }


    private BoulderLocationInfo GetBoulderLocationInfo()
    {
        var boulderLocationInfo = new BoulderLocationInfo() { isInRange = false };

        var boulderDirection = (boulder.transform.position - transform.position).normalized;

        if (!Physics.Raycast(transform.position, boulderDirection, out var hitInfo, DetectionRadius) ||
            hitInfo.transform.gameObject != boulder)
        {
            return boulderLocationInfo;
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
                secondHitInfo.transform.gameObject != boulder)
            {
                return boulderLocationInfo;
            }

            var secondHitDirection = (secondHitInfo.point - transform.position).normalized;
            hitAngle = Vector3.SignedAngle(transform.forward, secondHitDirection, transform.up);
        }


        boulderLocationInfo.hitAngle = hitAngle;
        boulderLocationInfo.toBoulderDirection = boulderDirection;
        return boulderLocationInfo;
    }

    private void OnDrawGizmos()
    {
        var colorAlpha = .25f;

        Handles.color = new Color(0, 0, 1, colorAlpha);
        if (lastBli.isInRange)
        {
            colorAlpha = Mathf.Abs(CorrectionModifier);
            Handles.color = BoulderOnLeft
                ? new Color(1, 0, 0, colorAlpha)
                : new Color(1, .5f, 0, colorAlpha);
        }

        var forward = transform.forward;
        var up = transform.up;
        var position = transform.position + Height / 2 * transform.up;
        if (Physics.Raycast(position, Vector3.down, out var hitInfo, Height)) //TODO check for ground tag?
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

        if (!boulder.IsUnityNull())
        {
            Handles.DrawLine(boulder.transform.position, boulder.transform.position + -Resistance, 4f);
        }
    }

    private struct BoulderLocationInfo
    {
        public bool isInRange;
        public float hitAngle;
        public Vector3 toBoulderDirection;
    }
}
