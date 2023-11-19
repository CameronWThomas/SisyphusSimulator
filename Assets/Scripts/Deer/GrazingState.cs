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
        public float checkChangeWaypointTime = 13f;
        public float checkChangeCounter = 0f;



        public override void FurtherInit()
        {
            curWaypoint = GetBestWaypoint();
            waypoints = waypoints.Where(el => el.waypointType == DeerWaypoint.WaypointType.General).ToArray();
        }

        private void Update()
        {
            //go to a general waypoint. Preferably close to the player. Idle a while.
            agent.SetDestination( curWaypoint.transform.position);
            //wander around area in radius specified on waypoint

            checkChangeCounter += Time.deltaTime;
            if( checkChangeCounter > checkChangeWaypointTime )
            {
                curWaypoint = GetBestWaypoint();
                SetTargetToRandomPointAroundWaypoint();
                checkChangeCounter = 0f;
            }

            if (IsAtDestination())
            {
                if(checkChangeCounter > 5f)
                {
                    bool shouldCough = UnityEngine.Random.Range(0, 100) > 80;

                    anim.SetBool(shouldCough ? "cough" : "eatGrass", true);
                }
            }
            else
            {
                anim.SetBool("cough", false);
                anim.SetBool("eatGrass", false);
            }
        }

        public void SetTargetToRandomPointAroundWaypoint()
        {

            Vector3 centerOfRadius = curWaypoint.transform.position;
            float radius = 10f;
            Vector3 target = centerOfRadius + (Vector3)(radius * UnityEngine.Random.insideUnitCircle);

            targetPosition = curWaypoint.transform.position;
            //targetPosition = ConvertPositionToNavmesh(target);

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
    }
}
