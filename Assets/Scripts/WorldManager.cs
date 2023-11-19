using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class WorldManager :MonoBehaviour
    {
        public float absurdityLevel = 0f;
        Sisyphus Sisyphus;

        private float zero;

        private void Start()
        {
            Sisyphus = FindObjectOfType<Sisyphus>();
            zero = Sisyphus.transform.position.y;
        }

        public void Update()
        {
            //treat my y as max height
            absurdityLevel = (Sisyphus.transform.position.y - zero) / transform.position.y;

        }
    }
}
