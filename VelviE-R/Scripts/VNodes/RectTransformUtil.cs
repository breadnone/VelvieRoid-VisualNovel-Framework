using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Threading.Tasks;
using VTasks;

namespace VelvieR
{
    [System.Serializable]
    public enum RectUtilv
    {
        WidthHeight,
        FlipX,
        FlixY,
        RectIsOverlap,
        None
    }
    [VTag("UIUX/RectTransformUtil", "Rect transfrom utilities.", VColor.Red, "Ru")]
    public class RectTransformUtil : VBlockCore
    {
        [SerializeField, HideInInspector] public RectTransform rect;
        [SerializeField, HideInInspector] public RectTransform rectTarget;
        [SerializeField, HideInInspector] public bool animate = true;
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public Vector3 value;
        [SerializeField, HideInInspector] public RectUtilv rectUtil = RectUtilv.None;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        [SerializeField, HideInInspector] public LeanTweenType ease = LeanTweenType.notUsed;
        [SerializeField, HideInInspector] public bool clamp = true;
        [SerializeField, HideInInspector] public int loopCount = 0;
        [SerializeField, HideInInspector] public bool disableOnComplete;
        [SerializeField, HideInInspector] public IVar vcore;
        [SerializeField, HideInInspector] public string varValue;
        private VTokenSource cts;
        void OnDisable()
        {
            if(cts != null)
            {
                VTokenManager.CancelVToken(cts);
            }
        }
        public override void OnVEnter()
        {
            if (rect != null)
            {
                if (rectUtil == RectUtilv.FlipX)
                {
                    if (animate)
                    {
                        ScaleRect(new Vector3(-1f, rect.localScale.y, rect.localScale.z));
                    }
                    else
                    {
                        rect.localScale = new Vector3(-1f, rect.localScale.y, rect.localScale.z);
                    }
                }
                else if (rectUtil == RectUtilv.FlixY)
                {
                    if (animate)
                    {
                        ScaleRect(new Vector3(rect.localScale.x, -1, rect.localScale.z));
                    }
                    else
                    {
                        rect.localScale = new Vector3(rect.localScale.x, -1f, rect.localScale.z);
                    }
                }
                else if (rectUtil == RectUtilv.WidthHeight)
                {
                    if (animate)
                    {
                        ScaleRect(value);
                    }
                    else
                    {
                        rect.sizeDelta = value;
                    }
                }
                else if (rectUtil == RectUtilv.RectIsOverlap)
                {
                    if(RectIsOverlap())
                    {
                        //TODO
                    }
                }
            }
        }
        private bool RectIsOverlap()
        {
            Rect rect1 = new Rect(rect.localPosition.x, rect.localPosition.y, rect.rect.width, rect.rect.height);
            Rect rect2 = new Rect(rectTarget.localPosition.x, rectTarget.localPosition.y, rectTarget.rect.width, rectTarget.rect.height);
            return rect1.Overlaps(rect2);
        }
        private void ScaleRect(Vector3 vec)
        {
            var loop = LeanTweenType.clamp;

            if(!clamp)
            {
                loop = LeanTweenType.pingPong;
            }

            LeanTween.scale(rect, vec, duration).setOnComplete(()=>
            {
                if(waitUntilFinished)
                {
                    OnVContinue();
                }

                if(disableOnComplete)
                {
                    rect.gameObject.SetActive(false);
                }

            }).setEase(ease).setLoopCount(loopCount).setLoopType(loop);
        }
        public override void OnVExit()
        {
            if (!waitUntilFinished || !animate)
            {
                OnVContinue();
            }
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(rect == null)
            {
                summary += "No RectTransform object can be found!";
            }
            return summary;
        }
    }
}