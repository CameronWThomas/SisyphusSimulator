using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public class WindBox : MonoBehaviour
    {

        private List<Rigidbody> rbList = new List<Rigidbody>();
        public Vector3 forceVector;
        [SerializeField]
        private Vector3 modulatedForceVector;
        [SerializeField]
        private float perlinSampleValue;
        public BoxCollider boxCollider;

        AudioSource windSound;
        Sisyphus sisyphus;

        public Vector2 perlinCoords = Vector2.zero;
        public Vector2 newPerlinCoords = Vector2.zero;
        public float modulationTickChange = 5f;
        private float modulationTickCounter = 0f;

        private void OnEnable()
        {
            boxCollider = GetComponent<BoxCollider>();

            Rigidbody[] allRbs = FindObjectsOfType<Rigidbody>();
            
            foreach(Rigidbody rb in allRbs)
            {
                if (boxCollider.bounds.Contains(rb.transform.position))
                {
                    rbList.Add(rb);
                }
            }
        }
        private void Start()
        {
            windSound = GetComponent<AudioSource>();
            windSound.volume = 0;
            sisyphus = FindObjectOfType<Sisyphus>();
        }
        private void Update()
        {
            modulationTickCounter += Time.deltaTime;
            if(modulationTickCounter > modulationTickChange)
            {
                newPerlinCoords.x = perlinCoords.x + UnityEngine.Random.Range(0, 5);
                newPerlinCoords.y = perlinCoords.y + UnityEngine.Random.Range(0, 5);
                modulationTickCounter = 0f;
            }

            perlinCoords = Vector2.Lerp(perlinCoords, newPerlinCoords, Time.deltaTime);

            if (rbList.Count > 0)
            {
                foreach (Rigidbody rb in rbList)
                {
                    Boulder boulder = rb.GetComponent<Boulder>();
                    float dist = Vector3.Distance(rb.transform.position, transform.position + boxCollider.center);
                    modulatedForceVector = forceVector;
                    perlinSampleValue = 0.3f + Mathf.PerlinNoise(perlinCoords.x, perlinCoords.y);
                    if (perlinSampleValue == 0f) perlinSampleValue = 0.01f;
                    modulatedForceVector *= perlinSampleValue;

                    Vector3 distAdjusted = modulatedForceVector / dist;
                    if(boulder != null)
                    {

                        rb.AddForce(distAdjusted * 50);
                    }
                    else
                    {
                        rb.AddForce(distAdjusted);
                    }
                }
            }

            if (boxCollider.bounds.Contains(sisyphus.transform.position))
            {
                Vector3 refPoint = transform.position + boxCollider.center;
                refPoint.y = sisyphus.transform.position.y;
                float dist = Vector3.Distance(refPoint, sisyphus.transform.position);

                ///goal, as distance approaches 0 bring volume to 1;


                windSound.volume = Mathf.Lerp(windSound.volume, 1 - dist / 100f, Time.deltaTime * 2);
            }
            else
            {
                windSound.volume = 0f;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rbList.Add(rb);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rbList.Remove(rb);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (boxCollider != null)
            {
                Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + forceVector);
        }
    }
}
