using Assets.Scripts;
using Assets.Scripts.BoulderStuff;
using Assets.Scripts.Deer;
using Assets.Scripts.MovementStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.InputSystem.XR;

public class DeerController : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    DeerWaypoint[] waypoints;
    WorldManager wm;
    public DeerAiState currentState;
    public GrazingState grazingState;
    public ChargingState chargingState;
    DeerRagdollController drc;

    public float BehRecheckCounter = 0f;
    public Vector2 BehRecheckFrameRange = new Vector2(20, 60);
    public float BehRecheckFrames = -1f;
    public float velocity;
    public float curSpeed;
    public float runSpeed = 7f;
    public float walkSpeed = 3.5f;

    public float hostilityChance = 5f;


    public float ragdollActivationImpulse = 200f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        waypoints = FindObjectsOfType<DeerWaypoint>();

        wm = FindObjectOfType<WorldManager>();
        grazingState = GetComponent<GrazingState>();
        chargingState = GetComponent<ChargingState>();
        currentState = grazingState;
        drc = GetComponent<DeerRagdollController>();
        agent.speed = walkSpeed;
        BehRecheckCounter = Mathf.Infinity;

    }

    // Update is called once per frame
    void Update()
    {
        BehRecheckCounter += Time.deltaTime;
        if(BehRecheckCounter > BehRecheckFrames) 
        {
            RecheckBehState();
        }
        curSpeed = agent.velocity.magnitude;
        velocity = agent.velocity.magnitude / runSpeed;
        anim.SetFloat("speedPercent", velocity);
    }
    public void ChangeState(DeerAiState newState)
    {
        anim.SetBool("wallLean", false);
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.enabled = true;
        currentState.OnEnter();
    }

    public void EndCharge()
    {
        ChangeState(grazingState);
    }
    public void RecheckBehState()
    {

        if(wm.absurdityLevel < 0.3f)
        {
            ChangeState(grazingState);
        }
        else 
        {
            bool shouldCharge = UnityEngine.Random.Range(0, 100) <= hostilityChance;
            if(shouldCharge)
                ChangeState(chargingState);
            else
                ChangeState(grazingState);
        }

        BehRecheckFrames = UnityEngine.Random.Range(BehRecheckFrameRange.x, BehRecheckFrameRange.y);
        BehRecheckCounter = 0f;
    }

    public void SetRunning()
    {
        agent.speed = runSpeed;
    }
    public void SetWalking()
    {
        agent.speed = walkSpeed;
    }

    private void Kill(Vector3 forceImpulse)
    {

        agent.isStopped = true;

        currentState.enabled = false;
        drc.EnableRagdoll();
        drc.root.AddForce(forceImpulse, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > ragdollActivationImpulse)
        {
            Kill(collision.impulse);
            //ChangeState(MovementState.Ragdolling);
            //currentMovementController.AddForce(collision.impulse, ForceMode.Impulse);
        }
    }
}

public abstract class DeerAiState : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;
    public DeerWaypoint[] waypoints;
    public Vector3 targetPosition = Vector3.zero;
    protected DeerController deerController;

    bool chargeReady;
    // Start is called before the first frame update
    protected void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        waypoints = FindObjectsOfType<DeerWaypoint>();
        deerController = GetComponent<DeerController>();

    }

    public Vector3 ConvertPositionToNavmesh(Vector3 position)
    {
        NavMeshHit myNavHit;
        if (NavMesh.SamplePosition(position, out myNavHit, 100, -1))
        {
            return myNavHit.position;
        }

        return position;

    }

    public bool IsAtDestination()
    {
        float dist = Vector3.Distance(transform.position, targetPosition);
        if(dist < 1f)
        {
            return true;
        }
        return false;
    }
    private void Initialize()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        waypoints = FindObjectsOfType<DeerWaypoint>();
        FurtherInit();
    }
    public abstract void FurtherInit();
    public void OnEnter()
    {
        Initialize();
    }
    public void OnExit()
    {
        this.enabled = false;
    }
    
}
