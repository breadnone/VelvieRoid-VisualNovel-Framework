using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{

    public class SetVStage : VBlockCore
    {
        [SerializeField, HideInInspector] private VStageUtil setDefaultVstage;
        [SerializeField, HideInInspector] private bool dim = false;
        public VStageUtil SetDefaultVStage{get{return setDefaultVstage;} set{setDefaultVstage = value;}}
        public bool Dim {get{return dim;} set{dim = value;}}
        public override void OnVEnter()
        {
            if(setDefaultVstage != null)
            {
                VCharacterManager.SetActiveStage = setDefaultVstage;
                VStageManager.GetActiveStage().Dim = dim;
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;
            
            if(setDefaultVstage == null)
            {
                summary += "Default VStage can't be empty! IF none in the scene, default will be created on runtime.";
            }

            return summary;
        }
        void Start()
        {

        }


    }
}