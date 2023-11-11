using Assets.Scripts.BoulderStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.MovementStates
{
    public class OnFootState : MovementState
    {


        private void Update()
        {
            GroundCheck();
        }
        public override void Move(UnityEngine.Vector2 inputDir)
        {
            float baseSpeed = moveSpeed;
            float targetSpeed = baseSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothStep(currentSpeed, targetSpeed, speedStepMultiplier * Time.deltaTime);

            animator.speedPercent = currentSpeed / moveSpeed;

            float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
            var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
            var moveDir = cameraController.PlanarRotation2 * moveInput;
            //var moveDir = Quaternion.Euler(cameraController.transform.forward) * moveInput;
            //var moveDir = cameraController.fwd.normalized + moveInput;

            //falling
            if (isGrounded && !isJumping)
            {
                velocityY = -0.5f;
            }
            else
            {
                velocityY += Time.deltaTime * Physics.gravity.y;
            }

            var velocity = moveDir * currentSpeed;
            velocity.y = velocityY;




            controller.Move(velocity * Time.deltaTime);

            if (moveAmount > 0)
            {
                targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                //targetRotation.y = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;

            }
        }
        public override void StateEntered()
        {
            cameraController.SetWalkingCamera();

            controller.enabled = true;
            Collider myCollider = GetComponent<Collider>();
            myCollider.enabled = true;
            //controller.transform.parent = null;
        }

        

    }
}
