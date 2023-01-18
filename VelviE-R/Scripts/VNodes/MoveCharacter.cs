using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace VelvieR
{
    [System.Serializable]
    public enum CharacterMoveType
    {
        To,
        Swap,
        MoveToFront,
        SendToBack
    }

    [VTag("VCharacter/MoveCharacter", "Move character to certain stages.", VColor.Magenta, "Ms")]
    public class MoveCharacter : VBlockCore
    {
        [SerializeField, HideInInspector] private VCharacterV character;
        [SerializeField, HideInInspector] private VCharacterV characterToSwap;
        [SerializeField, HideInInspector] private CharacterMoveType moveType;
        [SerializeField, HideInInspector] private VStageUtil mainStage;
        [SerializeField, HideInInspector] private VStageClass fromStage;
        [SerializeField, HideInInspector] private VStageClass toStage;
        [SerializeField, HideInInspector] private float duration = 1f;
        [SerializeField, HideInInspector] private LeanTweenType easeType = LeanTweenType.easeInOutQuad;
        [SerializeField, HideInInspector] private bool waitUntilFinished = false;
        [SerializeField, HideInInspector] private bool is2D = true;
        public bool WaitUntilFinished{get{return waitUntilFinished;} set{waitUntilFinished = value;}}
        public bool Is2D {get{return is2D;} set{value = is2D;}}
        public VCharacterV Character{get{return character;} set{character = value;}}
        public VCharacterV CharacterToSwap{get{return characterToSwap;} set{characterToSwap = value;}}
        public VStageClass FromStage{get{return fromStage;} set{fromStage = value;}}
        public VStageClass ToStage{get{return toStage;} set{toStage = value;}}
        public CharacterMoveType MoveType{get{return moveType;} set{moveType = value;}}
        public LeanTweenType EaseType{get{return easeType;} set{easeType = value;}}
        public VStageUtil MainStage {get{return mainStage;} set{mainStage = value;}}
        public float Duration{get{return duration;} set{duration = value;}}

        public override void OnVEnter()
        {
            if(character == null)
            {
                throw new Exception("Character has not been spawned! Use SetPortrait to spawn the character first!");
            }

            Canvas.ForceUpdateCanvases();
            
            if(moveType == CharacterMoveType.To)
            {
                VStageClass pos = null;
                //if null, gets previous position
                if(fromStage == null)
                {
                    if(!String.IsNullOrEmpty(character.lastStagePosition))
                    {
                        pos = VCharacterManager.GetLastStage(character);                        
                    }
                    else
                    {
                        #if UNITY_EDITOR
                        Debug.Log("Character has no last stage position!");
                        #endif
                    }
                }
                else
                {
                    if(pos != null)
                    {
                        VCharacterManager.MoveVCharacter(character, mainStage, pos, toStage, duration, easeType, setOnComplete: waitUntilFinished, OnCompleteAction:()=> OnVContinue());
                    }
                    else
                    {
                        VCharacterManager.MoveVCharacter(character, mainStage, fromStage, toStage, duration, easeType, setOnComplete: waitUntilFinished, OnCompleteAction:()=> OnVContinue());
                    }
                }
            }
            else if(moveType == CharacterMoveType.Swap)
            {
                if(characterToSwap != null)
                {
                    VCharacterManager.SwapVCharacter(character, characterToSwap, mainStage, duration, easeType, waitUntilFinished, ()=> OnVContinue());
                }
            }
            else if(moveType == CharacterMoveType.SendToBack)
            {
                VCharacterManager.SendToBack(character);                
            }
            else if(moveType == CharacterMoveType.MoveToFront)
            {
                VCharacterManager.MoveToFront(character);                
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

            if(character == null)
            {
                summary += "No character is selected!";
            }

            if(moveType == CharacterMoveType.Swap && characterToSwap == null)
            {
                summary += " :: " + "Character to swap can't be empty!";
            }

            return summary;
        }
    }
}