using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.AstheticObjects
{
    public class Grass : MonoBehaviour
    {
        Renderer[] rends;
        WindBox[] windBoxes;
        private void Start()
        {
            rends = GetComponentsInChildren<Renderer>();
            windBoxes = FindObjectsOfType<WindBox>();

            foreach (Renderer rend in rends)
            {

                foreach(WindBox windBox in windBoxes)
                {
                    if (windBox.boxCollider.bounds.Contains(transform.position))
                    {

                        rend.material.SetFloat("_WindStrength", 2f);
                    }
                }

            }

        }


    }
}
