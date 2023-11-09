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
        public float forceMultiplier = 2f;
        BoulderFollower boulderFollower;
        public float heightOffset = 0.4f;

        public float rotateSpeed = 5f;

        MovementStateController msc;

        private void Start()
        {
            boulderFollower = FindObjectOfType<BoulderFollower>();  
            msc = GetComponent<MovementStateController>();
        }
        private void Update()
        {
            CheckDetach();
        }
        public override void Move(Vector2 inputDir)
        {
            float moveAmount = Mathf.Abs(inputDir.x) + Mathf.Abs(inputDir.y);
            var moveInput = (new Vector3(inputDir.x, 0, inputDir.y)).normalized;
            var moveDir = cameraController.PlanarRotation2 * moveInput;

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
            controller.enabled = false;
            Collider myCollider = GetComponent<Collider>();
            myCollider.enabled = false;

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

        private void OnDrawGizmos()
        {
            
        }

    }

    

}
