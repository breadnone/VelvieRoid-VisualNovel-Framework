using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Collections.Generic;

namespace VIEditor
{
    [CustomEditor(typeof(SetPortrait))]
    public class SetPortraitEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetPortrait;

            var port = VUITemplate.PortraitTemplate(t.Character);

            if (t.ActivePortrait != null && t.ActivePortrait.portraitSprite != null)
            {
                port.child.value = t.ActivePortrait.portraitSprite.name;
            }
            else
            {
                port.child.value = "<None>";

            }
            if (!PortsUtils.PlayMode)
            {
                port.child.RegisterValueChangedCallback((x) =>
                {
                    if (String.IsNullOrEmpty(x.newValue) || x.newValue == "<None>")
                    {
                        t.ActivePortrait = null;
                        port.child.value = "<None>";
                        imgCon.sprite = null;
                    }
                    else
                    {
                        t.ActivePortrait = t.Character.charaPortrait.Find(xx => xx.portraitSprite.name == x.newValue);

                        if (t.ActivePortrait == null)
                        {
                            Debug.LogWarning("ActivePortrait does not exists!!");
                            return;
                        }

                        port.child.value = x.newValue;

                        if (t.ActivePortrait.portraitSprite != null)
                            imgCon.sprite = t.ActivePortrait.portraitSprite;
                    }

                });
            }
            portraitL = port.child;

            root.Add(DrawCharacter(t));
            root.Add(DrawStage(t));
            root.Add(DrawPortrait(t));
            root.Add(DrawDuration(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private Box DrawCharacter(SetPortrait t)
        {
            var vchar = VUITemplate.CharacterTemplate();

            if (t.Character == null)
            {
                vchar.child.value = "<None>";
            }
            else
            {
                vchar.child.value = t.Character.name;
            }

            if (!PortsUtils.PlayMode)
            {
                vchar.child.RegisterValueChangedCallback((x) =>
                {
                    if (String.IsNullOrEmpty(x.newValue) || x.newValue == "<None>")
                    {
                        t.Character = null;
                        PoolPortraits(t);
                    }
                    else
                    {
                        t.Character = Array.Find(Chars(), xx => xx.character.name == x.newValue).character;

                        if (t.Character == null)
                        {
                            vchar.child.value = "<None>";
                            PoolPortraits(t);
                        }
                        else
                        {
                            PoolPortraits(t);
                        }
                    }

                    PortsUtils.SetActiveAssetDirty();
                    EditorUtility.SetDirty(t);
                });
            }

            return vchar.root;
        }


        private VCharacterUtil[] Chars()
        {
            return Resources.FindObjectsOfTypeAll<VCharacterUtil>();
        }
        private Box DrawStage(SetPortrait t)
        {
            var vstage = VUITemplate.VStageTemplate();

            if (t.ActiveVStage != null)
            {
                vstage.child.value = t.ActiveVStage.vstageName;
            }
            else
            {
                vstage.child.value = "<None>";
            }

            if (!PortsUtils.PlayMode)
            {
                vstage.child.RegisterValueChangedCallback((x) =>
                {
                    if (String.IsNullOrEmpty(x.newValue) || x.newValue == "<None>")
                    {
                        vstage.child.value = "<None>";
                        t.ActiveVStage = null;
                    }
                    else
                    {
                        var getStages = VEditorFunc.EditorGetVStageUtils();
                        t.ActiveVStage = Array.Find(getStages, xx => xx.vstageName == x.newValue);

                        if (t.ActiveVStage != null)
                        {
                            vstage.child.value = t.ActiveVStage.vstageName;
                        }
                        else
                        {
                            vstage.child.value = "<None>";
                        }

                        EditorUtility.SetDirty(t);
                    }

                });
            }
            return vstage.root;
        }
        private Image imgCon;
        private DropdownField portraitL;
        private Box DrawPortrait(SetPortrait t)
        {
            var vis = new Box();
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            vis.style.marginBottom = 10;

            var lblPortraitName = new Label();
            lblPortraitName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblPortraitName.text = "Portrait : ";
            vis.Add(lblPortraitName);

            var imgContainer = new VisualElement();
            imgContainer.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            imgContainer.style.flexDirection = FlexDirection.Column;
            vis.Add(imgContainer);

            var img = new Image();
            imgCon = img;
            img.style.height = 220;
            img.style.width = 190;
            img.style.marginTop = 10;
            img.style.borderBottomWidth = 2;
            img.style.borderLeftWidth = 2;
            img.style.borderTopWidth = 2;
            img.style.borderRightWidth = 2;
            img.style.borderBottomColor = Color.blue;
            img.style.borderTopColor = Color.blue;
            img.style.borderLeftColor = Color.blue;
            img.style.borderRightColor = Color.blue;

            if (t.Character != null)
            {
                if (t.ActivePortrait != null && t.ActivePortrait.portraitSprite != null)
                {
                    portraitL.value = t.ActivePortrait.portraitSprite.name;
                    imgCon.sprite = t.ActivePortrait.portraitSprite;
                }
                else
                {

                }
            }

            imgContainer.Add(portraitL);
            imgContainer.Add(img);

            try
            {
                return vis;
            }
            finally
            {
                PoolPortraits(t);
            }
        }
        private void PoolPortraits(SetPortrait t)
        {
            if (portraitL == null)
                return;

            portraitL.choices.Clear();

            if (t.Character != null && t.Character.charaPortrait.Count > 0)
            {
                var list = new List<string>();
                t.Character.charaPortrait.ForEach(x =>
                {
                    if (x != null && x.portraitSprite != null && !String.IsNullOrEmpty(x.portraitSprite.name))
                        list.Add(x.portraitSprite.name);
                });
                portraitL.choices = list;
                list.Add("<None>");
            }
            else
            {
                portraitL.value = "<None>";
                t.ActivePortrait = null;
            }

            portraitL.MarkDirtyRepaint();
        }
        private VisualElement DrawWait(SetPortrait t)
        {
            var vis = new VisualElement();
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.marginTop = 10;
            vis.style.marginBottom = 10;

            var wait = new Label();
            wait.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            wait.text = "WaitUntilFinished : ";

            var toggle = new Toggle();
            toggle.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            toggle.value = t.WaitUntilFinished;

            if (!PortsUtils.PlayMode)
            {
                toggle.RegisterValueChangedCallback((x) =>
                {
                    t.WaitUntilFinished = toggle.value;
                });
            }
            
            vis.Add(wait);
            vis.Add(toggle);
            return vis;
        }
        private VisualElement DrawDuration(SetPortrait t)
        {
            var vis = new VisualElement();
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.marginTop = 5;
            vis.style.marginBottom = 5;

            var wait = new Label();
            wait.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            wait.text = "Duration : ";

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var floatField = new FloatField();
            visCon.Add(floatField);
            floatField.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            floatField.value = t.Duration;

            if (!PortsUtils.PlayMode)
            {
                floatField.RegisterValueChangedCallback((x) =>
                {
                    t.Duration = floatField.value;
                });
            }

            vis.Add(wait);
            vis.Add(visCon);
            return vis;
        }
    }
}