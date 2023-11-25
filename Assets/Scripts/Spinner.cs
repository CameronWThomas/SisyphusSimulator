using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class Spinner : MonoBehaviour
    {

        public float spinSpeed = 20f;


        void Update()
        {
            transform.Rotate(Vector3.up * spinSpeed * Time.unscaledDeltaTime);
        }

    }
}
