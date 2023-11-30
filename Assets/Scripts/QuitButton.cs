using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class QuitButton :MonoBehaviour
    {

        public void QuitGame()
        {
            Application.Quit();
            //for editor
            EditorApplication.isPlaying = false;
        }
    }
}
