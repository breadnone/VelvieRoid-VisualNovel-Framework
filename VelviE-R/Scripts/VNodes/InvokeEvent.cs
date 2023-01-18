using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace VelvieR
{
    [VTag("Events/InvokeEvent", "Invoke methods based on UnityEvent class.", VColor.Green01, "Ie")]
    public class InvokeEvent : VBlockCore
    {
        [SerializeField] public UnityEvent invokeEvent;
        public override void OnVEnter()
        {
            if(invokeEvent != null)
            {
                invokeEvent.Invoke();
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(invokeEvent == null)
            {
                summary += "Events must be assigned!";
            }

            return summary;
        }
    }
}