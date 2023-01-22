using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;


namespace VelvieR
{
    [System.Serializable]
    public enum XYZAxis
    {
        X,
        Y,
        Z,
        W,
        None
    }
    public class VStageManager : MonoBehaviour
    {
        public static List<VStageClass> vstages = new List<VStageClass>();
        private static VStageUtil activeStage;
        public static VStageUtil SetActiveStage{private get{return activeStage;} set{activeStage = value;}}
        private static List<VStageUtil> rootVstages = new List<VStageUtil>();
        public static VStageUtil GetActiveStage(bool checkExist = false)
        {
            if(checkExist)
            {
                return activeStage;
            }
            
            if(activeStage == null)
            {
                return VCharacterManager.FindVStage(null, null, true);
            }
            else
            {
                return activeStage;
            }
        }

        public static void InsertRootVStage(VStageUtil rootStage)
        {
            if (!rootVstages.Contains(rootStage))
            {
                rootVstages.Add(rootStage);
            }
        }
        public static VStageUtil FindVStage(string stageName, string vstageId, bool findDefaultFirst = false)
        {
            if(findDefaultFirst)
            {
                if(rootVstages.Count > 0)
                    return rootVstages[0];
                else
                    return null;
            }

            VStageUtil stage = null;
            stage = rootVstages.Find(x => x.vstageName == stageName);
            return stage;
        }
        //Use this on scene changes only!
        public static void CancelAllCharacterTweens(bool execOnComplete = false)
        {
            if (vstages.Count == 0)
                return;

            foreach (var chars in VCharacterManager.AllCharacters)
            {
                if (chars == null)
                    continue;

                VExtension.CheckTweensToCancel(chars.root, execOnComplete);
            }
        }
        public static void FlipVCharacter(VCharacterV character, float value, float duration, LeanTweenType easeType, XYZAxis axis, bool setOnComplete = false, Action OnCompleteAction = null)
        {
            Vector3 oriValue = character.root.transform.localScale;
            Vector3 changedVal = Vector3.zero;

            if (axis == XYZAxis.X)
            {
                changedVal = new Vector3(value, oriValue.y, oriValue.z);
            }
            else if (axis == XYZAxis.Y)
            {
                changedVal = new Vector3(oriValue.x, value, oriValue.z);
            }
            else if (axis == XYZAxis.Z)
            {
                changedVal = new Vector3(oriValue.x, oriValue.y, value);
            }

            LeanTween.scale(character.root, changedVal, duration).setEase(easeType).setFrom(Vector3.one).setOnComplete(() =>
            {
                character.root.transform.localScale = oriValue;

                if (setOnComplete && OnCompleteAction != null)
                {
                    OnCompleteAction.Invoke();
                }
            });
        }
        public static void MoveVCharacter(VCharacterV character, VStageUtil vstage, VStageClass from, VStageClass to, float duration, LeanTweenType easeType, bool setOnComplete = false, Action OnCompleteAction = null)
        {
            var vchara = VCharacterManager.GetVCharacter(character.charaId);

            if(vstage != null)
            {
                vstage = VCharacterManager.FindVStage(vstage.vstageName, vstage.vstageId);
            }
            else
            {
                vstage = VCharacterManager.GetActiveStage();
            }

            VStageManager.SetActiveStage = vstage;
            VStageClass fstage = VCharacterManager.GetStage(from);
            VStageClass tstage = VCharacterManager.GetStage(to);

            if(fstage == null)
            {
                //Get previous if any!
                var prevStage = VCharacterManager.GetLastStage(character);
                fstage = prevStage;

                if(prevStage == null)
                {
                    fstage = VCharacterManager.GetActiveStage().TwoDStage.Find(x => x.name == "middle");
                } 
            }

            Vector3 fromvstage = fstage.stageTransform.localPosition;
            Vector3 tovstage = tstage.stageTransform.localPosition;
            vchara.lastStagePosition = tstage.name;    
            VExtension.CheckTweensToCancel(vchara.root);

            if (vchara.is2D)
            {
                if (vchara.root.transform.parent != fstage.stageObject.transform.parent)
                {
                    VCharacterManager.SetParentToStage(vchara, fstage);
                    VCharacterManager.SetDeltaSize(vchara, fstage.rect.sizeDelta, false);
                }
            }

            if(!vchara.root.activeInHierarchy)
            {
                vchara.root.SetActive(true);
            }

            LeanTween.moveLocal(vchara.root, tovstage, duration).setEase(easeType).setFrom(fromvstage).setOnComplete(() =>
            {
                vchara.root.transform.localPosition = tovstage;

                if (setOnComplete && OnCompleteAction != null)
                {
                    OnCompleteAction.Invoke();
                }
            });
        }
        public static void SwapVCharacter(VCharacterV characterFrom, VCharacterV characterTo, VStageUtil vstage, float duration, LeanTweenType easeType, bool setOnComplete = false, Action OnCompleteAction = null)
        {
            var vcharaFrom = VCharacterManager.GetVCharacter(characterFrom.charaId);
            var vcharaTo = VCharacterManager.GetVCharacter(characterTo.charaId);

            VStageClass fromStagePos = VCharacterManager.GetLastStage(characterFrom);
            VStageClass toStagePos =  VCharacterManager.GetLastStage(characterTo);
            
            vcharaFrom.lastStagePosition = toStagePos.name;
            vcharaTo.lastStagePosition = fromStagePos.name;

            if(vstage != null)
            {
                vstage = VCharacterManager.FindVStage(vstage.vstageName, vstage.vstageId);
            }
            else
            {
                vstage = VCharacterManager.GetActiveStage();
            }

            if(vcharaFrom.is2D)
            {
                fromStagePos = vstage.TwoDStage.Find(x => x.name == fromStagePos.name);
                toStagePos = vstage.TwoDStage.Find(x => x.name == toStagePos.name);
            }

            Vector3 to = toStagePos.stageTransform.localPosition;
            Vector3 from = fromStagePos.stageTransform.localPosition;

            VExtension.CheckTweensToCancel(vcharaFrom.root);
            VExtension.CheckTweensToCancel(vcharaTo.root);

            LeanTween.moveLocal(vcharaFrom.root, to, duration).setEase(easeType).setOnComplete(() =>
            {
                vcharaFrom.root.transform.localPosition = to;
            });

            LeanTween.moveLocal(vcharaTo.root, from, duration).setEase(easeType).setOnComplete(() =>
            {
                vcharaTo.root.transform.localPosition = from;

                if (setOnComplete && OnCompleteAction != null)
                {
                    OnCompleteAction.Invoke();
                }
            });
        }

        public static void SendToBack(VCharacterV character)
        {
            var prevChara = VCharacterManager.GetVCharacter(character.charaId);

            if(prevChara == null)            
                return;
            
            var props = prevChara.charaPortrait.Find(x => x.portraitObj.activeInHierarchy);

            if(props == null)
                return;

            VExtension.CheckTweensToCancel(props.portraitObj);            
            float duration = 0.3f;

            if(VBlockManager.DefaultDialog != null)
                duration = VBlockManager.DefaultDialog.SpritesDuration;
            
            var ltseq = LeanTween.sequence();
            ltseq.append(LeanTween.alpha(props.portraitRect, 0f, duration));
            ltseq.append(()=> character.root.transform.SetAsFirstSibling());
            ltseq.append(LeanTween.alpha(props.portraitRect, 1f, duration));
        }
        public static void MoveToFront(VCharacterV character)
        {
            var prevChara = VCharacterManager.GetVCharacter(character.charaId);

            if(prevChara == null)            
                return;
            
            var props = prevChara.charaPortrait.Find(x => x.portraitObj.activeInHierarchy);

            if(props == null)
                return;

            VExtension.CheckTweensToCancel(props.portraitObj);
            float duration = 0.3f;

            if(VBlockManager.DefaultDialog != null)
                duration = VBlockManager.DefaultDialog.SpritesDuration;
            
            var ltseq = LeanTween.sequence();
            ltseq.append(LeanTween.alpha(props.portraitRect, 0f, duration));
            ltseq.append(()=> character.root.transform.SetAsLastSibling());
            ltseq.append(LeanTween.alpha(props.portraitRect, 1f, duration));
        }
    }
}