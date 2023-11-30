using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class GravityCatcher : MonoBehaviour
    {
        public float catchRadius = 10;
        public float maxForce = 20;

        private SphereCollider col;

        private List<Rigidbody> rbList = new List<Rigidbody>();

        private void Start()
        {
            col = GetComponent<SphereCollider>();
            col.radius = catchRadius;
        }

        private void Update()
        {

            if(rbList.Count > 0)
            {
                foreach(Rigidbody rb in rbList)
                {
                    Sisyphus sisy = rb.GetComponent<Sisyphus>();
                    if (sisy == null)
                    {
                        PullRigidbody(rb);
                    }
                }
            }
        }

        private void PullRigidbody(Rigidbody rb)
        {
            if (rb != null)
            {
                Vector3 towardsMe = transform.position - rb.transform.position;
                float distance = Vector3.Distance(transform.position, rb.transform.position);
                towardsMe = towardsMe.normalized;
                Vector3 interpretedForce = towardsMe * (maxForce / distance);
                rb.AddForce(interpretedForce, ForceMode.Force);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            
            if(rb != null)
            {
                rbList.Add(rb);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rbList.Remove(rb);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, catchRadius);
        }
    }
}
