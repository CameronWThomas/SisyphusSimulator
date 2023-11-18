using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MovementStates
{
    public class RagdollMovementController : MovementController
    { 
        List<Rigidbody> bones;
        public Rigidbody root;

        public bool ragdolling = false;

        public override MovementState ApplicableMovementState => MovementState.Ragdolling;

        protected override void Awake()
        {
            base.Awake();

            bones = root.GetComponentsInChildren<Rigidbody>().ToList();
            DisableRagdoll();
        }

        public override void Enable()
        {
            base.Enable();
            EnableRagdoll();
        }

        public override void Disable()
        {
            base.Disable();
            DisableRagdoll();
        }

        private void EnableRagdoll()
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

        private void DisableRagdoll()
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
