using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Events/SendMessage", "Executes method in a gameObject.", VColor.Yellow03, "Sm")]
    public class SendMessage : VBlockCore
    {
        [SerializeField] public GameObject gameobject;
        [SerializeField] public string method;

        public override void OnVEnter()
        {
            if(gameobject != null && !String.IsNullOrEmpty(method))
            {
                gameobject.SendMessage("method");
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(gameobject == null)
            {
                summary += "GameObject can't be null.";
            }

            if(String.IsNullOrEmpty(method))
            {
                summary += "";
            }

            return summary;
        }
    }
}