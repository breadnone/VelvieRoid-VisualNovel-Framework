using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace VelvieR
{
    [VTag("Events/LerpFloat", "Lerps float local variable.\n\nOutput text is optional.", VColor.Green01, "Lf")]
    public class LerpFloat : VBlockCore
    {
        [SerializeReference, HideInInspector] public IVar variable;
        [SerializeField, HideInInspector] public TMP_Text outputText;
        [SerializeField, HideInInspector] public float to;
        [SerializeField, HideInInspector] public string format = "0.00";
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        
        public override void OnVEnter()
        {
            if(variable != null)
            {
                LeanTween.value(this.gameObject, SetVal, variable.GetFloat(), to, duration).setOnComplete(()=>
                {
                    if(waitUntilFinished)
                    {
                        OnVContinue();
                    }
                });
            }
        }
        private void SetVal(float val, float ratio)
        {
            variable.SetFloat(val);

            if(outputText != null)
            {
                outputText.SetText(val.ToString(format));
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

            if(variable == null)
            {
                summary += "Variable can't be null!";
            }

            return summary;
        }
    }
}