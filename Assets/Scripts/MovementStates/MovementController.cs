using Assets.Scripts.BoulderStuff;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Assets.Scripts.MovementStates
{
    public enum MovementState
    {
        OnFoot,
        Pushing,
        Ragdolling
    }

    public abstract class MovementController : MonoBehaviour
    {
        protected Animator animator; //TODO animation should be managed by somethign else. Here is fine for now
        protected Rigidbody rb;
        protected Transform boulderTransform;
        protected Rigidbody boulderRb;
        protected Vector3 lastMoveDir;
        protected MovementStateController msc;
        protected Rig rig;


        public float MaxSpeed { get; set; }

        //TODO maybe move to some character info page? used in two locations
        protected virtual float Height => GetComponent<CapsuleCollider>().height;
        protected float BoulderRadius => boulderTransform.GetComponent<SphereCollider>().radius;


        public Vector3 inputMoveDir = Vector3.zero;

        public abstract MovementState ApplicableMovementState { get; }


        public Vector3 Position => transform.position + Height / 2 * transform.up;


        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            Boulder boulder = GameObject.FindFirstObjectByType<Boulder>();
            boulderTransform = boulder.transform;
            boulderRb = boulderTransform.GetComponent<Rigidbody>();
            msc = GetComponent<MovementStateController>();
        }

        /// <summary>
        /// Will just move direction based on the ground below
        /// </summary>
        protected Vector3 GetCorrectedMoveDir(Vector3 originPosition)
        {
            if (Physics.Raycast(originPosition, Vector3.down, out var hitInfo, Height * 1.2f)) //TODO check for ground tag?
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

        public virtual void AddForce(Vector3 force, ForceMode forceMode)
        {
            rb.AddForce(force, forceMode);
        }

        private void OnDrawGizmos()
        {
            if (enabled && !boulderTransform.IsUnityNull())
            {
                //Drawing the ground checker
                Handles.color = Color.blue;

                Handles.DrawLine(Position, Position + lastMoveDir, 5f);
            }
        }
    }
}