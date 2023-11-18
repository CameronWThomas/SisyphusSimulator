using Assets.Scripts.MovementStates;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.BoulderStuff
{
    public class Other_MovementStateController : MonoBehaviour
    {
        private Vector3 moveDir;

        private CameraController cameraRef;
        private MovementController[] movementControllers;
        private MovementController currentMovementController;
        private Other_BoulderDetector boulderDetector;

        private MovementState CurrentMovementState => currentMovementController.ApplicableMovementState;
        
        private void Start()
        {
            cameraRef = FindObjectOfType<CameraController>();
            movementControllers = GetComponents<MovementController>();
            boulderDetector = GetComponent<Other_BoulderDetector>();

            foreach (var movementController in movementControllers)
            {
                movementController.Disable();
            }

            currentMovementController = movementControllers.First(x => x.ApplicableMovementState == MovementState.OnFoot);
            currentMovementController.Enable();
        }

        private void Update()
        {
            var input = GetComponent<Other_PlayerInputBus>().moveInput;
            moveDir = (new Vector3(input.x, 0, input.y).normalized) * input.magnitude;
            moveDir = cameraRef.PlanarRotation2 * moveDir;

            currentMovementController.inputMoveDir = moveDir;

            MovementState newState = CurrentMovementState;
            if (Input.GetKeyDown(KeyCode.R))
            {
                newState = CurrentMovementState == MovementState.Ragdolling
                    ? MovementState.OnFoot : MovementState.Ragdolling;
            }
            else if (CurrentMovementState != MovementState.Ragdolling)
            {
                newState = boulderDetector.IsPushing ? MovementState.Pushing : MovementState.OnFoot;
            }

            if (newState != CurrentMovementState)
            {
                ChangeState(newState);
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
    }
}
