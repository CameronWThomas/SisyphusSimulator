using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SliderVolume : MonoBehaviour
    {
        [SerializeField] Slider soundSlider;
        [SerializeField] AudioMixer audioMixer;
        public string prefName;
        public string volumeName;
        private void Start()
        {
            SetVolume(PlayerPrefs.GetFloat(prefName, 100));
        }

        public void SetVolume(float _value)
        {
            if(_value < 1)
            {
                _value = 0.001f;
            }

            RefreshSlider(_value);
            PlayerPrefs.SetFloat(prefName, _value);
            audioMixer.SetFloat(volumeName, Mathf.Log10(_value / 100) * 20f);
        }
        public void SetVolumeFromSlider()
        {
            SetVolume(soundSlider.value);
        }
        public void RefreshSlider(float _value)
        {
            soundSlider.value = _value;
        }
    }
}
