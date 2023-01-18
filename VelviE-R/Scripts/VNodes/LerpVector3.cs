using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace VelvieR
{
    [VTag("Events/LerpVector3", "Lerps Vector3 local variable.\n\nOutput text is optional.", VColor.Green01, "Lv")]
    public class LerpVector3 : VBlockCore
    {
        [SerializeReference, HideInInspector] public IVar variable;
        [SerializeField, HideInInspector] public TMP_Text outputText;
        [SerializeField, HideInInspector] public Vector3 to;
        [SerializeField, HideInInspector] public string format = "0.00";
        [SerializeField, HideInInspector] public float duration = 1f;
        [SerializeField, HideInInspector] public bool waitUntilFinished = false;
        
        public override void OnVEnter()
        {
            if(variable != null)
            {
                LeanTween.value(gameObject, variable.GetVector3(), to, duration).setOnUpdate( (Vector3 val)=>
                {
                    variable.SetVector3(val);

                    if(outputText != null)
                    {
                        outputText.SetText(val.ToString(format));
                    }

                }).setOnComplete(()=>
                {
                    if(waitUntilFinished)
                    {
                        OnVContinue();
                    }
                });
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
            return summary;
        }
    }
}