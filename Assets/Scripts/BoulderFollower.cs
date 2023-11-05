using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoulderFollower :MonoBehaviour
    {
        Boulder boulder;
        BoulderScaleAdjuster boulderScale;
        [SerializeField]
        public float rotateRadius;
        public float additionalSpacing = 1;

        public float detachDistance;
        public float detachDistanceMultiplier = 3f;
        private void Start()
        {
            boulder = FindObjectOfType<Boulder>();
            boulderScale = boulder.GetComponent<BoulderScaleAdjuster>();
        }

        private void Update()
        {
            rotateRadius = boulderScale.scaleSize + additionalSpacing;
            transform.position = boulder.transform.position;

            DetachDistanceCalc();

        }

        public void DetachDistanceCalc()
        {
            detachDistance = rotateRadius * detachDistanceMultiplier;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rotateRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detachDistance);
        }
    }
}
