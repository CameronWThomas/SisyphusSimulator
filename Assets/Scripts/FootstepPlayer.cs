using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class FootstepPlayer :MonoBehaviour
    {

        public AudioSource footstepPlayer;
        public List<AudioClip> footsteps;

        public void Start()
        {
            footstepPlayer = GetComponent<AudioSource>();
            //footstepPlayer.clip = footsteps[UnityEngine.Random.Range(0, footsteps.Count)];
        }

        public void PlayFootstep()
        {
            footstepPlayer.PlayOneShot(footsteps[UnityEngine.Random.Range(0, footsteps.Count)]);
            //footstepPlayer.clip = 
        }
    }
}
