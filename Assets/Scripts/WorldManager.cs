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
        public bool paused;

        GuiText[] guis;

        private void Start()
        {
            Sisyphus = FindObjectOfType<Sisyphus>();
            zero = Sisyphus.transform.position.y;
            guis = FindObjectsOfType<GuiText>();
        }

        public void Update()
        {
            //treat my y as max height
            absurdityLevel = (Sisyphus.transform.position.y - zero) / transform.position.y;

        }

        public void PauseTriggered()
        {
            paused = !paused;

            if(paused)
            {
                foreach(GuiText gui in guis)
                {
                    if (gui.pauseText)
                    {
                        gui.ShowText();
                    }
                }
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
                foreach (GuiText gui in guis)
                {
                    if (gui.pauseText)
                    {
                        gui.HideText();
                    }
                }
            }
        }
    }
}
