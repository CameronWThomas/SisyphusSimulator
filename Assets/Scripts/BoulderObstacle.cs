using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoulderObstacle : MonoBehaviour
    {
        public AudioSource rollingSound;
        public Rigidbody rbody;
        public float soundVolume = 0;
        public float pitchScaleFactor = 0.5f;

        private float scaleSize = 0f;

        private void Start()
        {
            rbody = GetComponent<Rigidbody>();
            rollingSound = GetComponent<AudioSource>();
            rollingSound.volume = soundVolume;
            rollingSound.pitch = 1;
            rollingSound.Play();
            scaleSize = transform.localScale.x;
        }

        private void Update()
        {
            if (rbody.velocity.magnitude > 10)
            {
                soundVolume = 1;
            }
            else
            {
                soundVolume = rbody.velocity.magnitude / 10;
            }
            rollingSound.volume = soundVolume;
            float soundPitch = (1 - (-1 + (scaleSize * pitchScaleFactor)));
            rollingSound.pitch = Mathf.Clamp(soundPitch, 0.2f , 1);
        }
    }
}
