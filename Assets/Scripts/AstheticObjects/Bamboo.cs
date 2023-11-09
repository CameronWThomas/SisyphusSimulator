using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.AstheticObjects
{
    public class Bamboo : MonoBehaviour
    {
        public List<Rigidbody> rbody= new List<Rigidbody>();

        private void Start()
        {
            rbody = GetComponentsInChildren<Rigidbody>().ToList();


        }

        private void Update()
        {
            foreach (var r in rbody)
            {
                r.AddForce(Physics.gravity * -1);
            }
        }
    }
}
