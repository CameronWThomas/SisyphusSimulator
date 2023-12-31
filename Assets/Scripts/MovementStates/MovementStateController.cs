﻿using Assets.Scripts.MovementStates;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.BoulderStuff
{
    public class MovementStateController : MonoBehaviour
    {
        public float maxSpeed = 5f;
        public float ragdollActivationImpulse = 200f;

        private CameraController cameraRef;
        private MovementController[] movementControllers;
        private MovementController currentMovementController;
        private BoulderDetector boulderDetector;
        private Rigidbody rb;

        private Vector3 moveDir;
        private MovementState CurrentMovementState => currentMovementController.ApplicableMovementState;

        private AudioSource player;
        public AudioClip hitSound;

        public Vector3 handjamPosFix;
        public float posFixTime = 2f;
        public float posFixCounter;
        public bool PushingBoulder => currentMovementController == 
            movementControllers.Where(el => 
                el.ApplicableMovementState == MovementState.Pushing
            ).First();
        
        private void Start()
        {
            cameraRef = FindObjectOfType<CameraController>();
            movementControllers = GetComponents<MovementController>();
            boulderDetector = GetComponent<BoulderDetector>();
            rb = GetComponent<Rigidbody>();
            player = GetComponent<AudioSource>();

            foreach (var movementController in movementControllers)
            {
                movementController.Disable();
                movementController.MaxSpeed = maxSpeed;
            }

            currentMovementController = movementControllers.First(x => x.ApplicableMovementState == MovementState.Ragdolling );
            currentMovementController.Enable();
        }

        public void HandJamPosFix(Vector3 posFix)
        {
            handjamPosFix = posFix;
            posFixCounter = 0f;
        }
        private void Update()
        {
            
            var input = GetComponent<PlayerInputBus>().moveInput;
            moveDir = (new Vector3(input.x, 0, input.y).normalized) * input.magnitude;
            moveDir = cameraRef.PlanarRotation2 * moveDir;

            currentMovementController.inputMoveDir = moveDir;

            MovementState newState = CurrentMovementState;
            //if (Input.GetKeyDown(KeyCode.R) && CurrentMovementState != MovementState.Ragdolling)
            //{
            //    newState = MovementState.Ragdolling;
            //}
            if (CurrentMovementState != MovementState.Ragdolling)
            {
                newState = boulderDetector.IsPushing ? MovementState.Pushing : MovementState.OnFoot;
            }

            if (newState != CurrentMovementState)
            {
                ChangeState(newState);
            }

            if(posFixCounter < posFixTime) 
            {
                posFixCounter += Time.deltaTime;
                transform.position = handjamPosFix;
            }
            
            
        }
        public void ChangeState(MovementState newState)
        {
            if (currentMovementController.ApplicableMovementState == newState)
            {
                return;
            }

            // Activate new state and deactivate other
            currentMovementController.Disable();
            currentMovementController = movementControllers.First(x => x.ApplicableMovementState == newState);
            currentMovementController.Enable();

            currentMovementController.inputMoveDir = moveDir;
        }
        public void ToggleRagdoll(Vector3 forceVector)
        {
            player.PlayOneShot(hitSound);
            ChangeState(MovementState.Ragdolling);
            currentMovementController.AddForce(forceVector, ForceMode.Impulse);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.impulse.magnitude > ragdollActivationImpulse)
            {
                ToggleRagdoll(collision.impulse);
                //ChangeState(MovementState.Ragdolling);
                //currentMovementController.AddForce(collision.impulse, ForceMode.Impulse);
            }
        }
    }
}
