using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class GuiText :MonoBehaviour
    {
        public List<MeshRenderer> m_renderer;

        public bool pauseText = false;
        public bool winText = false;
        public bool pushText = false;
        public float ttl = 20f;
        private float ttlCounter = 0f;
        private void Start()
        {
            m_renderer = GetComponents<MeshRenderer>().ToList();
            m_renderer.AddRange(GetComponentsInChildren<MeshRenderer>());
            if(ttl == -1)
            {
                HideText();
            }
        }

        private void Update()
        {
            if(ttl != -1)
            {
                ttlCounter += Time.deltaTime;
                if (ttlCounter > ttl) HideText();
            }
        }
        public void ResetTTL()
        {
            ttlCounter = 0f;
        }

        public void ShowText()
        {
            foreach (MeshRenderer renderer in m_renderer)
            {
                renderer.enabled = true;
            }
        }

        public void HideText()
        {
            foreach (MeshRenderer renderer in m_renderer)
            {
                renderer.enabled = false;
            }
        }

    }
}
