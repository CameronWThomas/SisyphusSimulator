using Assets.Scripts.MovementStates;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.BoulderStuff
{
    public class Other_MovementStateController : MonoBehaviour
    {
        public float maxSpeed = 5f;
        public float ragdollActivationFactor = 1250f;
        public float ragdollImpactMitigation = 100f;

        private Vector3 moveDir;

        private CameraController cameraRef;
        private MovementController[] movementControllers;
        private MovementController currentMovementController;
        private Other_BoulderDetector boulderDetector;
        private Rigidbody rb;
        private Vector3 lastVelocity;

        private MovementState CurrentMovementState => currentMovementController.ApplicableMovementState;
        
        private void Start()
        {
            cameraRef = FindObjectOfType<CameraController>();
            movementControllers = GetComponents<MovementController>();
            boulderDetector = GetComponent<Other_BoulderDetector>();
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
            var input = GetComponent<Other_PlayerInputBus>().moveInput;
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

        private void LateUpdate()
        {
            lastVelocity = rb.velocity;
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

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.transform.CompareTag("Boulder") && CurrentMovementState != MovementState.Ragdolling)
            {
                return;
            }

            var boulderTransform = collision.transform;
            var boulderRb = boulderTransform.GetComponent<Rigidbody>();
            
            var boulderToDirection = (currentMovementController.Position - boulderTransform.position).normalized;
            var magitudeVelocity = Vector3.Dot(boulderRb.velocity - lastVelocity, boulderToDirection);
            if (magitudeVelocity < 0f)
            {
                return;
            }

            var impactFactor = (boulderRb.mass * (magitudeVelocity / Time.fixedDeltaTime)) / rb.mass;
            Debug.Log($"impactFactor={impactFactor}");
            if (impactFactor > ragdollActivationFactor)
            {
                ChangeState(MovementState.Ragdolling);
                currentMovementController.AddForce((impactFactor / ragdollImpactMitigation) * boulderToDirection, ForceMode.Impulse);
            }
        }
    }
}
