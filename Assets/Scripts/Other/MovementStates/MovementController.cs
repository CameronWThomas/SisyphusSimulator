using Assets.Scripts;
using Assets.Scripts.BoulderStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum MovementState
{
    OnFoot,
    Pushing,
    Ragdolling
}

public abstract class MovementController : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody rb;
    protected Transform boulderTransform;
    protected Vector3 lastMoveDir;

    //TODO maybe move to some character info page? used in two locations
    protected float Height => GetComponent<CapsuleCollider>().height;

    public Vector3 inputMoveDir = Vector3.zero;

    public abstract MovementState ApplicableMovementState { get; }

    
    public Vector3 Position => transform.position + Height / 2 * transform.up;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        boulderTransform = GameObject.FindGameObjectsWithTag("Boulder").First().transform;
    }

    /// <summary>
    /// Will just move direction based on the ground below
    /// </summary>
    protected Vector3 GetCorrectedMoveDir()
    {
        if (Physics.Raycast(Position, Vector3.down, out var hitInfo, Height * 1.2f)) //TODO check for ground tag?
        {
            var angle = -Mathf.Abs(90f - Vector3.Angle(transform.forward, hitInfo.normal));
            return Quaternion.Euler(angle, 0f, 0f) * inputMoveDir;
        }

        return Vector3.zero;
    }

    public virtual void Enable()
    {
        enabled = true;
    }

    public virtual void Disable()
    {
        enabled = false;
    }

    private void OnDrawGizmos()
    {
        //Drawing the ground checker
        Handles.color = Color.red;

        Handles.DrawLine(Position, Position + lastMoveDir, 3f);
    }
}