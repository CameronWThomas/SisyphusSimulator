using Assets.Scripts.MovementStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.BoulderStuff
{
    public class MovementStateController : MonoBehaviour
    {
        public Vector2 inputDir;
        public MovementStateOther currentState;

        private CameraController cameraRef;

        CharacterController playerController;
        public Rigidbody boulderRb;
        public Boulder boulder;


        [SerializeField] Vector3 groundCheckOffset;
        [SerializeField] LayerMask groundCheckLayerMask;
        [SerializeField] float groundCheckRadius = 0.2f;


        public OnFootState onFoot;
        public RollingBoulderState rolling;

        public bool isGrounded = false;
        public bool isJumping = false;
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
            GroundCheck();

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

        public void ChangeState(MovementStateOther newState)
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

        public void GroundCheck()
        {
            Vector3 checkPos = transform.TransformPoint(groundCheckOffset);
            isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundCheckLayerMask);
        }

        public void Jump()
        {
            currentState.Jump();
        }
        //private void OnDrawGizmos()
        //{
        //    //Drawing the ground checker
        //    Vector3 checkPos = transform.TransformPoint(groundCheckOffset);
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawSphere(checkPos, groundCheckRadius);
        //}

    }

    public abstract class MovementStateOther : MonoBehaviour
    {
        GameObject gameObject { get; } 
        MonoBehaviour monoBehaviour { get; }
        public SisyphusAnimator animator;
        public CharacterController controller;
        public CameraController cameraController;
        public Rigidbody rb;


        public float currentSpeed;
        public float speedStepMultiplier = 30f;
        public float moveSpeed = 10f;
        public UnityEngine.Quaternion targetRotation;
        protected MovementStateController msc;

        public float velocityY;
        public bool isGrounded => msc.isGrounded;
        public bool isJumping => msc.isJumping;

        public void SetIsGrounded(bool g)
        {
            msc.isGrounded = g;
        }
        public void SetIsJumping(bool j)
        {
            msc.isJumping = j;
        }
        //must not start active
        private void Awake()
        {
            enabled = false;
            animator = GetComponent<SisyphusAnimator>();
            msc = GetComponent<MovementStateController>();
        }
        public void OnEnter(CharacterController me, Rigidbody boulder, CameraController camera)
        {
            controller = me;
            rb = boulder;
            cameraController = camera;
            StateEntered();
        }
        public abstract void Jump();
        public void OnExit()
        {
            this.enabled = false;
        }
        public abstract void StateEntered();
        public abstract void Move(Vector2 inputDir);


        
    }
}
