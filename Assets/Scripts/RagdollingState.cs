using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Accessibility;

namespace Assets.Scripts
{
    //TODO: Implement movement state
    public class RagdollingState : MonoBehaviour
    { 
        Animator animator;
        List<Rigidbody> bones;
        public Rigidbody root;

        public bool ragdolling = false;

        private void Awake()
        {
            bones = GetComponentsInChildren<Rigidbody>().ToList();
            animator = GetComponent<Animator>();

            DisableRagdoll();
        }


        public void EnableRagdoll()
        {
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
