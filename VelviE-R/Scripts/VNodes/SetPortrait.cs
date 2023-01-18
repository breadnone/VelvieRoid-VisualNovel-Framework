using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("VCharacter/SetPortrait", "Changes character's portrait", VColor.Yellow02, "Cp")]
    public class SetPortrait : VBlockCore
    {
        [SerializeField, HideInInspector] private VCharacterV character;
        [SerializeField, HideInInspector] private PortraitProps activePortrait;
        [SerializeField, HideInInspector] private bool waitUntilFinished;
        [SerializeField, HideInInspector] private float duration = 0.2f;
        [SerializeField, HideInInspector] private VStageUtil activeStage;
        public VStageUtil ActiveVStage {get{return activeStage;} set{activeStage = value;}}
        public VCharacterV Character{get{return character;} set{character = value;}}
        public PortraitProps ActivePortrait{get{return activePortrait;} set{activePortrait = value;}}
        public bool WaitUntilFinished{get{return waitUntilFinished;} set{waitUntilFinished = value;}}
        public float Duration {get{return duration;} set{duration = value;}}
        public override void OnVEnter()
        {
            if(character != null && activePortrait != null)
            {
                VCharacterManager.ShowVPortrait(activeStage, duration, character, activePortrait, waitUntilFinished, ()=> OnVContinue());
            }
        }

        public override void OnVExit()
        {
            if(!waitUntilFinished)
            {
                OnVContinue();
            }
        }
    }
}