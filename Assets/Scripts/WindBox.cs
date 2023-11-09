using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WindBox : MonoBehaviour
    {

        private List<Rigidbody> rbList = new List<Rigidbody>();
        public Vector3 forceVector;
        public BoxCollider boxCollider;

        private void OnEnable()
        {
            boxCollider = GetComponent<BoxCollider>();
        }
        private void Update()
        {

            if (rbList.Count > 0)
            {
                foreach (Rigidbody rb in rbList)
                {
                    rb.AddForce(forceVector);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
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
            if (boxCollider != null)
            {
                Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + forceVector);
        }
    }
}
