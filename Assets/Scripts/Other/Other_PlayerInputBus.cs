using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    /// <summary>
    /// Only responsible for getting input and putting it in one area.
    /// All classes that wish to act on it can add handlers to the InputAction or access the movement info it needs
    /// </summary>
    public class Other_PlayerInputBus : MonoBehaviour
    {
        PlayerControls inputActions;

        public Vector2 moveInput;
        public Vector2 rightStickInput;

        public InputAction move;
        public InputAction look;
        public InputAction interact;
        public InputAction testRagdoll;
        public InputAction jump;


        private void Update()
        {
            //TODO most likely will want to get the world space movement here?
            moveInput = move.ReadValue<Vector2>();
            rightStickInput = look.ReadValue<Vector2>();
        }

        private void Awake()
        {
            inputActions = new PlayerControls();
        }

        private void OnEnable()
        {
            move = inputActions.Player.Movement;
            move.Enable();

            look = inputActions.Player.Camera;
            look.Enable();

            interact = inputActions.Player.Interact;
            interact.Enable();
            //interact.performed += OnInteractStart;
            //interact.canceled += OnInteractEnd;


            testRagdoll = inputActions.Player.TestToggleRagdoll;
            testRagdoll.Enable();
            //testRagdoll.performed += OnRagdollStart
            //testRagdoll.canceled += OnRagdollEnd;


            jump = inputActions.Player.Jump;
            jump.Enable();
            //jump.performed += OnJumpStart;
            //jump.canceled += OnJumpEnd;
        }
    }
}
