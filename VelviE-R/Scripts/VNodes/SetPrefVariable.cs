using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace VelvieR
{

    [VTag("Variables/SetPrefVariable", "Sets value of a PlayerPref's key.\nIf none in the PlayerPrefs, a new one will be created.", VColor.Yellow03, "Sp")]
    public class SetPrefVariable : VBlockCore
    {
        [SerializeField] public string key;
        [SerializeField] public AnyTypes anyType = new AnyTypes();
        [SerializeReference] public IVar localVariable;
        [SerializeField] public Button btn;
        public override void OnVEnter()
        {
            if (!String.IsNullOrEmpty(key))
            {
                if (localVariable != null)
                {
                    UPref.SavePlayerPrefs(key, anyType);
                }
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (String.IsNullOrEmpty(key))
            {
                summary += "Key can't be empty!";
            }

            if(localVariable == null)
            {
                summary += "| Local variable can't be empty!";
            }

            return summary;
        }
    }
}