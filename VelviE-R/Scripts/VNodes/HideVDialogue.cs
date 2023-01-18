using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace VelvieR
{
    [VTag("VProps/HideVDialog", "Hides active VDialogPanel, if any", VColor.Red)]
    public class HideVDialogue : VBlockCore
    {
        [SerializeField] private VelvieDialogue vdialogue;
        public VelvieDialogue Vdialogue {get{return vdialogue;} set{vdialogue = value;}}

        public override void OnVEnter()
        {
            if(vdialogue != null)
            {
                vdialogue.HideThisVDialogue(true, ()=> OnVContinue());

                if(!String.IsNullOrEmpty(vdialogue.TmpComponent.text))
                    vdialogue.TmpComponent.SetText(string.Empty);
            }            
        }
    }
}