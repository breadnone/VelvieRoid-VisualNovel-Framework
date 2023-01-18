using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VTasks;
using System.Threading.Tasks;
using System.Threading;

namespace VelvieR
{
    [System.Serializable]
    public enum AnimatorParamType
    {
        Int,
        Float,
        Bool
    }
    [VTag("Animation/AnimatorSetParam", "Triggers animation.", VColor.Green, "Sp")]
    public class AnimatorSetParam : VBlockCore
    {
        [SerializeField, HideInInspector] public Animator animator;
        [SerializeField, HideInInspector] public AnimatorParamType paramType = AnimatorParamType.Int;
        [SerializeField, HideInInspector] public string paramStr;
        [SerializeField, HideInInspector] public bool boolvalue;
        [SerializeField, HideInInspector] public float floatvalue;
        [SerializeField, HideInInspector] public int intvalue;
        public override void OnVEnter()
        {
            if(paramType == AnimatorParamType.Int)
            {
                animator.SetInteger(paramStr, intvalue);
            }
            else if(paramType == AnimatorParamType.Float)
            {
                animator.SetFloat(paramStr, floatvalue);
            }
            else
            {
                animator.SetBool(paramStr, boolvalue);
            }
        }

        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (animator == null || String.IsNullOrEmpty(paramStr))
            {
                summary += "Animator can't be empty!";
            }

            return summary;
        }
    }
}