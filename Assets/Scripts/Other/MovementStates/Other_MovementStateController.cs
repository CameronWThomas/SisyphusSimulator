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
        
        private void Start()
        {
            cameraRef = FindObjectOfType<CameraController>();
            movementControllers = GetComponents<MovementController>();
            
            foreach (var movementController in movementControllers)
            {
                movementController.enabled = false;
            }

            currentMovementController = movementControllers.First(x => x.ApplicableMovementState == MovementState.OnFoot);
            currentMovementController.enabled = true;
        }

        private void Update()
        {
            var input = GetComponent<Other_PlayerInputBus>().moveInput;
            moveDir = (new Vector3(input.x, 0, input.y).normalized) * input.magnitude;
            moveDir = cameraRef.PlanarRotation2 * moveDir;

            currentMovementController.moveDir = moveDir;

            if (Input.GetKeyDown(KeyCode.R))
            {
                var ragdollingState = GetComponent<RagdollController>();
                if (ragdollingState.ragdolling)
                {
                    ragdollingState.DisableRagdoll();
                }
                else
                {
                    ragdollingState.EnableRagdoll();
                }
            }
        }

        public void ChangeState(MovementState newState)
        {
            // Activate new state and deactivate other
            currentMovementController.enabled = false;
            currentMovementController = movementControllers.First(x => x.ApplicableMovementState == newState);
            currentMovementController.enabled = true;

            currentMovementController.moveDir = moveDir;
        }
    }
}
