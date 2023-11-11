using Assets.Scripts.BoulderStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Assets.Scripts.MovementStates
{
    public class RollingBoulderState : MovementState
    {
        private Vector3 activeForce;
        public float forceMultiplier = 15f;
        BoulderFollower boulderFollower;
        public float heightOffset = 0.4f;

        public float rotateSpeed = 5f;
        public float pushDistance = 1f;

        public Vector3 targetPosition;
        public float targetDistance = Mathf.Infinity;
        public Vector3 targetDirection = Vector3.zero;

        MovementStateController msc;

        Vector2 inputDirIntercept;
        private void Start()
        {
            boulderFollower = FindObjectOfType<BoulderFollower>();  
            msc = GetComponent<MovementStateController>();
        }
        private void Update()
        {
            CheckDetach();
        }
        private void FixedUpdate()
        {

            CcMove(inputDirIntercept);
        }
        public override void Move(Vector2 inputDir)
        {

            inputDirIntercept = inputDir;
        }
        public void CcMove(Vector2 inputDir)
        {
            float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
            var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
            var moveDir = cameraController.PlanarRotation2 * moveInput;

            animator.speedPercent = 0f;
            float baseSpeed = moveSpeed;
            float targetSpeed = baseSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothStep(currentSpeed, targetSpeed, speedStepMultiplier * Time.deltaTime);
            animator.speedPercent = currentSpeed / moveSpeed;

            //falling
            if (isGrounded && !isJumping)
            {
                velocityY = -0.5f;
            }
            else
            {
                velocityY += Time.deltaTime * Physics.gravity.y;
            }

            //apply gravity
            Vector3 grav = Vector3.zero;
            grav.y = velocityY;
            controller.Move(grav * Time.deltaTime);


            if (moveDir != Vector3.zero)
            {
                targetPosition = boulderFollower.transform.position + (moveDir.normalized) * -boulderFollower.rotateRadius;
                targetDirection = targetPosition - transform.position;
                //targetPosition = GetTargetPosition(targetDirection);
                //targetPosition = GetTargetPosition((moveDir.normalized) * -1);
                MoveTowardsTarget(targetPosition);
            }

            if (targetDistance <= pushDistance)
            {
                animator.SetPushing(true);
                //TODO: cap speed?
                rb.AddForce(moveDir * forceMultiplier, ForceMode.Acceleration);

                //prevent warning log
                if (moveDir != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                }
            }
            else 
            {

                animator.SetPushing(false);
                targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                 
                
            }


            //just for insight
            activeForce = rb.GetAccumulatedForce();
        }
        void MoveTowardsTarget(Vector3 target)
        {

            //try remove y
            targetDirection.y = 0;

            Vector3 yLessTarget = target; yLessTarget.y = transform.position.y;
            targetDistance = Vector3.Distance(yLessTarget, transform.position);
            //Get the difference.
            if (targetDirection.magnitude > .1f)
            {
                //If we're further away than .1 unit, move towards the target.
                //The minimum allowable tolerance varies with the speed of the object and the framerate. 
                // 2 * tolerance must be >= moveSpeed / framerate or the object will jump right over the stop.
                targetDirection = targetDirection.normalized * currentSpeed;
                //normalize it and account for movement speed.
                controller.Move(targetDirection * Time.deltaTime);
                //actually move the character.
            }
        }
        private Vector3 GetTargetPosition(Vector3 direction)
        {

            Vector3 targetPos = boulderFollower.transform.position + 
                (direction * boulderFollower.rotateRadius);

            return new Vector3(targetPos.x, targetPos.y + heightOffset, targetPos.z);
        }
        public void NormalMove(Vector2 inputDir)
        {
            float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
            var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
            var moveDir = cameraController.PlanarRotation2 * moveInput;

            animator.speedPercent = 0f;

            if(moveDir != Vector3.zero )
            {
                SetPosition((moveDir.normalized) * -1);
            }

            //TODO: cap speed?
            rb.AddForce(moveDir * forceMultiplier, ForceMode.Acceleration);

            //just for insight
            activeForce = rb.GetAccumulatedForce();
        }

        private void SetPosition(Vector3 direction)
        {

            Vector3 targetPos = boulderFollower.transform.position + (direction * boulderFollower.rotateRadius);


            //move towards position
            Vector3 myPosition = transform.position;

            targetPos = new Vector3(targetPos.x, targetPos.y + heightOffset, targetPos.z);
            transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * rotateSpeed);
            //transform.position = Vector3.RotateTowards(transform.position, targetPos, Time.deltaTime * rotateSpeed, 0f);

        }

        
        public override void StateEntered()
        {
            cameraController.SetRollingCamera();
            //controller.enabled = false;
            Collider myCollider = GetComponent<Collider>();
            //myCollider.enabled = false;

            //wasnt getting set in start...
            boulderFollower = FindObjectOfType<BoulderFollower>();

            Vector3 direction = (boulderFollower.transform.position - transform.position).normalized;


            SetPosition(direction);
        }

        public void CheckDetach()
        {
            float dist = Vector3.Distance(transform.position, boulderFollower.transform.position);
            if(dist > boulderFollower.detachDistance)
            {
                msc.SwitchState();
            }
        }


        public override void Jump()
        {
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawSphere(targetPosition, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + targetDirection * 2);
        }

    }

    

}
