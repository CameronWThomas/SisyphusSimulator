using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.MovementStates
{
    public class RagdollMovementController : MovementController
    {
        private enum RagdollStates
        {
            Initial,
            BufferForFall,
            WaitingForStill
        }

        public Rigidbody root;
        public float timeTillStand = 1.5f;
        public float isNotMovingBias = 0.1f;

        List<Rigidbody> bones;        
        CinemachineFreeLook cinemachine;
        RagdollStates state = RagdollStates.Initial;
        float? timeOfEvent = null;
        float? TimePassed => timeOfEvent is null ? null : Time.time - timeOfEvent;

        bool IsStill => root.velocity.magnitude < isNotMovingBias;

        public override MovementState ApplicableMovementState => MovementState.Ragdolling;

        protected override void Awake()
        {
            base.Awake();

            bones = root.GetComponentsInChildren<Rigidbody>().ToList();
            cinemachine = FindObjectOfType<CinemachineFreeLook>();
            DisableRagdoll();
        }

        private void Update()
        {
            switch (state)
            {
                case RagdollStates.Initial:
                    state = RagdollStates.BufferForFall;
                    timeOfEvent = Time.time;
                    break;

                case RagdollStates.BufferForFall:
                    // velocity may be zero at start, so wait for some time to allow the ragdoll to fall
                    if (TimePassed >= timeTillStand)
                    {
                        state = RagdollStates.WaitingForStill;
                        timeOfEvent = null;
                    }
                    break;

                case RagdollStates.WaitingForStill:
                    if (TimePassed is null && IsStill)
                    {
                        timeOfEvent = Time.time;
                    }
                    else if (!IsStill)
                    {
                        timeOfEvent = null;
                    }
                    else if (TimePassed is not null && TimePassed >= timeTillStand)
                    {
                        msc.ChangeState(MovementState.OnFoot);
                    }
                    break;
            }
        }

        public override void Enable()
        {
            base.Enable();

            state = RagdollStates.Initial;
            EnableRagdoll();
        }

        public override void Disable()
        {
            base.Disable();

            DisableRagdoll();
        }

        public override void AddForce(Vector3 force, ForceMode forceMode)
        {
            root.AddForce(force, forceMode);
            //foreach (var bone in bones)
            //{
            //    bone.AddForce(force, forceMode);
            //}
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

            cinemachine.Follow = root.transform;
            cinemachine.LookAt = root.transform;
        }
        private void DisableRagdoll()
        {
            foreach (var b in bones)
            {
                b.isKinematic = true;
                b.useGravity = false;
                b.GetComponent<Collider>().enabled = false;
            }

            animator.enabled = true;
            Vector3 rootpos = new Vector3(root.position.x, root.position.y, root.position.z);
            //msc.handjamPosFix = rootpos;
            //transform.position = rootpos;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.GetComponent<Collider>().enabled = true;


            transform.position = rootpos;

            cinemachine.Follow = transform;
            cinemachine.LookAt = transform;

            msc.HandJamPosFix(rootpos);
            
        }
    }
}
