using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Deer
{
    public class ChargingState : DeerAiState
    {
        Sisyphus Sisyphus;
        public float dist = Mathf.Infinity;
        public override void FurtherInit()
        {
            Sisyphus = FindObjectOfType<Sisyphus>();
            deerController.SetRunning();
            anim.SetBool("charging", true);
            dist = Mathf.Infinity;
        }

        private void Update()
        {
            dist = Vector3.Distance(transform.position, agent.destination);
            if (dist > 3f)
            {
                agent.SetDestination(Sisyphus.transform.position);
            }
            if(dist < 2f)
            {
                anim.SetBool("charging", false);
            }
        }
    }
}
