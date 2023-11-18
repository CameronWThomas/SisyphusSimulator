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
        public float maxSpeed = 5f;

        [Range(0f, 2f)]
        public float boulderMovementPreventionDistance = 1f;

        public override MovementState ApplicableMovementState => MovementState.OnFoot;

        private float boulderRadius => boulderTransform.GetComponent<SphereCollider>().radius;

        void Update()
        {
            //TODO will need to set differently. Probably have something that is responsible for animation? maybe based on movement state?
            animator.SetFloat("speedPercent", rb.velocity.magnitude / maxSpeed);
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
            var correctedMoveDir = GetCorrectedMoveDir();
            if (correctedMoveDir == Vector3.zero)
            {
                return;
            }

            // If close to the boulder, prevent force being applied in the direction of the boulder so it is unmovable
            if ((boulderTransform.position - Position).magnitude < boulderRadius + boulderMovementPreventionDistance)
            {
                var toBoulderDirection = (boulderTransform.position - Position).normalized;
                var toBoulderMagnitude = Vector3.Dot(toBoulderDirection, correctedMoveDir);
                correctedMoveDir -= toBoulderMagnitude * 2f * toBoulderDirection;
            }

            lastMoveDir = correctedMoveDir;
            rb.AddForce(50 * rb.mass * forceModifier * Time.fixedDeltaTime * correctedMoveDir, ForceMode.Force);

            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity =  rb.velocity.normalized * maxSpeed;
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