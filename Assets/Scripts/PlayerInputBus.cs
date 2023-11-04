using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class PlayerInputBus : MonoBehaviour
    {

        PlayerControls inputActions;
        BoulderPhysicsMvmntController bpmc;
        CameraReference camera;

        public Vector2 moveInput;
        public Vector2 rightStickInput;


        public InputAction move;
        public InputAction look;
        private void Start()
        {
            bpmc = GetComponent<BoulderPhysicsMvmntController>();
            camera = GameObject.FindAnyObjectByType<CameraReference>();
        }

        private void Update()
        {
            moveInput = move.ReadValue<Vector2>();
            rightStickInput = look.ReadValue<Vector2>();

            bpmc.inputDir = moveInput;
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
