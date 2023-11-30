using Assets.Scripts.BoulderStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class WinTrigger : MonoBehaviour
    {
        List<GuiText> winText;
        public bool triggered = false;
        public Vector3 PushDir;
        public float PushForce = 1f;
        public float pushTime = 15f;
        public float pushCounter = 15f;
        public bool pushed;
        SphereCollider sc;
        private void Start()
        {
            winText = FindObjectsOfType<GuiText>().Where(x => x.winText).ToList();
            sc = GetComponent<SphereCollider>();    
        }

        public void ShowWinText()
        {
            foreach (var text in winText)
            {
                text.ResetTTL();
                text.ShowText();
            }
        }

        private void Update()
        {
            if(pushCounter < pushTime)
            {
                pushCounter += Time.deltaTime;
            }
            else
            {
                if (!pushed)
                {
                    pushed = true;
                    PushRigidbodies();
                }
            }
        }

        private void PushRigidbodies()
        {
            MovementStateController sisy = GameObject.FindObjectOfType<MovementStateController>();
            sisy.ToggleRagdoll(PushDir.normalized * PushForce);
            Boulder b = FindObjectOfType<Boulder>();
            Rigidbody bRb = b.GetComponent<Rigidbody>();
            bRb.AddForce(PushDir.normalized * PushForce, ForceMode.Impulse);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!triggered)
            {
                Sisyphus boulder = other.GetComponent<Sisyphus>();
                if (boulder != null)
                {
                    ShowWinText();
                    triggered = true;
                    pushCounter = 0f;
                    pushed = false;
                }
            }
        }
        public void Reset()
        {
            pushed = false;
            triggered = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position+ (PushDir.normalized * PushForce));
        }
    }
}
