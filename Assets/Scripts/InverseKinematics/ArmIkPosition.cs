using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.InverseKinematics
{
    public class ArmIkPosition :MonoBehaviour
    {
        public enum PositionType
        {
            Resting,
            Ack,
            FrontPushing,
            OutPushing
        }
        public bool right = false;
        public PositionType positionType;
    }
}
