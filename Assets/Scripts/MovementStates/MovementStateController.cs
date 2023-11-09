using Assets.Scripts.MovementStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BoulderStuff
{
    public class MovementStateController : MonoBehaviour
    {
        public Vector2 inputDir;
        public MovementState currentState;

        private CameraController cameraRef;

        CharacterController playerController;
        public Rigidbody boulderRb;
        public Boulder boulder;


        public OnFootState onFoot;
        public RollingBoulderState rolling;

        private void Start()
        {
            playerController = GetComponent<CharacterController>();
            cameraRef = GameObject.FindObjectOfType<CameraController>();

            onFoot = GetComponent<OnFootState>();
            rolling = GetComponent<RollingBoulderState>();

            boulder = FindObjectOfType<Boulder>();
            boulderRb = boulder.GetComponent<Rigidbody>();
            

            ChangeState(onFoot);
        }
        private void Update()
        {
            //currentState.UpdateState();
            if (currentState == onFoot)
            {
                currentState.Move(inputDir);
            }
        }
        private void FixedUpdate()
        {

            if (currentState == rolling)
            {
                currentState.Move(inputDir);
            }
        }

        public void ChangeState(MovementState newState)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }
            currentState = newState;
            currentState.enabled = true;
            currentState.OnEnter(playerController, boulderRb, cameraRef);
        }

        public void SwitchState()
        {
            if(currentState == onFoot)
            {
                ChangeState(rolling);
            }
            else
            {
                ChangeState(onFoot);
            }
        }

        
    }

    public abstract class MovementState : MonoBehaviour
    {
        GameObject gameObject { get; } 
        MonoBehaviour monoBehaviour { get; }

        public CharacterController controller;
        public CameraController cameraController;
        public Rigidbody rb;

        //must not start active
        private void Awake()
        {
            enabled = false;
        }

        public void OnEnter(CharacterController me, Rigidbody boulder, CameraController camera)
        {
            controller = me;
            rb = boulder;
            cameraController = camera;
            StateEntered();
        }
        public void OnExit()
        {
            this.enabled = false;
        }
        public abstract void StateEntered();
        public abstract void Move(Vector2 inputDir);
    }
}
