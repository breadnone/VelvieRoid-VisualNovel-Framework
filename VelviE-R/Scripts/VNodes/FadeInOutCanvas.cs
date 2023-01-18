using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("UIUX/FadeCanvas", "Fades in/out a Canvas object via it's CanvasGroup component.\n\nMake sure, CanvasGroup component is attached to the Canvas.", VColor.Yellow01, "Fc")]
    public class FadeInOutCanvas : VBlockCore
    {
        [SerializeField] public CanvasGroup canvas;
        [SerializeField] public bool fadeIn;
        [SerializeField] public float duration;
        [SerializeField] public bool waitUntilFinished;
        [SerializeField] public bool disableOnComplete = false;
        [SerializeField] public bool enableOnStart = false;
        public override void OnVEnter()
        {
            if(canvas != null)
            {
                if(enableOnStart)
                {
                    canvas.gameObject.SetActive(true);
                }

                FadeInOut();
            }
        }
        private void FadeInOut()
        {
            float val = 1f;

            if(fadeIn)
            {
                canvas.alpha = 0f;
            }
            else
            {
                canvas.alpha = 1f;
                val = 0f;
            }

            LeanTween.alphaCanvas(canvas, val, duration).setOnComplete(()=>
            {
                canvas.alpha = val;

                if(waitUntilFinished)
                {
                    OnVContinue();
                }

                if(disableOnComplete)
                {
                    canvas.gameObject.SetActive(false);
                }
            });
        }
        public override void OnVExit()
        {
            if(!waitUntilFinished)
                OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;
            
            if(canvas == null)
            {
                summary += "Canvas object can't be empty!";
            }

            return summary;
        }
    }
}