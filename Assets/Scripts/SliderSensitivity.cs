using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace Assets.Scripts
{
    public class SliderSensitivity :MonoBehaviour
    {
        [SerializeField] Slider xSlider;
        [SerializeField] Slider ySlider;
        [SerializeField] CinemachineFreeLook cinemachine;
        public string xPrefName;
        public string yPrefName;

        private void Start()
        {

            SetSensitivity(PlayerPrefs.GetFloat(xPrefName, 180), true);
            SetSensitivity(PlayerPrefs.GetFloat(yPrefName, 2), false);
        }


        public void SetSensitivity(float _value, bool x)
        {
            if(x)
            {
                if(_value < 20) 
                    _value = 20;
            }
            else
            {
                if (_value < 0.01)
                    _value = 0.0001f;
            }

            if (x)
            {
                PlayerPrefs.SetFloat(xPrefName, _value);
                RefreshXSlider(_value);
                cinemachine.m_XAxis.m_MaxSpeed = _value;
            }
            else
            {
                PlayerPrefs.SetFloat(yPrefName, _value);
                RefreshYSlider(_value);
                cinemachine.m_YAxis.m_MaxSpeed = _value;
            }
        }
        public void SetXSensitivityFromSlider()
        {
            SetSensitivity(xSlider.value, true);
        }
        public void SetYSensitivityFromSlider()
        {
            SetSensitivity(ySlider.value, false);
        }
        public void RefreshXSlider(float _value)
        {
            xSlider.value = _value;
        }
        public void RefreshYSlider(float _value)
        {
            ySlider.value = _value;
        }
    }
}
