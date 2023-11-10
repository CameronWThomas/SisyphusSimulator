using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class PlayerInputBus : MonoBehaviour
    {
        PlayerControls inputActions;

        public Vector2 moveInput;
        public Vector2 rightStickInput;
        public InputAction move;
        public InputAction look;

        private void Update()
        {
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
        }

        private void OnDisable()
        {
            move.Disable();
            look.Disable();
        }
    }
}
