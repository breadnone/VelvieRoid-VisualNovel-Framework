using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("Animation/PauseAnimation", "Pauses/resumes active animations on target or all active entities.", VColor.Green01, "Ps")]
    public class PauseAnimation : VBlockCore
    {
        [SerializeField, HideInInspector] public bool isResume = false;
        [SerializeField, HideInInspector] public GameObject targetobject;
        [SerializeField, HideInInspector] public bool pauseAll = false;
        public override void OnVEnter()
        {
            if (!isResume)
            {
                if (pauseAll)
                {
                    LeanManager.PauseAll();
                }
                else if (!pauseAll && targetobject != null)
                {
                    LeanManager.Pause(targetobject);
                }
            }
            else
            {
                if (!pauseAll && targetobject != null)
                {
                    LeanManager.Resume(targetobject);
                }
                else 
                {
                    LeanManager.ResumeAll();
                }
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (targetobject == null && !pauseAll)
            {
                if (!isResume)
                {
                    summary += "Object to pause can't be empty!";
                }
                else
                {
                    summary += "Object to cancel can't be empty!";
                }
            }

            return summary;
        }
    }
}