using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace VelvieR
{
    [VTag("UIUX/FadeUI", "Fades Unity UI over period of time.\nFadeFrom and FadeTo min/max values are 0/1, if pingPong is toggled off linear mode will be used.", VColor.Pink01, "St")]
    public class FadeUI : VBlockCore
    {
        [SerializeField, HideInInspector] public RectTransform rect;
        [SerializeField, HideInInspector] public float fadeFrom = 0f;
        [SerializeField, HideInInspector] public float fadeTo = 1f;
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public int loopCount = 0;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        [SerializeField, HideInInspector] public bool disableOnComplete = false;
        [SerializeField, HideInInspector] public bool enableOnStart = false;
        [SerializeField, HideInInspector] public bool pingPong = false;
        [SerializeField, HideInInspector] public LeanTweenType ease = LeanTweenType.easeInOutQuad;
        public override void OnVEnter()
        {
            if(rect != null)
            {
                if(enableOnStart)
                {
                    rect.gameObject.SetActive(true);
                }

                var loopType = LeanTweenType.pingPong;

                if(!pingPong)
                {
                    loopType = LeanTweenType.clamp;
                }

                try
                {
                    LeanTween.alpha(rect, fadeTo, duration).setFrom(fadeFrom).setLoopCount(0).setLoopType(loopType).setEase(ease).setOnComplete(()=>
                    {
                        if(disableOnComplete)
                        {
                            rect.gameObject.SetActive(false);
                        }

                        if(waitUntilFinished)
                        {
                            OnVContinue();
                        }
                    });
                }
                catch(Exception)
                {
                    throw new Exception("VError: Probably not a UI!");
                }
            }
        }
        public override void OnVExit()
        {
            if(!waitUntilFinished)
                OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(rect == null)
            {
                summary += "UI component must be assigned!";
            }

            return summary;
        }
    }
}