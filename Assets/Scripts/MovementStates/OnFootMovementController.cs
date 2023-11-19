using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.MovementStates
{
    public class OnFootMovementController : MovementController
    {
        public float forceModifier = 30f;
        public float slowDownModifer = 10f;

        [Range(0f, 2f)]
        public float boulderMovementPreventionDistance = 1f;

        public override MovementState ApplicableMovementState => MovementState.OnFoot;

        void Update()
        {
            //TODO will need to set differently. Probably have something that is responsible for animation? maybe based on movement state?
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

            // Sets max speed taking into account if you are falling (hopefully...)
            //TODO used by bolder movement controller so should be put in base class
            var goingDown = rb.velocity.y < 0f;
            var speedCheckVelocity = goingDown 
                ? new Vector3(rb.velocity.x, 0f, rb.velocity.z)
                : rb.velocity;
            if (speedCheckVelocity.magnitude > MaxSpeed)
            {
                speedCheckVelocity.Normalize();
                speedCheckVelocity *= MaxSpeed;
                rb.velocity = goingDown
                    ? new Vector3(speedCheckVelocity.x, rb.velocity.y, speedCheckVelocity.z)
                    : speedCheckVelocity;
            }
        }

        private void SlowDown()
        {
            // So we don't fight gravity
            if (!Physics.Raycast(Position, Vector3.down, Height * 1.2f))
            {
                return;
            }

            var direction = -rb.velocity;
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