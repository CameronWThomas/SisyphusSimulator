using Assets.Scripts.MovementStates;
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

            foreach (var movementController in movementControllers)
            {
                movementController.Disable();
                movementController.MaxSpeed = maxSpeed;
            }

            currentMovementController = movementControllers.First(x => x.ApplicableMovementState == MovementState.OnFoot);
            currentMovementController.Enable();
        }

        private void Update()
        {
            var input = GetComponent<PlayerInputBus>().moveInput;
            moveDir = (new Vector3(input.x, 0, input.y).normalized) * input.magnitude;
            moveDir = cameraRef.PlanarRotation2 * moveDir;

            currentMovementController.inputMoveDir = moveDir;

            MovementState newState = CurrentMovementState;
            if (Input.GetKeyDown(KeyCode.R) && CurrentMovementState != MovementState.Ragdolling)
            {
                newState = MovementState.Ragdolling;
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
        public void ToggleRagdoll(float impactFactor, Vector3 direction)
        {

            ChangeState(MovementState.Ragdolling);
            currentMovementController.AddForce((impactFactor / ragdollImpactMitigation) * direction, ForceMode.Impulse);
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.impulse.magnitude > ragdollActivationImpulse)
            {
                ChangeState(MovementState.Ragdolling);
                currentMovementController.AddForce(collision.impulse, ForceMode.Impulse);
            }
        }
    }
}
