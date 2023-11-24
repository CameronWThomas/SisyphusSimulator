using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Boulder : MonoBehaviour
    {
        BoulderScaleAdjuster scaleAdjuster;
        public AudioSource rollingSound;
        public Rigidbody rbody;
        public float soundVolume = 0;
        public float pitchScaleFactor = 0.5f;
        private void Start()
        {
            rbody = GetComponent<Rigidbody>();
            rollingSound = GetComponent<AudioSource>();
            scaleAdjuster = GetComponent<BoulderScaleAdjuster>();
            rollingSound.volume = soundVolume;
            rollingSound.pitch = 1;
            rollingSound.Play();
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
            float soundPitch = (1 - (-1 + (scaleAdjuster.scaleSize * pitchScaleFactor)));
            rollingSound.pitch = Mathf.Clamp(soundPitch, 0.2f , 1);

        }
    }
}
