using Assets.Scripts.BoulderStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.MovementStates
{
    public class OnFootMovementController : MovementController
    {
        public float forceModifier = 30f;
        public float slowDownModifer = 10f;
        public float maxSpeed = 5f;

        private Rigidbody rb;

        public override MovementState ApplicableMovementState => MovementState.OnFoot;

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            //TODO will need to set differently. Probably have something that is responsible for animation? maybe based on movement state?
            animator.SetFloat("speedPercent", rb.velocity.magnitude / maxSpeed);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            animator.SetBool("pushing", false);
        }

        void FixedUpdate()
        {
            if (moveDir != Vector3.zero)
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
            var lookRotation = Quaternion.LookRotation(moveDir);
            var targetRotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.fixedDeltaTime * 5 * 180);
            transform.rotation = targetRotation;
        }
    }
}

//public class OnFootMovementController : MovementController
//{

//    //Jumping shit
//    public float jumpHeight = 5;

//    public float jumpTimer = 0f;
//    float jumpTime = 0.5f;
//    public float targetedYVelocity = 0f;

//    public override void Move(UnityEngine.Vector2 inputDir)
//    {
//        float baseSpeed = moveSpeed;
//        float targetSpeed = baseSpeed * inputDir.magnitude;
//        currentSpeed = Mathf.SmoothStep(currentSpeed, targetSpeed, speedStepMultiplier * Time.deltaTime);

//        animator.speedPercent = currentSpeed / moveSpeed;

//        float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
//        var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
//        var moveDir = cameraController.PlanarRotation2 * moveInput;
//        //var moveDir = Quaternion.Euler(cameraController.transform.forward) * moveInput;
//        //var moveDir = cameraController.fwd.normalized + moveInput;

//        //falling
//        if (isGrounded && !isJumping)
//        {
//            velocityY = -0.5f;
//        }
//        else
//        {
//            velocityY += Time.deltaTime * Physics.gravity.y;
//        }

//        var velocity = moveDir * currentSpeed;
//        velocity.y = velocityY;




//        controller.Move(velocity * Time.deltaTime);

//        if (moveAmount > 0)
//        {
//            targetRotation = Quaternion.LookRotation(moveDir);
//            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
//            //targetRotation.y = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;

//        }
//    }

//    public override void Jump()
//    {
//        float effectiveHeight = (jumpHeight / (Physics.gravity.y / -2));

//        if (isGrounded)
//        {
//            SetIsGrounded(false);


//            SetIsJumping(true);
//            animator.SetJumping(true);
//            float jumpVelocity = Mathf.Sqrt(-2 * Physics.gravity.y * effectiveHeight);
//            velocityY = jumpVelocity;
//            targetedYVelocity = effectiveHeight;

//        }

//    }

//    public override void StateEntered()
//    {
//        cameraController.SetWalkingCamera();

//        animator.SetPushing(false);
//        controller.enabled = true;
//        Collider myCollider = GetComponent<Collider>();
//        myCollider.enabled = true;
//        //controller.transform.parent = null;
//    }



//}
