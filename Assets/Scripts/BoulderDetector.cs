using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoulderDetector : MonoBehaviour
    {
        [Range(0f, 180f)]
        public float detectionAngle = 90f;
        [Range(0f, 10f)]
        public float startingDetectionRadius = 1.5f;
        [Range(0f, 1f)]
        public float scalingEffect = .5f;
        [Range(0f, 1f)]
        public float handOverlap = .1f;

        public float DetectionRadius => startingDetectionRadius * Mathf.Max(scaleSize * scalingEffect, 1f);

        public bool IsPushing { get; private set; }
        public bool BoulderOnLeft { get; private set; }
        public bool LeftHand { get; set; }
        public bool RightHand { get; set; }
        public float CorrectionModifier { get; private set; }
        public float CorrectionVelocity { get; private set; }
        public Vector3 Resistance { get; private set; }

        private GameObject boulder;
        private BoulderLocationInfo lastBli;

        private float Height => GetComponent<CapsuleCollider>().height;
        private Vector3 Position => transform.position + Height / 2 * transform.up;
        private float HalfDetectionAngle => detectionAngle / 2f;
        private float scaleSize => boulder.GetComponent<BoulderScaleAdjuster>().scaleSize;

        void Start()
        {

            boulder = FindObjectOfType<Boulder>().gameObject;
        }

        void FixedUpdate()
        {
            lastBli = GetBoulderLocationInfo();

            //TODO I know this is wrong. Maybe use mouse clicks?
            //LeftHand = Input.GetKey(KeyCode.Q);
            //RightHand = Input.GetKey(KeyCode.E);
            UpdateProperties(lastBli);
        }

        private void UpdateProperties(BoulderLocationInfo bli)
        {
            if (!bli.isInRange)
            {
                IsPushing = false;
            }

            // Make sure a hand is pressed that will work
            BoulderOnLeft = bli.hitAngle < 0;
            var boulderWithinHandRange = BoulderOnLeft
                ? LeftHand || (Mathf.Abs(bli.hitAngle) < HalfDetectionAngle * handOverlap && RightHand)
                : RightHand || (Mathf.Abs(bli.hitAngle) < HalfDetectionAngle * handOverlap && LeftHand);
            IsPushing = bli.isInRange && boulderWithinHandRange;
            if (!IsPushing)
            {
                return;
            }

            // Figure out the correction strength
            var contactAnglePercent = Mathf.Lerp(0f, 1f, Mathf.Abs(bli.hitAngle) / HalfDetectionAngle);
            CorrectionModifier = Mathf.Pow(contactAnglePercent, 1.5f);

            // Figure out the currently velocity in the correction direction
            var boulderRb = boulder.GetComponent<Rigidbody>();
            var localBoulderVelocity = transform.InverseTransformDirection(boulderRb.velocity);
            CorrectionVelocity = localBoulderVelocity.x;

            // Figure out how strong of resistance to an oncoming boulder
            var approachSpeed = Vector3.Dot(-bli.toBoulderDirection, boulder.GetComponent<Rigidbody>().velocity);
            var approachingVelocity = approachSpeed <= 0 ? Vector3.zero : -1 * approachSpeed * bli.toBoulderDirection;
            Resistance = -1 * approachingVelocity;
        }


        private BoulderLocationInfo GetBoulderLocationInfo()
        {
            var boulderLocationInfo = new BoulderLocationInfo() { isInRange = false };

            var boulderDirection = (boulder.transform.position - Position).normalized;
            if (!Physics.Raycast(Position, boulderDirection, out var hitInfo, DetectionRadius) ||
                hitInfo.transform.gameObject != boulder)
            {
                // Boulder not close enough
                return boulderLocationInfo;
            }

            // Check if boulder is directly in view
            var hitDirection = (hitInfo.point - Position).normalized;
            var hitAngle = Vector3.SignedAngle(transform.forward, hitDirection, transform.up);
            if (Mathf.Abs(hitAngle) > HalfDetectionAngle)
            {
                // Boulder out of view, but edge may still be, check
                var rotation = hitAngle > 0
                    ? Quaternion.Euler(0, hitAngle - HalfDetectionAngle, 0)
                    : Quaternion.Euler(0, hitAngle + HalfDetectionAngle, 0);
                var raycastDirection = rotation * hitDirection;

                if (!Physics.Raycast(Position, raycastDirection, out var secondHitInfo, DetectionRadius) ||
                    secondHitInfo.transform.gameObject != boulder)
                {
                    return boulderLocationInfo;
                }

                var secondHitDirection = (secondHitInfo.point - Position).normalized;
                hitAngle = Vector3.SignedAngle(transform.forward, secondHitDirection, transform.up);
            }

            boulderLocationInfo.isInRange = true;
            boulderLocationInfo.hitAngle = hitAngle;
            boulderLocationInfo.toBoulderDirection = boulderDirection;
            return boulderLocationInfo;
        }

        private void OnDrawGizmos()
        {
            var colorAlpha = .25f;

            //Handles.color = new Color(0, 0, 1, colorAlpha);
            if (lastBli.isInRange)
            {
                colorAlpha = Mathf.Abs(lastBli.hitAngle) / HalfDetectionAngle;
                //Handles.color = BoulderOnLeft
                //    ? new Color(1, 0, 0, colorAlpha)
                //    : new Color(1, .5f, 0, colorAlpha);
            }

            var forward = transform.forward;
            var up = transform.up;
            if (Physics.Raycast(Position, Vector3.down, out var hitInfo, Height * 1.2f)) //TODO check for ground tag?
            {
                up = hitInfo.normal;
                var angle = -Mathf.Abs(90f - Vector3.Angle(forward, up));
                forward = Quaternion.Euler(angle, 0f, 0f) * forward;
            }

            var detectionAngleHalf = detectionAngle / 2f;
            var radius = startingDetectionRadius;
            if (!boulder.IsUnityNull())
            {
                radius = DetectionRadius;
            }
            //Handles.DrawSolidArc(Position, up, forward, detectionAngleHalf, radius);
            //Handles.DrawSolidArc(Position, up, forward, -detectionAngleHalf, radius);
            //Handles.DrawSolidArc(Position, transform.right, forward, -detectionAngleHalf, radius);
            //Handles.DrawSolidArc(Position, transform.right, forward, detectionAngleHalf, radius);

            if (!boulder.IsUnityNull())
            {
                //Handles.DrawLine(boulder.transform.position, boulder.transform.position + -Resistance, 4f);
            }
        }

        private struct BoulderLocationInfo
        {
            public bool isInRange;
            public float hitAngle;
            public Vector3 toBoulderDirection;
        }
    }
}