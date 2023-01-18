using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("VProps/VMenuProperty", "Change vmenu properties.", VColor.Yellow03, "Vp")]
    public class VMenuProperty : VBlockCore
    {
        [SerializeField] public VMenuOption vmenu;
        [SerializeField] public MenuAnimType animType;
        [SerializeField] public bool setAsDefault;
        public override void OnVEnter()
        {
            if(vmenu != null)
            {
                vmenu.AnimType = animType;

                if(setAsDefault)
                {
                    VMenuOption.SetDefaultMenu(vmenu);
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

            if(vmenu == null)
            {
                summary += "Vmenu can't be empty!";
            }
            return summary;
        }
    }
}