using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [VTag("Events/SetTimeScale", "Set timeScale value min/max of 0/1.\n\nBecareful using this to pausing the game as it will affect physics, Update and FixedUpdate timing as well.", VColor.Red, "Ts")]
    public class SetTimeScale : VBlockCore
    {
        [SerializeField, HideInInspector] public float value;
        [SerializeField, HideInInspector] public bool resetToDefault = false;
        public override void OnVEnter()
        {
            if(!resetToDefault)
            {
                Time.timeScale = value;
            }
            else
            {
                ResetTimeScale();
            }
        }
        public void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            return summary;
        }
    }
}