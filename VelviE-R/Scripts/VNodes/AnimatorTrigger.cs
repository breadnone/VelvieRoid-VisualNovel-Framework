using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Animation/AnimatorTrigger", "Triggers animation.", VColor.Green, "At")]
    public class AnimatorTrigger : VBlockCore
    {
        [SerializeField, HideInInspector] public Animator animator;
        [SerializeField, HideInInspector] public string parameter;

        public override void OnVEnter()
        {
            if(!String.IsNullOrEmpty(parameter))
            {
                animator.SetTrigger(parameter);
            }
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

            if (animator == null || String.IsNullOrEmpty(parameter))
            {
                summary += "Animator and parameter can't be empty!";
            }

            return summary;
        }
    }
}