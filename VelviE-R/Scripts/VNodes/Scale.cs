using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("Animation/Scale", "Scales gameObject via Vector3 or target transform.", VColor.Green01, "Sl")]
    public class Scale : LeanAbstract
    {
        public override void OnVEnter()
        {
            if(targetobject != null)
            {
                if(enableOnStart)
                {
                    targetobject.SetActive(true);
                }

                if (target != null)
                {
                    to = target.localScale;
                }

                var act = new Action(()=>
                {
                    if(waitUntilFinished)
                    {
                        OnVContinue();
                    }

                    if(disableOnComplete)
                    {
                        targetobject.SetActive(false);
                    }

                    if(LeanManager.ActiveTweens.Contains(targetobject))
                        LeanManager.ActiveTweens.Remove(targetobject);
                });

                if(!LeanManager.ActiveTweens.Contains(targetobject))
                LeanManager.ActiveTweens.Add(targetobject);
                VExtension.VTweenScale(targetobject, to, duration, loopCount, true, act, loopType, ease);
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

            if(targetobject == null)
            {
                summary += "Object to scale can't be empty!";
            }

            return summary;
        }
    }
}