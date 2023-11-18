using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Accessibility;

namespace Assets.Scripts
{
    public class RagdollController : MonoBehaviour
    { 
        Animator animator;
        List<Rigidbody> bones;
        public Rigidbody root;

        Rigidbody rb;

        public bool ragdolling = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            bones = root.GetComponentsInChildren<Rigidbody>().ToList();
            animator = GetComponent<Animator>();

            DisableRagdoll();

            //TODO this should get info from input bus rather than have the controller update it. Those should be private methods
        }

        public void EnableRagdoll()
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.GetComponent<Collider>().enabled = false;

            foreach (var b in bones)
            {
                b.isKinematic = false;
                b.useGravity = true;
                b.GetComponent<Collider>().enabled = true;
            }
            animator.enabled = false; 
            ragdolling= true;
        }

        public void DisableRagdoll()
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.GetComponent<Collider>().enabled = true;

            foreach (var b in bones)
            {
                b.isKinematic = true;
                b.useGravity = false;
                b.GetComponent<Collider>().enabled = false;
            }
            animator.enabled = true;
            ragdolling = false;
        }
    }
}
