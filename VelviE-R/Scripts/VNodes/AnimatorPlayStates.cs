using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Animation/AnimatorPlayStates", "Play animation layer. e.g: SomeLayer.Bounce", VColor.Green, "Ps")]
    public class AnimatorPlayStates : VBlockCore
    {
        [SerializeField, HideInInspector] public Animator animator;
        [SerializeField, HideInInspector] public string layerName;
        [SerializeField, HideInInspector] public string stateName;
        [SerializeField, HideInInspector] public float weight = 1;
        [SerializeField, HideInInspector] public float speedMultiplier = 0;
        [SerializeField, HideInInspector] public bool playInFixedTime = false;
        [SerializeField, HideInInspector] public bool stop;
        [SerializeField, HideInInspector] public bool interpolate = false;
        [SerializeField, HideInInspector] public float interpolateDuration = 0.3f;
        [SerializeField, HideInInspector] public bool resetDefaultSpeed = false;

        public override void OnVEnter()
        {
            if(resetDefaultSpeed)
            {
                animator.speed = 1;
            }

            var getlayer = animator.GetLayerIndex(layerName);
            animator.speed += speedMultiplier;

            if(!stop)
            {                
                if(!playInFixedTime)
                {
                    animator.Play(stateName, getlayer);
                }
                else
                {
                    animator.PlayInFixedTime(stateName, getlayer);
                }

                if(!interpolate)
                {
                    animator.SetLayerWeight(getlayer, weight);
                }
                else
                {
                    LeanTween.value(animator.gameObject, 0f, weight, interpolateDuration).setOnUpdate((float val)=>
                    {
                        animator.SetLayerWeight(getlayer, val);
                    });
                }
            }
            else
            {
                animator.SetLayerWeight(getlayer, 0);
                animator.Rebind();
            }
        }

        public override void OnVExit()
        {
            OnVContinue();
        }

        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (animator == null || String.IsNullOrEmpty(layerName))
            {
                summary += "Animator and layer value can't be empty!";
            }

            return summary;
        }
    }
}