using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("Animation/CancelAnimation", "Cancels active animations on target or all active entities.", VColor.Red, "Ro")]
    public class CancelAnimation : VBlockCore
    {
        [SerializeField, HideInInspector] public GameObject targetobject;
        [SerializeField, HideInInspector] public bool cancelAll = false;
        [SerializeField, HideInInspector] public bool executeOnComplete = false;
        public override void OnVEnter()
        {
            if(cancelAll)
            {
                LeanManager.CancelAll(executeOnComplete);
            }
            else if(!cancelAll && targetobject != null)
            {
                LeanManager.Cancel(targetobject, executeOnComplete);                
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(targetobject == null && !cancelAll)
            {
                summary += "Object to cancel can't be empty!";
            }

            return summary;
        }
    }
}