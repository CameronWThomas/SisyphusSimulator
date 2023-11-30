using System;
using UnityEngine;
using UnityEngine.XR;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.MovementStates
{
    public class OnFootMovementController : MovementController
    {
        public float forceModifier = 30f;
        public float slowDownModifer = 10f;
        public float sprintSpeedMultiplier = 2f;

        [Range(0f, 2f)]
        public float boulderMovementPreventionDistance = 1f;

        public override MovementState ApplicableMovementState => MovementState.OnFoot;

        void Update()
        {
            //TODO Should use the SisyphusAnimator along with other uses. Will integrate with that later
            animator.SetFloat("speedPercent", rb.velocity.magnitude / MaxSpeed);
        }

        public override void Enable()
        {
            base.Enable();
            animator.SetBool("pushing", false);
        }

        void FixedUpdate()
        {
            lastMoveDir = Vector3.zero;
            if (inputMoveDir != Vector3.zero)
            {
                Move();
                Rotate();
            }
            else if (rb.velocity != Vector3.zero)
            {
                SlowDown();
            }
        }

        private void Move()
        {
            var correctedMoveDir = GetCorrectedMoveDir(Position);
            if (correctedMoveDir == Vector3.zero)
            {
                return;
            }

            var boulderTowardsUs = Vector3.Dot(boulderRb.velocity, (Position - boulderTransform.position).normalized);
            if (boulderTowardsUs < 1f && (boulderTransform.position - Position).magnitude < BoulderRadius + boulderMovementPreventionDistance)
            {
                var toBoulderDirection = (boulderTransform.position - Position).normalized;
                var toBoulderMagnitude = Vector3.Dot(toBoulderDirection, correctedMoveDir);
                if (toBoulderMagnitude > 0)
                {
                    correctedMoveDir -= toBoulderMagnitude * 2f * toBoulderDirection;
                }
            }

            lastMoveDir = correctedMoveDir;
            rb.AddForce(50 * rb.mass * forceModifier * Time.fixedDeltaTime * correctedMoveDir, ForceMode.Force);

            var maxSpeed = GetComponent<PlayerInputBus>().IsSprinting ? MaxSpeed * sprintSpeedMultiplier : MaxSpeed;
            if (rb.velocity.magnitude > maxSpeed)
            {
                var newVelocity = rb.velocity.normalized * maxSpeed;
                rb.velocity = newVelocity;
            }
        }

        private void SlowDown()
        {
            // So we only slow down when we are on the ground
            if (!Physics.Raycast(Position, Vector3.down, Height * 1.1f))
            {
                return;
            }

            var direction = new Vector3(-rb.velocity.x, 0f, -rb.velocity.z);
            rb.AddForce(50 * rb.mass * slowDownModifer * Time.fixedDeltaTime * direction, ForceMode.Force);
        }

        private void Rotate()
        {
            var lookRotation = Quaternion.LookRotation(inputMoveDir);
            var targetRotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.fixedDeltaTime * 5 * 180);
            transform.rotation = targetRotation;
        }
    }
}