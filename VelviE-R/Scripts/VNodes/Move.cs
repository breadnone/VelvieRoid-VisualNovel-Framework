using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VelvieR
{
    [VTag("Animation/Move", "Moves gameObject via Vector3 or target transform.", VColor.Blue, "Mv")]
    public class Move : LeanAbstract
    {
        [SerializeField, HideInInspector] public bool isLocal = false;
        
        public override void OnVEnter()
        {
            if (targetobject != null)
            {
                if (enableOnStart)
                {
                    targetobject.SetActive(true);
                }
                
                if (target != null)
                {
                    if (isLocal)
                    {
                        to = target.localPosition;
                    }
                    else
                    {
                        to = target.position;
                    }
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
                VExtension.VTweenMove(targetobject, to, duration, loopCount, true, act, loopType, ease, isLocal);
            }
        }
        public override void OnVExit()
        {
            if (!waitUntilFinished)
            {
                OnVContinue();
            }
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (targetobject == null)
            {
                summary += "Object to move can't be empty!";
            }

            return summary;
        }
    }
}