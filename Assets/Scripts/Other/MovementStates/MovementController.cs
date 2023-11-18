using Assets.Scripts.BoulderStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum MovementState
{
    OnFoot,
    Pushing
}

public abstract class MovementController : MonoBehaviour
{
    protected Animator animator;

    //TODO maybe move to some character info page? used in two locations
    protected float Height => GetComponent<CapsuleCollider>().height;

    public Vector3 moveDir = Vector3.zero;

    public abstract MovementState ApplicableMovementState { get; }

    
    public Vector3 Position => transform.position + Height / 2 * transform.up;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Will just move direction based on the ground below
    /// </summary>
    protected Vector3 GetCorrectedMoveDir()
    {
        if (Physics.Raycast(Position, Vector3.down, out var hitInfo, Height * 1.2f)) //TODO check for ground tag?
        {
            var angle = -Mathf.Abs(90f - Vector3.Angle(transform.forward, hitInfo.normal));
            return Quaternion.Euler(angle, 0f, 0f) * moveDir;
        }

        return Vector3.zero;
    }

    public virtual void OnEnabled() { }

    private void OnDrawGizmos()
    {
        //Drawing the ground checker
        Handles.color = Color.red;

        var position = transform.position + Height / 2 * transform.up;
        Handles.DrawLine(position, position + GetCorrectedMoveDir(), 3f);
    }
}


//public abstract class MovementController : MonoBehaviour
//{
//    GameObject gameObject { get; }
//    MonoBehaviour monoBehaviour { get; }
//    public SisyphusAnimator animator;
//    public CharacterController controller;
//    public CameraController cameraController;
//    public Rigidbody rb;


//    public float currentSpeed;
//    public float speedStepMultiplier = 30f;
//    public float moveSpeed = 10f;
//    public UnityEngine.Quaternion targetRotation;
//    MovementStateController msc;

//    public float velocityY;
//    public bool isGrounded => msc.isGrounded;
//    public bool isJumping => msc.isJumping;

//    public void SetIsGrounded(bool g)
//    {
//        msc.isGrounded = g;
//    }
//    public void SetIsJumping(bool j)
//    {
//        msc.isJumping = j;
//    }
//    //must not start active
//    private void Awake()
//    {
//        enabled = false;
//        animator = GetComponent<SisyphusAnimator>();
//        msc = GetComponent<MovementStateController>();
//    }
//    public void OnEnter(CharacterController me, Rigidbody boulder, CameraController camera)
//    {
//        controller = me;
//        rb = boulder;
//        cameraController = camera;
//        StateEntered();
//    }
//    public abstract void Jump();
//    public void OnExit()
//    {
//        this.enabled = false;
//    }
//    public abstract void StateEntered();
//    public abstract void Move(Vector2 inputDir);



//}
