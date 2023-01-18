using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace VelvieR
{
    [VTag("Events/Timer", "The result will be displayed on a Text component.", VColor.Magenta, "Ti")]
    public class Timer : VBlockCore
    {
        [SerializeField, HideInInspector] public TMP_Text text;
        [SerializeField, HideInInspector] public float start = 10f;
        [SerializeField, HideInInspector] public float end = 0f;
        [SerializeField, HideInInspector] public string format = "0.00";
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        private bool isValid = false;
        private bool forward = false;
        public void StopTimer()
        {
            isValid = false;
        }

        void Update()
        {
            if (isValid)
            {
                if (!forward)
                {
                    start -= Time.deltaTime;
                }
                else
                {
                    start += Time.deltaTime;
                }

                text.SetText(start.ToString(format));

                if (FisEqual(start, end, 0.001f))
                {
                    isValid = false;

                    if (waitUntilFinished)
                    {
                        OnVContinue();
                    }
                }
            }
        }
        public bool FisEqual(float a, float b, float threshold)
        {
            return Mathf.Abs(a - b) < threshold;
        }
        public override void OnVEnter()
        {
            if (text != null)
            {
                if (start > end)
                {
                    forward = false;
                }
                else
                {
                    forward = true;
                }

                isValid = true;
            }
        }
        public override void OnVExit()
        {
            if (!waitUntilFinished)
                OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (text == null)
            {
                summary += "Text component can't be empty!";
            }

            return summary;
        }
    }
}