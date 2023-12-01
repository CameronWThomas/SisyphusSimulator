using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Deer
{
    public class DeerRagdollController :MonoBehaviour
    {
        public bool ragdolling;
        Rigidbody[] bones;
        public Rigidbody root;
        Animator animator;

        private float ragdollStartTime = 0f;

        private void Start()
        {
            bones = GetComponentsInChildren<Rigidbody>();
            animator = GetComponent<Animator>();
            DisableRagdoll();
        }

        private void Update()
        {
            if (ragdolling && (Time.time - ragdollStartTime) > 2f)
            {
                GetComponent<Collider>().enabled = false;
                ragdollStartTime = float.MaxValue;
            }
        }

        public void EnableRagdoll()
        {
            ragdolling = true;
            ragdollStartTime = Time.time;
            //rb.isKinematic = true;
            //rb.useGravity = false;
            //rb.GetComponent<Collider>().enabled = false;

            foreach (var b in bones)
            {
                b.isKinematic = false;
                b.useGravity = true;
                b.GetComponent<Collider>().enabled = true;
            }
            animator.enabled = false;

            //cinemachine.Follow = root.transform;
            //cinemachine.LookAt = root.transform;
        }

        public void DisableRagdoll()
        {
            ragdolling = false;
            //rb.isKinematic = false;
            //rb.useGravity = true;
            //rb.GetComponent<Collider>().enabled = true;

            foreach (var b in bones)
            {
                b.isKinematic = true;
                b.useGravity = false;
                b.GetComponent<Collider>().enabled = false;
            }

            transform.position = root.position;
            animator.enabled = true;

            //cinemachine.Follow = transform;
            //cinemachine.LookAt = transform;
        }
    }
}
