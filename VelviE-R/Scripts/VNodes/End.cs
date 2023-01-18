using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Flow/End", "Conditional statement. Must be started/coupled with If", VColor.Yellow01, "En")]
    public class End : VBlockCore
    {
        public override void OnVExit()
        {
            OnVContinue();
        }
    }
}