using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VelvieR
{
    [VTag("UIUX/SetInteractable", "Sets interactability of a button.", VColor.Pink01, "Sb")]
    public class SetButtonInteractable : VBlockCore
    {
        [SerializeField, HideInInspector] public Button button;
        [SerializeField, HideInInspector] public bool interactable = false;
        public override void OnVEnter()
        {
            button.interactable = interactable;
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(button == null)
            {
                summary += "Button can't be empty!";
            }

            return summary;
        }
    }
}