using Assets.Scripts.BoulderStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Deer
{
    public class DeerHitbox :MonoBehaviour
    {
        public bool attacking = false;
        public float impactFactor = 0.2f;
        DeerController dc;
        private void Start()
        {

            dc = GetComponentInParent<DeerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("colliderEntered");

            if (attacking)
            {
                Sisyphus sis = other.GetComponent<Sisyphus>();
                if (sis != null)
                {
                    MovementStateController msc = sis.GetComponent<MovementStateController>();
                    Vector3 dir = (msc.transform.position - dc.transform.position).normalized;
                    msc.ToggleRagdoll(impactFactor, dir);
                }
            }
        }
    }

    
}
