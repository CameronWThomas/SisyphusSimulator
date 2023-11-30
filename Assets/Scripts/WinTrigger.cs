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
        List<GuiText> pushText;
        public bool triggered = false;
        public Vector3 PushDir;
        public float SisyphusPushForce = 700f;
        public float BoulderPushForce = 10000f;
        public float pushTime = 15f;
        public float pushCounter = 15f;
        public bool pushed;
        SphereCollider sc;
        private void Start()
        {
            winText = FindObjectsOfType<GuiText>().Where(x => x.winText).ToList();
            pushText = FindObjectsOfType<GuiText>().Where(x => x.pushText).ToList();
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
        public void ShowPushText()
        {
            foreach (var text in pushText)
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
                    ShowPushText();
                    PushRigidbodies();
                }
            }
        }

        private void PushRigidbodies()
        {
            MovementStateController sisy = GameObject.FindObjectOfType<MovementStateController>();
            sisy.ToggleRagdoll(PushDir.normalized * SisyphusPushForce);
            Boulder b = FindObjectOfType<Boulder>();
            Rigidbody bRb = b.GetComponent<Rigidbody>();
            bRb.AddForce(PushDir.normalized * BoulderPushForce, ForceMode.Impulse);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!triggered)
            {
                Boulder boulder = other.GetComponent<Boulder>();
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
            Gizmos.DrawLine(transform.position, transform.position+ (PushDir.normalized * SisyphusPushForce));
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position+ (PushDir.normalized * BoulderPushForce));
        }
    }
}
