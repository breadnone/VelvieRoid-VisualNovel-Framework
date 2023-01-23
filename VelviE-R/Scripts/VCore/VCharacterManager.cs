using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

#if UNITY_EDITOR
using VIEditor;
#endif

namespace VelvieR
{
    public class VCharacterManager : VStageManager
    {
        public static List<VCharacterV> AllCharacters = new List<VCharacterV>();
        public static List<(VCharacterV vchara, PortraitProps previousPortraitProp, int? prevIndex)> ActiveCharacters = new List<(VCharacterV vchara, PortraitProps previousPortraitProp, int? prevIndex)>();
        public static HashSet<VStageClass> AllStages = new HashSet<VStageClass>();
        public static VStageClass dummyStage;
        public static void InsertActiveCharacter(VCharacterV vchara, PortraitProps portrait, int index)
        {
            if (vchara != null && !ActiveCharacters.Exists(x => x.vchara == vchara))
                ActiveCharacters.Add((vchara, portrait, index));
        }
        public static void RemoveActiveCharacter((VCharacterV vchara, PortraitProps previousPortraitProp, int? prevIndex) param)
        {
            ActiveCharacters.Remove(param);
        }
        public static (VCharacterV vchara, PortraitProps previousPortraitProp, int? prevIndex) GetActiveCharacter(VCharacterV vchara)
        {
            return ActiveCharacters.Find(x => x.vchara == vchara);
        }
        public static void ClearActiveCharacters() { ActiveCharacters = new List<(VCharacterV vchara, PortraitProps portraitProp, int? prevIndex)>(); }
        public static List<(VCharacterV vchara, PortraitProps portraitProp, int? prevIndex)> GetActiveCharacters() { return ActiveCharacters; }
        public static bool VCharaIsActive(VCharacterV vchara)
        {
            return ActiveCharacters.Exists(x => x.vchara == vchara);
        }
        public static VStageClass GetStage(VStageClass vstage = null, string stageName = "")
        {
            if (vstage != null)
            {
                return AllStages.ToList().Find(x => x.stageHashId == vstage.stageHashId);
            }

            if (!String.IsNullOrEmpty(stageName))
            {
                return AllStages.ToList().Find(x => x.stageHashId == vstage.stageHashId);
            }

            return null;
        }
        public static VStageClass GetLastStage(VCharacterV vchara)
        {
            if (AllStages.Count == 0)
            {
                throw new Exception("VError: No VStages found in the scene! Create VStage from VGraph menu!");
            }

            var chara = VCharacterManager.GetVCharacter(vchara.charaId);

            foreach (var lastStage in AllStages)
            {
                if (lastStage.name == chara.lastStagePosition)
                {
                    if (lastStage.stageIs2d == chara.is2D)
                    {
                        return lastStage;
                    }
                    else
                    {
                        //TODO 3d stage here
                    }
                }
            }

            if (chara.is2D)
            {
                return AllStages.ToList().Find(x => x.name == "middle");
            }
            else
            {
                //TODO 3d default stage here
            }

            return null;
        }
        public static void SetParentToStage(VCharacterV vchara, VStageClass vstage)
        {
            vchara.root.transform.SetParent(vstage.stageObject.transform.parent.gameObject.transform, false);
        }
        public static VCharacterV GetVCharacter(string vcharaId)
        {
            for (int i = 0; i < VCharacterManager.AllCharacters.Count; i++)
            {
                if (vcharaId.Length == VCharacterManager.AllCharacters[i].charaId.Length && vcharaId == VCharacterManager.AllCharacters[i].charaId)
                {
                    return VCharacterManager.AllCharacters[i];
                }
            }
            return null;
        }
        public static (VCharacterV vchara, PortraitProps portraitProp, int? index) GetVPortrait(string vcharaId, string portraitName)
        {
            for (int i = 0; i < VCharacterManager.AllCharacters.Count; i++)
            {
                var vc = VCharacterManager.AllCharacters[i];

                if (vcharaId.Length == vc.charaId.Length && vcharaId == vc.charaId)
                {
                    for (int j = 0; j < vc.charaPortrait.Count; j++)
                    {
                        var portrait = vc.charaPortrait[j];

                        if (portrait.portraitSprite == null || portrait.portraitSprite.name != portraitName)
                            continue;

                        return (vc, portrait, j);
                    }
                }
            }

            return (null, null, null);
        }
        public static void ShowVPortrait(VStageUtil vstage, float duration, VCharacterV vchara = null, PortraitProps portraitProp = null, bool setOnComplete = false, Action act = null)
        {
            //hide active portrait first
            var getPortrait = GetVPortrait(vchara.charaId, portraitProp.portraitSprite.name);

            if (getPortrait.vchara == null || getPortrait.portraitProp == null || getPortrait.vchara.charaPortrait.Count == 0)
                return;

            if (vstage != null)
            {
                vstage = FindVStage(vstage.vstageName, vstage.vstageId);
            }
            else
            {
                //Get/set default if none found.
                if (VStageManager.GetActiveStage(true) == null)
                {
                    if (getPortrait.vchara.is2D)
                    {
                        vstage = VStageManager.GetActiveStage();
                    }
                    else
                    {
                        //TODO Look for 3D stage here
                    }
                }
            }

            if (String.IsNullOrEmpty(getPortrait.vchara.lastStagePosition))
            {
                var activeDialog = VBlockManager.DefaultDialog;

                #if UNITY_EDITOR
                if(activeDialog == null)
                {
                    var getDialogs = Resources.FindObjectsOfTypeAll<VelvieDialogue>();

                    if(getDialogs == null || getDialogs.Length == 0)
                    {
                        var tmpDialog = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load("VProps/Prefab/VDialogPanel"));
                        UnityEditor.PrefabUtility.UnpackPrefabInstance(tmpDialog, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
                        var arTags = UnityEditorInternal.InternalEditorUtility.tags;

                        if(arTags == null || arTags.Length == 0)
                            return;

                        if(!Array.Exists(arTags, x => x == "VelvieRoid"))
                            UnityEditorInternal.InternalEditorUtility.AddTag("VelvieRoid");

                        tmpDialog.tag = "VelvieRoid";
                        activeDialog = tmpDialog.GetComponent<VelvieDialogue>();
                    }
                    else
                    {
                        activeDialog = getDialogs[0];
                    }

                    activeDialog = getDialogs[0];
                    VBlockManager.DefaultDialog = activeDialog;
                }
                #endif

                getPortrait.vchara.lastStageDuration = activeDialog.SpritesDuration;
                duration = activeDialog.SpritesDuration;
                getPortrait.vchara.lastStagePosition = "middle";
                getPortrait.vchara.root.transform.localPosition = vstage.TwoDStage.Find(x => x.name == getPortrait.vchara.lastStagePosition).stageTransform.localPosition;

                if(getPortrait.portraitProp != null)
                {
                    VExtension.SetImageAlpha(0f, getPortrait.portraitProp.portraitObj);
                }
            }

            var prevChara = GetActiveCharacter(getPortrait.vchara);
            var showChaining = LeanTween.sequence();

            if (prevChara.vchara != null)
            {
                if(prevChara.previousPortraitProp.portraitSprite.name == portraitProp.portraitSprite.name)
                    return;

                VExtension.CheckTweensToCancel(prevChara.previousPortraitProp.portraitRect.gameObject);

                if (prevChara.prevIndex.HasValue)
                {
                    showChaining.append(HideVPortrait(0f, duration - 0.01f, prevChara.vchara, false, false, null, prevChara.prevIndex.Value));
                }
            }

            if (getPortrait.vchara.is2D)
            {
                var getNonNullDefaultStage = vstage.TwoDStage.Find(x => x != null && x.name == "middle");

                if (getPortrait.vchara.root.transform.parent != getNonNullDefaultStage.stageObject.transform.parent)
                {
                    VCharacterManager.SetParentToStage(getPortrait.vchara, getNonNullDefaultStage);
                    VCharacterManager.SetDeltaSize(getPortrait.vchara, getNonNullDefaultStage.rect.sizeDelta, false);
                }

                //Always revalidate deltaSizes
                Vector2 sizeDelta = getNonNullDefaultStage.rect.sizeDelta;
                getPortrait.vchara.rootRect.sizeDelta = sizeDelta;
                getPortrait.portraitProp.portraitRect.sizeDelta = sizeDelta;

                showChaining.append(() =>
                {
                    RemoveActiveCharacter(prevChara);
                    getPortrait.vchara.activePortraitIndex = getPortrait.index.Value;
                    getPortrait.vchara.lastStageDuration = duration;
                    InsertActiveCharacter(getPortrait.vchara, getPortrait.portraitProp, getPortrait.index.Value);
                    getPortrait.portraitProp.portraitObj.SetActive(true);
                });

                showChaining.append(HideVPortrait(1f, duration, getPortrait.vchara, true, setOnComplete, act, getPortrait.index.Value));
            }
        }
        public static void SetDeltaSize(VCharacterV vchara, Vector2 vecDelta, bool all = false, PortraitProps portraitProps = null)
        {
            for (int i = 0; i < vchara.charaPortrait.Count; i++)
            {
                if (all)
                {
                    var portrait = vchara.charaPortrait[i];

                    if (portrait != null && portrait.portraitObj != null && portrait.portraitSprite != null)
                    {
                        portrait.portraitRect.sizeDelta = vecDelta;
                    }
                }
                else
                {
                    if (portraitProps == null)
                        return;

                    if (vchara.charaPortrait[i] == portraitProps)
                    {
                        portraitProps.portraitRect.sizeDelta = vecDelta;
                        break;
                    }
                }
            }
        }
        public static LTDescr HideVPortrait(float to, float duration, VCharacterV vchara = null, bool setactive = false, bool setOnComplete = false, Action act = null, int? currentIndex = null)
        {
            return LeanTween.alpha(vchara.charaPortrait[currentIndex.Value].portraitRect, to, duration).setOnComplete(() =>
            {
                vchara.charaPortrait[vchara.activePortraitIndex].portraitObj.SetActive(setactive);

                if (setOnComplete && act != null)
                    act.Invoke();
            });
        }
        public static void CancelPortraitTweens(VCharacterV vchara, bool executeOnComplete = true)
        {
            for (int i = 0; i < vchara.charaPortrait.Count; i++)
            {
                if (vchara.charaPortrait[i].portraitObj != null)
                {
                    LeanTween.cancel(vchara.charaPortrait[i].portraitObj, executeOnComplete);
                }
            }
        }

        public static void DimPortrait(VCharacterV vchara, bool dim, float duration = 0.2f)
        {
            if (vchara == null)
                return;

            Color colorFrom = Color.white;
            Color colorTo = Color.white;

            if (dim)
            {
                colorFrom = Color.white;
            }
            else
            {
                colorFrom = new Color(0.5f, 0.5f, 0.5f, 1f);
            }

            if (vchara.activePortraitIndex != -1)
            {
                SetImageColor(vchara.charaPortrait[vchara.activePortraitIndex].portraitObj, colorFrom);

                if (vchara.dimmed == true)
                {
                    vchara.dimmed = false;
                    LeanTween.color(vchara.charaPortrait[vchara.activePortraitIndex].portraitRect, colorTo, duration);
                }
            }

            SetAllDimmed(!dim, vchara);
        }
        public static void SetAllDimmed(bool dimmed, VCharacterV excludeThisChara = null, float duration = 0.2f)
        {
            for (int i = 0; i < ActiveCharacters.Count; i++)
            {
                if (excludeThisChara != null && ActiveCharacters[i].vchara == excludeThisChara)
                    continue;

                if (excludeThisChara == null)
                {
                    VExtension.CheckTweensToCancel(ActiveCharacters[i].vchara.charaPortrait[ActiveCharacters[i].vchara.activePortraitIndex].portraitRect.gameObject);
                }
                else
                {
                    ActiveCharacters[i].vchara.dimmed = dimmed;
                }

                if (ActiveCharacters[i].vchara.dimmed == dimmed)
                {
                    LeanTween.color(ActiveCharacters[i].vchara.charaPortrait[ActiveCharacters[i].vchara.activePortraitIndex].portraitRect, new Color(0.5f, 0.5f, 0.5f, 1f), duration);
                }
                else
                {
                    LeanTween.color(ActiveCharacters[i].vchara.charaPortrait[ActiveCharacters[i].vchara.activePortraitIndex].portraitRect, Color.white, duration);
                }
            }
        }
        public static void SetImageColor(GameObject obj, Color col)
        {
            obj.GetComponent<Image>().color = col;
        }
        public static AnimThumbnailProps GetAnimProps(VCharacterV vchara, string propName)
        {
            if (vchara == null || String.IsNullOrEmpty(propName))
                return null;

            var props = GetVCharacter(vchara.charaId).vcharacterUtil.animatableThumbnail.Find(x => x.name == propName);

            if (props != null)
                return props;

            return null;
        }
    }
}