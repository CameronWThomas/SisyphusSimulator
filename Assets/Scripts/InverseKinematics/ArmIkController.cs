using Assets.Scripts.BoulderStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.InverseKinematics
{
    public class ArmIkController : MonoBehaviour
    {
        public Transform r_ikTarget;
        public ArmIkPosition[] r_ikPositions;
        public ArmIkPosition r_activePosition;
        public Transform l_ikTarget;
        public ArmIkPosition[] l_ikPositions;
        public ArmIkPosition l_activePosition;

        MovementStateController msc; 

        public float moveSpeed = 5f;

        private bool rTrack = false;
        private bool lTrack = false;
        private void Start()
        {
            ArmIkPosition[] allIkPos = FindObjectsOfType<ArmIkPosition>().ToArray();
            r_ikPositions = allIkPos.Where(el => el.right).ToArray();
            l_ikPositions = allIkPos.Where(el => !el.right).ToArray();
            foreach(ArmIkPosition lPos in l_ikPositions)
            {
                lPos.transform.rotation = new Quaternion(lPos.transform.rotation.x * -1.0f,
                                            lPos.transform.rotation.y,
                                            lPos.transform.rotation.z,
                                            lPos.transform.rotation.w * -1.0f);
            }

            r_activePosition = r_ikPositions.Where(el => el.positionType == ArmIkPosition.PositionType.Resting).First();
            l_activePosition = l_ikPositions.Where(el => el.positionType == ArmIkPosition.PositionType.Resting).First();
        
            msc = GetComponent<MovementStateController>();
        }

        public void Update()
        {
            r_ikTarget.transform.position = Vector3.Lerp(r_ikTarget.transform.position, r_activePosition.transform.position, Time.deltaTime * moveSpeed);
            r_ikTarget.transform.rotation = Quaternion.Lerp(r_ikTarget.transform.rotation, r_activePosition.transform.rotation, Time.deltaTime * moveSpeed);

            l_ikTarget.transform.position = Vector3.Lerp(l_ikTarget.transform.position, l_activePosition.transform.position, Time.deltaTime * moveSpeed);
            l_ikTarget.transform.rotation = Quaternion.Lerp(l_ikTarget.transform.rotation, l_activePosition.transform.rotation, Time.deltaTime * moveSpeed);


            if (msc.PushingBoulder)
            {
                if (rTrack)
                    SetPosition(true, ArmIkPosition.PositionType.OutPushing);
                else
                    SetPosition(true, ArmIkPosition.PositionType.Resting);
                if (lTrack)
                    SetPosition(false, ArmIkPosition.PositionType.OutPushing);
                else
                    SetPosition(false, ArmIkPosition.PositionType.Resting);
            }
            else
            {
                if (rTrack)
                    SetPosition(true, ArmIkPosition.PositionType.Ack);
                else
                    SetPosition(true, ArmIkPosition.PositionType.Resting);
                if (lTrack)
                    SetPosition(false, ArmIkPosition.PositionType.Ack);
                else
                    SetPosition(false, ArmIkPosition.PositionType.Resting);
            }
        }

        public void SetPushing(bool right, bool pushing)
        {
            if(right)
                rTrack = pushing;
            else
                lTrack = pushing;
            


        }
        public void SetPosition(bool right, ArmIkPosition.PositionType position)
        {
            
            if (right)
                r_activePosition = r_ikPositions.Where(el => el.positionType == position).FirstOrDefault();
            else
                l_activePosition = l_ikPositions.Where(el => el.positionType == position).FirstOrDefault();
        }
        
    }
}
