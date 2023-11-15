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
    public float ApproachAnglePercent { get; private set; }

    private GameObject m_Boulder;

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
        ApproachAnglePercent = Mathf.Lerp(0f, 1f, Mathf.Abs(hitAngle) / (DetectionAngle / 2));
    }

    private void OnDrawGizmos()
    {
        var colorAlpha = .25f;
        
        Handles.color = new Color(0, 0, 1, colorAlpha);
        if (IsInRange)
        {
            colorAlpha = ApproachAnglePercent;
            Handles.color = BoulderOnLeft
                ? new Color(1, 0, 0, colorAlpha)
                : new Color(1, .5f, 0, colorAlpha);
        }

        var detectionAngleHalf = DetectionAngle / 2f;
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, detectionAngleHalf, DetectionRadius);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -detectionAngleHalf, DetectionRadius);
    }
}
