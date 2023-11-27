using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Deer
{
    public class GrazingState :DeerAiState
    {
        public  DeerWaypoint curWaypoint = null;
        public float checkChangeWaypointTime = 30f;
        public float checkChangeCounter = 0f;
        public float dist = Mathf.Infinity;


        public override void FurtherInit()
        {
            deerController = GetComponent<DeerController>();
            waypoints = FindObjectsOfType<DeerWaypoint>();
            waypoints = waypoints.Where(el => el.waypointType == DeerWaypoint.WaypointType.General || el.waypointType ==  DeerWaypoint.WaypointType.WallLean).ToArray();
            curWaypoint = GetBestWaypoint();
            agent.isStopped = false;
            deerController.SetWalking();
            checkChangeCounter = Mathf.Infinity;
            anim.SetBool("charging", false);
        }

        private void Update()
        {
            anim.SetBool("charging", false);
            dist = Vector3.Distance(transform.position, agent.destination);
            //go to a general waypoint. Preferably close to the player. Idle a while.
            //if (curWaypoint != null)
            //    agent.SetDestination( curWaypoint.transform.position);

            //wander around area in radius specified on waypoint

            checkChangeCounter += Time.deltaTime;
            if( checkChangeCounter > checkChangeWaypointTime )
            {
                curWaypoint = GetBestWaypoint();
                if(curWaypoint.waypointType == DeerWaypoint.WaypointType.WallLean)
                {
                    agent.SetDestination(curWaypoint.transform.position);
                }
                else
                {
                    SetTargetToRandomPointAroundWaypoint();
                }
                checkChangeCounter = 0f;
            }
            //in an attempt to stop spam
            if(checkChangeCounter < 2f)
            {
                if (dist > 10f)
                {
                    deerController.SetRunning();
                }
                else
                {
                    deerController.SetWalking();
                }
            }

            if (dist < 1)
            {
                if (curWaypoint != null)
                {
                    if (curWaypoint.waypointType == DeerWaypoint.WaypointType.WallLean)
                    {
                        anim.SetBool("wallLean", true);
                        transform.forward = curWaypoint.lookDir.normalized;
                    }
                }

                if (checkChangeCounter > 5f && checkChangeCounter < 6f)
                {
                    bool shouldCough = UnityEngine.Random.Range(0, 100) > 80;

                    anim.SetBool(shouldCough ? "cough" : "eatGrass", true);
                }

                if (checkChangeCounter > 17f && checkChangeCounter < 18f)
                {
                    bool shouldCough = UnityEngine.Random.Range(0, 100) > 80;

                    anim.SetBool(shouldCough ? "cough" : "eatGrass", true);
                }
            }
            else
            {
                anim.SetBool("cough", false);
                anim.SetBool("eatGrass", false);
                anim.SetBool("wallLean", false);
            }

            
        }

        public void SetTargetToRandomPointAroundWaypoint()
        {

            Vector3 centerOfRadius = curWaypoint.transform.position;
            float radius = curWaypoint.wanderRadius;
            Vector3 target = centerOfRadius + (Vector3)(radius * UnityEngine.Random.insideUnitCircle);

            //targetPosition = curWaypoint.transform.position;
            targetPosition = ConvertPositionToNavmesh(target);

            agent.SetDestination(targetPosition);
                
        }

        public DeerWaypoint GetBestWaypoint()
        {
            Sisyphus sisyphus = FindObjectOfType<Sisyphus>();   

            DeerWaypoint bestWaypoint = null;
            foreach(DeerWaypoint waypoint in waypoints)
            {
                if(bestWaypoint == null)
                {
                    bestWaypoint = waypoint;
                }
                else
                {
                    float oldDist = Vector3.Distance(bestWaypoint.transform.position, sisyphus.transform.position);
                    float newDist = Vector3.Distance(waypoint.transform.position, sisyphus.transform.position);

                    if(newDist < oldDist)
                    {
                        bestWaypoint = waypoint;
                    }
                }
            }
            return bestWaypoint;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(targetPosition, .2f);
        }
    }
}
