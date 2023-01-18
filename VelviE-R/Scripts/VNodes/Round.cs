using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Maths/Round", "Rounds value of double or float variable .\n\nThe variable will contain the results.", VColor.Yellow02, "Ro")]
    public class Round : VBlockCore
    {
        [SerializeReference] public IVar variable;
        [SerializeField] public bool roundUp;
        [SerializeField] public VTypes vtype = VTypes.Float;
        public override void OnVEnter()
        {
            if(vtype == VTypes.Float)
            {
                if(!roundUp)
                variable.SetFloat(Mathf.Round(variable.GetFloat()));
                else
                variable.SetFloat(Mathf.Ceil(variable.GetFloat()));
            }
            else
            {
                if(!roundUp)
                variable.SetDouble(Math.Round(variable.GetDouble()));
                else
                variable.SetDouble(Math.Ceiling(variable.GetDouble()));
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(variable == null)
            {
                summary += "Variable can't be empty!";
            }

            return summary;
        }

    }
}