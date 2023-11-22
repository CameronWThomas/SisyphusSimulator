using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Deer
{
    public class DeerWaypoint : MonoBehaviour
    {
        public float wanderRadius = 10f;
        public enum WaypointType
        {
            General,
            Directional,
            WallLean
        }

        public WaypointType waypointType;
        public Vector3 lookDir;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(transform.position, 0.5f);

            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, wanderRadius);

            Gizmos.DrawLine(transform.position, transform.position + lookDir.normalized * 3);
        }
    }
}
