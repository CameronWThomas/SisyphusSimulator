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
        Canvas canvas;

        private void Start()
        {
            Sisyphus = FindObjectOfType<Sisyphus>();
            zero = 0f;
            guis = FindObjectsOfType<GuiText>();
            canvas = FindObjectOfType<Canvas>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            canvas.enabled = false;
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
                Cursor.lockState = CursorLockMode.Confined; 
                Cursor.visible = true;
                canvas.enabled = true;
                foreach (GuiText gui in guis)
                {
                    if (gui.pauseText)
                    {
                        gui.ShowText();
                    }
                    else if(!gui.winText)
                    {
                        gui.HideText();
                    }
                }
                Time.timeScale = 0;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;
                canvas.enabled = false;
                foreach (GuiText gui in guis)
                {
                    if (gui.pauseText)
                    {
                        gui.HideText();
                    }
                    else if (!gui.winText)
                    {
                        gui.ShowText();
                    }
                }
            }
        }
    }
}
