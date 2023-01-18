using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Animation/IsAnimatorPlaying", "Check if animator is playing or animating.\nReturns boolean.", VColor.Green, "Ip")]
    public class IsAnimatorPlaying : VBlockCore
    {
        [SerializeField, HideInInspector] public Animator animator;
        [SerializeField, HideInInspector] public IVar variable;

        public override void OnVEnter()
        {
            variable.SetBool(isPlaying());
        }
        private bool isPlaying()
        {
            return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (animator == null)
            {
                summary += "Animator can't be empty!";
            }

            return summary;
        }
    }
}