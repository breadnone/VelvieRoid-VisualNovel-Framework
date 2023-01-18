using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace VelvieR
{
    [VTag("Animation/MoveSpline", "Spline curves object movements.", VColor.Yellow02, "Mp")]
    public class MoveSpline : LeanAbstract
    {
        [SerializeField, HideInInspector] public bool isLocal = false;
        [SerializeField, HideInInspector] public List<Vector3> splinePoint = new List<Vector3>(1);
        [SerializeField, HideInInspector] public bool orientToPath = true;
        [SerializeField, HideInInspector] public float delay = 1f;
        
        public override void OnVEnter()
        {
            if (targetobject != null && splinePoint.Count > 0)
            {
                if (enableOnStart)
                {
                    targetobject.SetActive(true);
                }

                LTSpline ltSpline = new LTSpline(splinePoint.ToArray());

                if(!LeanManager.ActiveTweens.Contains(targetobject))
                    LeanManager.ActiveTweens.Add(targetobject);

                if(isLocal)
                {
                    LeanTween.moveSpline(targetobject, ltSpline, duration).setOrientToPath(orientToPath).setMoveSplineLocal().setDelay(delay).setEase(ease).setOnComplete(()=>
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
                }
                else
                {
                    LeanTween.moveSpline(targetobject, ltSpline, duration).setOrientToPath(orientToPath).setDelay(delay).setEase(ease).setOnComplete(()=>
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
                }

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

            if(splinePoint.Count == 0)
            {
                summary += "| Spline point can't be empty!";
            }

            return summary;
        }
    }
}