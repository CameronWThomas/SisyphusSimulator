using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Assets.Scripts.MovementStates
{
    public class RollingBoulderMovementController : MovementController
    {
        public float constantForceModifier = 20f;
        public float moveForceModifier = 10f;

        public float maxCorrectiveVelocity = .45f;
        public float correctionForceModifier = 4f;
        public float overcorrectionCounterStrength = 10f;
        [Range(0f, 1f)]
        public float correctionTwoHandLoss = .2f;

        public float minResistiveForceApproachSpeed = .1f;
        public float resistiveForceModifer = 5f;
        [Range(0f, 1f)]
        public float resistanceOneHandLoss = .2f;

        [Range(0f, 1f)]
        public float catchUpSpeedIncrease = .2f;
        [Range(0f, 1f)]
        public float catchUpSpeedDecrease = .5f;
        [Range(0f, 1f)]
        public float distanceMin = .5f;
        [Range(0f, 1f)]
        public float distanceMax = .75f;

        public override MovementState ApplicableMovementState => MovementState.Pushing;

        protected override float Height
        {
            get
            {
                var boulderRadius = boulderTransform.GetComponent<SphereCollider>().radius;
                var boulderScale = boulderTransform.localScale.x;
                return 2f * boulderRadius * boulderScale;
            }
        }

        private BoulderDetector boulderDetector;

        public override void Enable()
        {
            base.Enable();
            animator.SetBool("pushing", true);
            //rig.weight = 1f;
        }

        protected override void Awake()
        {
            base.Awake();
            boulderDetector = GetComponent<BoulderDetector>();
            rig = GetComponentInChildren<Rig>();
        }        
        public override void Disable()
        {
            base.Disable();
            //rig.weight = 0f;
        }

        void Update()
        {
            animator.SetFloat("speedPercent", rb.velocity.magnitude / MaxSpeed);
        }

        private void FixedUpdate()
        {
            BoulderCorrection();
            BoulderResistance();

            lastMoveDir = Vector3.zero;
            if (inputMoveDir != Vector3.zero)
            {
                Move();
                Rotate();
            }
            //TODO have something where the player tries to catch up if not moving
        }

        private void BoulderCorrection()
        {
            var movingAwayFromCenter = (boulderDetector.CorrectionVelocity < 0f && boulderDetector.BoulderOnLeft)
                || (boulderDetector.CorrectionVelocity > 0f && !boulderDetector.BoulderOnLeft);
            if (!(movingAwayFromCenter || boulderDetector.CorrectionVelocity < maxCorrectiveVelocity))
            {
                return;
            }

            // We can overcorrect by moving the boulder too fast to the center and it moves fast on the other side. This fixes that
            var overCorrectionModifier = movingAwayFromCenter ? overcorrectionCounterStrength : 1f;

            // pos=right, neg=left
            var sign = boulderDetector.BoulderOnLeft ? 1f : -1f;
            var actualCorrectionForceModifier = (constantForceModifier * correctionForceModifier) * correctionTwoHandLoss;

            // Correction strength depends on which hand is being used and how many
            if (boulderDetector.LeftHand != boulderDetector.RightHand)
            {
                sign = boulderDetector.LeftHand ? 1f : -1f;
                actualCorrectionForceModifier = constantForceModifier * correctionForceModifier;
            }

            var power = overCorrectionModifier * actualCorrectionForceModifier * constantForceModifier * boulderDetector.CorrectionModifier * Time.fixedDeltaTime;
            var correctionForce = sign * power * transform.right;

            boulderRb.AddForce(correctionForce, ForceMode.Impulse);
        }

        private void BoulderResistance()
        {
            if (boulderDetector.Resistance.magnitude < minResistiveForceApproachSpeed)
            {
                return;
            }

            var force = resistiveForceModifer * boulderDetector.Resistance * (boulderRb.mass / 100f);
            if (boulderDetector.LeftHand != boulderDetector.RightHand)
            {
                force *= resistanceOneHandLoss;
            }

            
            boulderRb.AddForce(force, ForceMode.Impulse);
            rb.AddForce(-force / 10f, ForceMode.Impulse);
        }        

        private void Move()
        {
            var correctedMoveDir = GetCorrectedMoveDir(boulderTransform.position);
            if (correctedMoveDir == Vector3.zero)
            {
                correctedMoveDir = transform.forward;
            }

            var toBoulderDirection = (boulderTransform.position - transform.position).normalized;
            var angle = Vector3.Angle(toBoulderDirection, correctedMoveDir.normalized);
            //var force = Mathf.Abs(Mathf.Cos(angle)) * // TODO scale force based on the direction
            var force =
                constantForceModifier *
                boulderRb.mass * 
                moveForceModifier * 
                Time.fixedDeltaTime * 
                correctedMoveDir;

            boulderRb.AddForce(force, ForceMode.Force);
            
            var goingDown = boulderRb.velocity.y < 0;
            var speedCheckVelocity = goingDown
                ? new Vector3(boulderRb.velocity.x, 0f, boulderRb.velocity.z)
                : boulderRb.velocity;
            if (speedCheckVelocity.magnitude > MaxSpeed)
            {
                speedCheckVelocity.Normalize();
                speedCheckVelocity *= MaxSpeed;
                boulderRb.velocity = goingDown
                    ? new Vector3(speedCheckVelocity.x, boulderRb.velocity.y, speedCheckVelocity.z)
                    : speedCheckVelocity;
            }

            var playerMoveDir = GetCorrectedMoveDir(Position);
            if (playerMoveDir == Vector3.zero)
            {
                return;
            }

            var distance = (boulderTransform.position - Position).magnitude;
            var power = boulderRb.velocity.magnitude;
            if (distance > BoulderRadius + (boulderDetector.DetectionRadius * distanceMax))
            {
                power *= 1f + catchUpSpeedIncrease;
            }
            else if (distance < BoulderRadius + (boulderDetector.DetectionRadius * distanceMin)) 
            {
                power *= 1f - catchUpSpeedDecrease;
            }
            rb.velocity = power * playerMoveDir;

            lastMoveDir = playerMoveDir;
        }

        //TODO move to base
        private void Rotate()
        {
            var lookRotation = Quaternion.LookRotation(inputMoveDir);
            var targetRotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.fixedDeltaTime * 5 * 180);
            transform.rotation = targetRotation;
        }
    }
}