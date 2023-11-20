using Assets.Scripts.BoulderStuff;
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
        BoulderDetector boulderDetector;
        //RagdollingState ragdoller;

        public Vector2 moveInput;
        public Vector2 rightStickInput;

        //public bool interactInput;

        public InputAction move;
        public InputAction look;
        public InputAction interact;
        public InputAction testRagdoll;
        public InputAction jump;

        public InputAction leftpush;
        public InputAction rightPush;

        private void Start()
        {
            //bpmc = GetComponent<BoulderPhysicsMvmntController>();
            mc = GetComponent<MovementStateController>();
            camera = GameObject.FindAnyObjectByType<CameraController>();
            boulderDetector = GetComponent<BoulderDetector>();
            //ragdoller = GetComponent<RagdollingState>();
        }

        private void Update()
        {
            moveInput = move.ReadValue<Vector2>();
            rightStickInput = look.ReadValue<Vector2>();
            //interactInput = interact.ReadValue<bool>();

            //bpmc.inputDir = moveInput;
            //mc.inputDir = moveInput;
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


            testRagdoll = inputActions.Player.TestToggleRagdoll;
            testRagdoll.Enable();
            testRagdoll.performed += OnRagdollStart;
            testRagdoll.canceled += OnRagdollEnd;


            jump = inputActions.Player.Jump;
            jump.Enable();
            jump.performed += OnJumpStart;
            jump.canceled += OnJumpEnd;

            leftpush = inputActions.Player.LeftPush;
            leftpush.Enable();
            leftpush.performed += OnLeftPushStart;
            leftpush.canceled += OnLeftPushEnd;


            rightPush = inputActions.Player.RightPush;
            rightPush.Enable();
            rightPush.performed += OnRightPushStart;
            rightPush.canceled += OnRightPushEnd;
        }
        void OnLeftPushStart(InputAction.CallbackContext context)
        {
            boulderDetector.LeftHand = true;
        }
        void OnLeftPushEnd(InputAction.CallbackContext context)
        {
            boulderDetector.LeftHand = false;
        }

        void OnRightPushStart(InputAction.CallbackContext context)
        {
            boulderDetector.RightHand = true;
        }
        void OnRightPushEnd(InputAction.CallbackContext context)
        {
            boulderDetector.RightHand = false;

        }
        void OnJumpStart(InputAction.CallbackContext context)
        {
            //mc.Jump();
        }
        void OnJumpEnd(InputAction.CallbackContext context)
        {

        }
        void OnRagdollStart(InputAction.CallbackContext context)
        {
            //ragdoller.EnableRagdoll();
        }
        void OnRagdollEnd(InputAction.CallbackContext context)
        {
            //ragdoller.DisableRagdoll();

        }
        void OnInteractStart(InputAction.CallbackContext context)
        {

        }
        void OnInteractEnd(InputAction.CallbackContext context)
        {
            //TODO: increase Complexity
            //mc.SwitchState();
        }

        private void OnDisable()
        {
            move.Disable();
            look.Disable();
            interact.Disable();
            testRagdoll.Disable();
            jump.Disable();
            leftpush.Disable();
            rightPush.Disable();

        }
    }
}