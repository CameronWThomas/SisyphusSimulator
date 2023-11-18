﻿using Assets.Scripts.BoulderStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static BoulderDetector;

namespace Assets.Scripts.MovementStates
{
    public class RollingBoulderMovementController : MovementController
    {
        public float forceModifier = 30f;
        public float slowDownModifer = 10f;
        public float maxSpeed = 5f;

        public float maxCorrectiveVelocity = .45f;
        public float overcorrectionCounterStrength = 10f;

        public float minResistiveForceApproachSpeed = .1f;
        public float resistiveForceModifer = 20f;


        public override MovementState ApplicableMovementState => MovementState.Pushing;

        private const float ConstantForceModifier = 750f;

        private Rigidbody boulderRb;
        private Other_BoulderDetector boulderDetector;

        public override void Enable()
        {
            base.Enable();
            animator.SetBool("pushing", true);
        }

        protected override void Awake()
        {
            base.Awake();
            boulderRb = boulderTransform.GetComponent<Rigidbody>();
            boulderDetector = GetComponent<Other_BoulderDetector>();
        }

        void Update()
        {
            //animator.SetFloat("speedPercent", rb.velocity.magnitude / maxSpeed);
            animator.SetFloat("speedPercent", 1f);
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
        }

        private void BoulderCorrection()
        {
            var movingAwayFromCenter = boulderDetector.CorrectionVelocity * boulderDetector.CorrectionModifier < 0f;
            if (!(movingAwayFromCenter || boulderDetector.CorrectionVelocity < maxCorrectiveVelocity))
            {
                return;
            }

            var overCorrectionModifier = movingAwayFromCenter ? overcorrectionCounterStrength : 1f;

            var correctionForce = overCorrectionModifier * ConstantForceModifier * boulderDetector.CorrectionModifier * Time.fixedDeltaTime * transform.right;
            boulderRb.AddForce(correctionForce, ForceMode.Impulse);
        }

        private void BoulderResistance()
        {
            Debug.Log($"Boulder vel={boulderDetector.Resistance.magnitude}");
            if (boulderDetector.Resistance.magnitude < minResistiveForceApproachSpeed)
            {
                return;
            }

            boulderRb.AddForce(resistiveForceModifer * boulderDetector.Resistance, ForceMode.Impulse);
        }

        private void Move()
        {
            var correctedMoveDir = GetCorrectedMoveDir();
            if (correctedMoveDir == Vector3.zero)
            {
                return;
            }

            lastMoveDir = correctedMoveDir;
            boulderRb.AddForce(50 * boulderRb.mass * forceModifier * Time.fixedDeltaTime * correctedMoveDir, ForceMode.Force);

            rb.velocity = boulderRb.velocity;
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