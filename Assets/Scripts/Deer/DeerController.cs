using Assets.Scripts;
using Assets.Scripts.BoulderStuff;
using Assets.Scripts.Deer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class DeerController : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    DeerWaypoint[] waypoints;
    WorldManager wm;
    public DeerAiState currentState;
    public GrazingState grazingState;

    public float BehRecheckCounter = 0f;
    public Vector2 BehRecheckFrameRange = new Vector2(5, 30);
    public float BehRecheckFrames = -1f;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        waypoints = FindObjectsOfType<DeerWaypoint>();

        wm = FindObjectOfType<WorldManager>();
        grazingState = GetComponent<GrazingState>();
        currentState = grazingState;

    }

    // Update is called once per frame
    void Update()
    {
        BehRecheckCounter += Time.deltaTime;
        if(BehRecheckCounter > BehRecheckFrames) 
        {
            RecheckBehState();
        }
    }
    public void ChangeState(DeerAiState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = newState;
        currentState.enabled = true;
        currentState.OnEnter();
    }

    public void RecheckBehState()
    {

        if(wm.absurdityLevel < 0.3f)
        {
            ChangeState(grazingState);
        }

        BehRecheckFrames = UnityEngine.Random.Range(BehRecheckFrameRange.x, BehRecheckFrameRange.y);
        BehRecheckCounter = 0f;
    }
}

public abstract class DeerAiState : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;
    public DeerWaypoint[] waypoints;
    public Vector3 targetPosition = Vector3.zero;

    bool chargeReady;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        waypoints = FindObjectsOfType<DeerWaypoint>();

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
