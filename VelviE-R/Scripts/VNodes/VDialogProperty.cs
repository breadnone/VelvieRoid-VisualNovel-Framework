using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("VProps/VDialogProperty", "Change properties of vdialog.", VColor.Yellow03, "Vp")]
    public class VDialogProperty : VBlockCore
    {
        [SerializeField] public VelvieDialogue vdialog;
        [SerializeField] public ShowHideEffect showEffect;
        [SerializeField] public DialogType dialogType;
        [SerializeField] public ContinueAnim continueIndicator;
        [SerializeField] public string writeIndicatorString = string.Empty;
        [SerializeField] public bool enableWritingIndicator;
        [SerializeField] public bool setThisAsdefaultDialog;
        [SerializeField] public VTextSpeed textSpeed;
        public override void OnVEnter()
        {
            if(vdialog != null)
            {
                vdialog.TextEffectType = dialogType;
                vdialog.ContinueIndicator = continueIndicator;
                vdialog.EnableWritingIndicator = enableWritingIndicator;

                if(setThisAsdefaultDialog)
                {
                    VBlockManager.DefaultDialog = vdialog;
                }

                vdialog.ShowEffect = showEffect;

                if(vdialog.WritingIndicatorText != writeIndicatorString)
                {
                    vdialog.WritingIndicatorText = writeIndicatorString;
                }

                vdialog.SpeedText(textSpeed);
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(vdialog == null)
            {
                summary += "Vdialog can't be empty!";
            }

            return summary;
        }
    }
}