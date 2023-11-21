﻿using System;
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
                return;
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


//namespace Assets.Scripts.MovementStates
//{
//    public class Other_RollingBoulderState : MovementState
//    {
//        private Vector3 activeForce;
//        public float forceMultiplier = 15f;
//        BoulderFollower boulderFollower;
//        public float heightOffset = 0.4f;

//        public float rotateSpeed = 5f;
//        public float pushDistance = 1f;

//        public Vector3 targetPosition;
//        public float targetDistance = Mathf.Infinity;
//        public Vector3 targetDirection = Vector3.zero;

//        MovementStateController msc;

//        private BoulderDetector.BoulderMovementInfo boulderMI => GetComponent<BoulderDetector>().boulderMovementInfo;

//        Vector2 inputDirIntercept;
//        private void Start()
//        {
//            boulderFollower = FindObjectOfType<BoulderFollower>();  
//            msc = GetComponent<MovementStateController>();
//        }
//        private void Update()
//        {
//            CheckDetach();
//        }
//        private void FixedUpdate()
//        {

//            CcMove(inputDirIntercept);
//        }
//        public override void Move(Vector2 inputDir)
//        {

//            inputDirIntercept = inputDir;
//        }
//        public void CcMove(Vector2 inputDir)
//        {
//            float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
//            var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
//            var moveDir = cameraController.PlanarRotation2 * moveInput;

//            animator.speedPercent = 0f;
//            float baseSpeed = moveSpeed;
//            float targetSpeed = baseSpeed * inputDir.magnitude;
//            currentSpeed = Mathf.SmoothStep(currentSpeed, targetSpeed, speedStepMultiplier * Time.fixedDeltaTime);
//            animator.speedPercent = currentSpeed / moveSpeed;

//            //falling
//            if (isGrounded && !isJumping)
//            {
//                velocityY = -0.5f;
//            }
//            else
//            {
//                velocityY += Time.fixedDeltaTime * Physics.gravity.y;
//            }

//            //apply gravity
//            Vector3 grav = Vector3.zero;
//            grav.y = velocityY;
//            controller.Move(grav * Time.fixedDeltaTime);

//            if (boulderMI.IsPushing)
//            {
//                var direction = (transform.position - boulderFollower.transform.position).normalized;
//                targetPosition = boulderFollower.transform.position + direction * boulderFollower.rotateRadius;
//                targetDirection = targetPosition - transform.position;
//                //targetPosition = GetTargetPosition(targetDirection);
//                //targetPosition = GetTargetPosition((moveDir.normalized) * -1);
//                MoveTowardsTarget(targetPosition);
//            }

//            //if (moveDir != Vector3.zero)
//            //{
//            //    targetPosition = boulderFollower.transform.position + (moveDir.normalized) * -boulderFollower.rotateRadius;
//            //    targetDirection = targetPosition - transform.position;
//            //    //targetPosition = GetTargetPosition(targetDirection);
//            //    //targetPosition = GetTargetPosition((moveDir.normalized) * -1);
//            //    MoveTowardsTarget(targetPosition);
//            //}

//            if (targetDistance <= pushDistance)
//            {
//                animator.SetPushing(true);
//                //TODO: cap speed?
//                rb.AddForce(moveDir * forceMultiplier, ForceMode.Acceleration);

//                //prevent warning log
//                if (moveDir != Vector3.zero)
//                {
//                    targetRotation = Quaternion.LookRotation(moveDir);
//                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
//                }
//            }
//            else 
//            {

//                animator.SetPushing(false);
//                targetRotation = Quaternion.LookRotation(targetDirection);
//                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);


//            }


//            //just for insight
//            activeForce = rb.GetAccumulatedForce();
//        }
//        void MoveTowardsTarget(Vector3 target)
//        {

//            //try remove y
//            targetDirection.y = 0;

//            Vector3 yLessTarget = target; yLessTarget.y = transform.position.y;
//            targetDistance = Vector3.Distance(yLessTarget, transform.position);
//            //Get the difference.
//            if (targetDirection.magnitude > .1f)
//            {
//                //If we're further away than .1 unit, move towards the target.
//                //The minimum allowable tolerance varies with the speed of the object and the framerate. 
//                // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
//                //targetDirection = targetDirection.normalized * currentSpeed;
//                targetDirection = targetDirection.normalized;
//                //normalize it and account for movement speed.
//                Debug.Log($"Moving:{targetDirection * Time.deltaTime} target={target}");
//                controller.Move(targetDirection * Time.deltaTime);
//                //actually move the character.
//            }
//        }
//        private Vector3 GetTargetPosition(Vector3 direction)
//        {

//            Vector3 targetPos = boulderFollower.transform.position + 
//                (direction * boulderFollower.rotateRadius);

//            return new Vector3(targetPos.x, targetPos.y + heightOffset, targetPos.z);
//        }
//        public void NormalMove(Vector2 inputDir)
//        {
//            float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
//            var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
//            var moveDir = cameraController.PlanarRotation2 * moveInput;

//            animator.speedPercent = 0f;

//            if(moveDir != Vector3.zero )
//            {
//                SetPosition((moveDir.normalized) * -1);
//            }

//            //TODO: cap speed?
//            rb.AddForce(moveDir * forceMultiplier, ForceMode.Acceleration);

//            //just for insight
//            activeForce = rb.GetAccumulatedForce();
//        }

//        private void SetPosition(Vector3 direction)
//        {

//            Vector3 targetPos = boulderFollower.transform.position + (direction * boulderFollower.rotateRadius);


//            //move towards position
//            Vector3 myPosition = transform.position;

//            targetPos = new Vector3(targetPos.x, targetPos.y + heightOffset, targetPos.z);
//            transform.position = Vector3.Slerp(transform.position, targetPos, Time.fixedDeltaTime * rotateSpeed);
//            //transform.position = Vector3.RotateTowards(transform.position, targetPos, Time.fixedDeltaTime * rotateSpeed, 0f);

//        }


//        public override void StateEntered()
//        {
//            cameraController.SetRollingCamera();
//            //controller.enabled = false;
//            Collider myCollider = GetComponent<Collider>();
//            //myCollider.enabled = false;

//            //wasnt getting set in start...
//            boulderFollower = FindObjectOfType<BoulderFollower>();

//            Vector3 direction = (boulderFollower.transform.position - transform.position).normalized;


//            SetPosition(direction);
//        }

//        public void CheckDetach()
//        {
//            if (!boulderMI.IsPushing)
//            {
//                msc.ChangeState(msc.onFoot);
//            }

//            //float dist = Vector3.Distance(transform.position, boulderFollower.transform.position);
//            //if(dist > boulderFollower.detachDistance)
//            //{
//            //    msc.SwitchState();
//            //}
//        }


//        public override void Jump()
//        {
//        }
//        private void OnDrawGizmos()
//        {
//            Gizmos.color = Color.white;
//            Gizmos.DrawLine(transform.position, targetPosition);
//            Gizmos.DrawSphere(targetPosition, 0.1f);
//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(transform.position, transform.position + targetDirection * 2);
//        }

//    }



//}