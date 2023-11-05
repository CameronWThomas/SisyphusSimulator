using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace Assets.Scripts
{
    public class PlayerInputBus : MonoBehaviour
    {

        PlayerControls inputActions;
        //BoulderPhysicsMvmntController bpmc;
        MovementStateController mc;
        CameraController camera;

        public Vector2 moveInput;
        public Vector2 rightStickInput;

        //public bool interactInput;

        public InputAction move;
        public InputAction look;
        public InputAction interact;
        private void Start()
        {
            //bpmc = GetComponent<BoulderPhysicsMvmntController>();
            mc = GetComponent<MovementStateController>();
            camera = GameObject.FindAnyObjectByType<CameraController>();
        }

        private void Update()
        {
            moveInput = move.ReadValue<Vector2>();
            rightStickInput = look.ReadValue<Vector2>();

            //interactInput = interact.ReadValue<bool>();

            //bpmc.inputDir = moveInput;
            mc.inputDir = moveInput;
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
            interact.performed += OnInteractStart;
            interact.canceled += OnInteractEnd;
        }
        void OnInteractStart(InputAction.CallbackContext context)
        {
            
        }
        void OnInteractEnd(InputAction.CallbackContext context)
        {
            //TODO: increase Complexity
            mc.SwitchState();
        }

        private void OnDisable()
        {
            move.Disable();
            look.Disable();
            interact.Disable();
        }
    }
}
