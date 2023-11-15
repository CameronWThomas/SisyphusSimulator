using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BoulderDetector : MonoBehaviour
{
    [Range(0f, 180f)]
    public float DetectionAngle = 90f;
    public float DetectionRadius = 1.5f;

    public bool IsInRange { get; private set; }
    public bool BoulderOnLeft { get; private set; }
    public float ContactAnglePercent { get; private set; }
    public Vector3 ApproachingVelocity { get; private set; }

    private GameObject m_Boulder;
    private float m_Height => GetComponent<CapsuleCollider>().height;

    // Start is called before the first frame update
    void Start()
    {
        m_Boulder = GameObject.FindGameObjectsWithTag("Boulder").First();
    }

    // Update is called once per frame
    void Update()
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
        if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, m_Height)) //TODO check for ground tag?
        {
            up = hitInfo.normal;
            var angle = -Mathf.Abs(90f - Vector3.Angle(forward, up));
            forward = Quaternion.Euler(angle, 0f, 0f) * forward;
        }

        var detectionAngleHalf = DetectionAngle / 2f;
        Handles.DrawSolidArc(transform.position, up, forward, detectionAngleHalf, DetectionRadius);
        Handles.DrawSolidArc(transform.position, up, forward, -detectionAngleHalf, DetectionRadius);
        Handles.DrawSolidArc(transform.position, transform.right, forward, -detectionAngleHalf, DetectionRadius);
        Handles.DrawSolidArc(transform.position, transform.right, forward, detectionAngleHalf, DetectionRadius);

        if (!m_Boulder.IsUnityNull())
        {
            Handles.DrawLine(m_Boulder.transform.position, m_Boulder.transform.position + ApproachingVelocity, 4f);
        }
    }
}
