using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AstheticObjects
{
    public class Foliage : MonoBehaviour
    {
        public List<Texture2D> textureOptions;
        public Texture2D transparent;
        public List<MeshRenderer> renderers;
        Camera main;
        private void Start()
        {
            main = Camera.main;
            renderers = GetComponentsInChildren<MeshRenderer>().ToList();

            WindBox[] windBoxes = FindObjectsOfType<WindBox>();

            int numRenderers = UnityEngine.Random.Range(0, renderers.Count);

            for(var i = 0; i < renderers.Count; i++)
            {
                if(i <= numRenderers)
                {
                    renderers[i].material.SetTexture("_Texture2D", textureOptions[UnityEngine.Random.Range(0, textureOptions.Count)]);

                }
                else
                {
                    renderers[i].material.SetTexture("_Texture2D", transparent);
                }

                foreach (WindBox windBox in windBoxes)
                {
                    if (windBox.boxCollider.bounds.Contains(transform.position))
                    {

                        renderers[i].material.SetFloat("_WindStrength", 2f);
                    }
                }
            }


        }

        private void Update()
        {
            transform.forward = main.transform.position - transform.position;
        }
    }
}
