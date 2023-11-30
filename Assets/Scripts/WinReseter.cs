using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WinReseter : MonoBehaviour
    {
        WinTrigger winTrigger;

        private void Start()
        {
            winTrigger = FindObjectOfType<WinTrigger>();
        }

        private void OnTriggerEnter(Collider other)
        {

            Boulder boulder = other.GetComponent<Boulder>();
            if (boulder != null)
            {
                winTrigger.triggered = false;
            }
        }

    }
}
