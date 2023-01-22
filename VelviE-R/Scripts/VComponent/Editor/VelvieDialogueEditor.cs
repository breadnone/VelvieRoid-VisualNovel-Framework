using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(VelvieDialogue))]
    public class VelvieDialogueEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            Box parentBox = new Box();
            var t = target as VelvieDialogue;

            //TMP_Text component slot
            var tmpBox = new Box();
            tmpBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            tmpBox.style.flexDirection = FlexDirection.Row;

            var TMPfield = new ObjectField();
            TMPfield.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            TMPfield.objectType = typeof(TMP_Text);
            if (!PortsUtils.PlayMode)
            {
                TMPfield.RegisterValueChangedCallback((x) =>
                {
                    t.TmpComponent = TMPfield.value as TMP_Text;
                    EditorUtility.SetDirty(t);
                });
            }
            if (t.TmpComponent != null)
            {
                TMPfield.value = t.TmpComponent;
            }

            var lblTmp = new Label();
            lblTmp.tooltip = "Drag n drop TextMeshpro component here";
            lblTmp.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblTmp.text = "Text Component";
            tmpBox.Add(lblTmp);
            tmpBox.Add(TMPfield);

            //Enum slot writing speed
            var boxSpeed = new Box();
            boxSpeed.style.marginTop = 5;
            boxSpeed.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            boxSpeed.style.flexDirection = FlexDirection.Row;

            var dropDSpeed = new DropdownField();

            if (!PortsUtils.PlayMode)
            {
                dropDSpeed.choices = Enum.GetNames(typeof(VTextSpeed)).ToList();
            }

            dropDSpeed.value = t.TxtSpeed.ToString();
            dropDSpeed.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            if (!PortsUtils.PlayMode)
            {
                dropDSpeed.RegisterValueChangedCallback(x =>
                {
                    var enumss = Enum.GetValues(typeof(VTextSpeed));

                    foreach (var numsVals in enumss)
                    {
                        if (numsVals.ToString() == x.newValue)
                        {
                            t.SpeedText((VTextSpeed)numsVals);
                            EditorUtility.SetDirty(t);
                            break;
                        }
                    }
                });
            }
            //Just a visual representation of the dropdown above

            var lblSpeed = new Label();
            lblSpeed.tooltip = "Choose writing speed";
            lblSpeed.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblSpeed.text = "Writing Speed";
            boxSpeed.Add(lblSpeed);
            boxSpeed.Add(dropDSpeed);

            //Enum effect
            var boxEffect = new Box();
            boxEffect.style.marginTop = 5;
            boxEffect.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            boxEffect.style.flexDirection = FlexDirection.Row;

            var effectEnum = new ToolbarMenu { text = "Type Writer" };
            effectEnum.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            effectEnum.menu.AppendAction("Type Writer", a => { effectEnum.text = "Type Writer"; t.TextEffectType = DialogType.TypeWriter; EditorUtility.SetDirty(t); });
            effectEnum.menu.AppendAction("Gradient Fade", a => { effectEnum.text = "Gradient Fade"; t.TextEffectType = DialogType.GradientFade; EditorUtility.SetDirty(t); });
            effectEnum.menu.AppendAction("Auto Complete", a => { effectEnum.text = "Auto Complete"; t.TextEffectType = DialogType.None; EditorUtility.SetDirty(t); });

            if (t.TextEffectType == DialogType.TypeWriter)
                effectEnum.text = "Type Writer";
            else if (t.TextEffectType == DialogType.GradientFade)
                effectEnum.text = "Gradient Fade";
            else
                effectEnum.text = "Auto Complete";

            var lblEffect = new Label();
            lblEffect.tooltip = "Choose writing effects";
            lblEffect.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblEffect.text = "Text Effect";
            boxEffect.Add(lblEffect);
            boxEffect.Add(effectEnum);

            //Continue effect
            var continueEffect = new Box();
            continueEffect.style.marginTop = 5;
            continueEffect.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            continueEffect.style.flexDirection = FlexDirection.Row;

            //Continue effect
            var sprite = new Box();
            sprite.SetEnabled(false);
            sprite.style.marginTop = 5;
            sprite.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            sprite.style.flexDirection = FlexDirection.Row;

            var spriteIcon = new ObjectField();
            spriteIcon.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            spriteIcon.objectType = typeof(GameObject);
            if (!PortsUtils.PlayMode)
            {
                spriteIcon.RegisterValueChangedCallback((x) =>
                {
                    t.IconSprite = spriteIcon.value as GameObject;
                    EditorUtility.SetDirty(t);
                });
            }
            var lblSprite = new Label();
            lblSprite.tooltip = "Set GameObject";
            lblSprite.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblSprite.text = "Icon GameObject : ";

            sprite.Add(lblSprite);
            sprite.Add(spriteIcon);

            var continueEnum = new ToolbarMenu { text = "Three Dots" };
            continueEnum.style.width = new StyleLength(new Length(60, LengthUnit.Percent));

            if (!PortsUtils.PlayMode)
            {
                continueEnum.menu.AppendAction("Three Dots", a => { sprite.SetEnabled(false); continueEnum.text = "Three Dots"; t.ContinueIndicator = ContinueAnim.ThreeDots; });

                continueEnum.menu.AppendAction("Icon", a =>
                {
                    continueEnum.text = "Icon";
                    t.ContinueIndicator = ContinueAnim.Icon;
                    sprite.SetEnabled(true);
                    EditorUtility.SetDirty(t);
                });

                continueEnum.menu.AppendAction("None", a =>
                {
                    sprite.SetEnabled(false);
                    continueEnum.text = "None";
                    t.ContinueIndicator = ContinueAnim.None;
                    EditorUtility.SetDirty(t);
                });

                continueEnum.menu.AppendAction("Three Dots", a =>
                {
                    sprite.SetEnabled(false);
                    continueEnum.text = "Three Dots";
                    t.ContinueIndicator = ContinueAnim.ThreeDots;
                    EditorUtility.SetDirty(t);
                });
            }
            if (t.ContinueIndicator == ContinueAnim.ThreeDots)
            {
                continueEnum.text = "Three Dots";
            }
            else if (t.ContinueIndicator == ContinueAnim.Icon)
            {
                continueEnum.text = "Icon";
            }
            else
            {
                continueEnum.text = "None";
            }

            var lblContinue = new Label();
            lblContinue.tooltip = "Continue animation indicator";
            lblContinue.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblContinue.text = "Continue Indicator";
            continueEffect.Add(lblContinue);
            continueEffect.Add(continueEnum);

            //AudioClip typing bool
            var auclipConTypingBox = new Box();
            auclipConTypingBox.style.marginTop = 5;
            auclipConTypingBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            auclipConTypingBox.style.flexDirection = FlexDirection.Row;

            var auClipTypeTog = new Toggle();
            auClipTypeTog.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            auClipTypeTog.value = t.EnableTypingSound;
            if (!PortsUtils.PlayMode)
            {
                auClipTypeTog.RegisterValueChangedCallback((x) =>
                {
                    t.EnableTypingSound = auClipTypeTog.value;
                    EditorUtility.SetDirty(t);
                });
            }
            var lblAuConTypeClip = new Label();
            lblAuConTypeClip.tooltip = "Plays audio when typing.";
            lblAuConTypeClip.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblAuConTypeClip.text = "Enable Typing Sound";
            auclipConTypingBox.Add(lblAuConTypeClip);
            auclipConTypingBox.Add(auClipTypeTog);

            //AudioClip component
            var auclipBox = new Box();
            auclipBox.style.marginTop = 5;
            auclipBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            auclipBox.style.flexDirection = FlexDirection.Row;

            var auClip = new ObjectField();
            auClip.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            auClip.objectType = typeof(AudioClip);
            auClip.value = t.TypingSound;
            if (!PortsUtils.PlayMode)
            {
                auClip.RegisterValueChangedCallback((x) =>
                {
                    t.TypingSound = auClip.value as AudioClip;
                    EditorUtility.SetDirty(t);
                });
            }
            var lblAuClip = new Label();
            lblAuClip.tooltip = "Drag n drop audio file here";
            lblAuClip.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblAuClip.text = "Typing Sound";
            auclipBox.Add(lblAuClip);
            auclipBox.Add(auClip);

            //AudioClip ender bool
            var auclipConBoolBox = new Box();
            auclipConBoolBox.style.marginTop = 5;
            auclipConBoolBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            auclipConBoolBox.style.flexDirection = FlexDirection.Row;

            var auClipConTog = new Toggle();
            auClipConTog.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            auClipConTog.value = t.EnableEnderAudio;
            if (!PortsUtils.PlayMode)
            {
                auClipConTog.RegisterValueChangedCallback((x) =>
                {
                    t.EnableEnderAudio = auClipConTog.value;
                    EditorUtility.SetDirty(t);
                });
            }
            var lblAuConBoolClip = new Label();
            lblAuConBoolClip.tooltip = "Plays audio at the end of sentence.";
            lblAuConBoolClip.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblAuConBoolClip.text = "Enable Ender Sound";
            auclipConBoolBox.Add(lblAuConBoolClip);
            auclipConBoolBox.Add(auClipConTog);

            //AudioClip ender 
            var auclipConBox = new Box();
            auclipConBox.style.marginTop = 5;
            auclipConBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            auclipConBox.style.flexDirection = FlexDirection.Row;

            var auClipCon = new ObjectField();
            auClipCon.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            auClipCon.objectType = typeof(AudioClip);
            auClipCon.value = t.EnderDelayAudio;
            if (!PortsUtils.PlayMode)
            {
                auClipCon.RegisterValueChangedCallback((x) =>
                {
                    t.EnderDelayAudio = auClipCon.value as AudioClip;
                    EditorUtility.SetDirty(t);
                });
            }
            var lblAuConClip = new Label();
            lblAuConClip.tooltip = "Drag n drop audio file here";
            lblAuConClip.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblAuConClip.text = "Ender Sound";
            auclipConBox.Add(lblAuConClip);
            auclipConBox.Add(auClipCon);

            //Pause between words slot
            var pauseBox = new Box();
            pauseBox.style.marginTop = 5;
            pauseBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            pauseBox.style.flexDirection = FlexDirection.Row;

            var pause = new FloatField();
            pause.style.width = new StyleLength(new Length(59, LengthUnit.Percent));

            if (!PortsUtils.PlayMode)
            {
                pause.RegisterValueChangedCallback((x) =>
                {
                    t.PauseBetweenWords = pause.value;
                    EditorUtility.SetDirty(t);
                });
            }
            pause.value = t.PauseBetweenWords;

            var lblPause = new Label();
            lblPause.tooltip = "Pauses/delays between words";
            lblPause.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblPause.text = "Delay Between Words";
            pauseBox.Add(lblPause);
            pauseBox.Add(pause);

            //ShowHide effects
            var showHideBox = new Box();
            showHideBox.style.marginTop = 5;
            showHideBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            showHideBox.style.flexDirection = FlexDirection.Row;

            var showHide = new ToolbarMenu { text = "Slide LeftRight" };
            showHide.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            showHide.menu.AppendAction("Fade InOut", a => { showHide.text = "Fade InOut"; t.ShowEffect = ShowHideEffect.FadeInOut; EditorUtility.SetDirty(t); });
            showHide.menu.AppendAction("Zoom InOut", a => { showHide.text = "Zoom InOut"; t.ShowEffect = ShowHideEffect.ZoomInOut; EditorUtility.SetDirty(t); });
            showHide.menu.AppendAction("Slide LeftRight", a => { showHide.text = "Slide LeftRightt"; t.ShowEffect = ShowHideEffect.SlideLeftRight; EditorUtility.SetDirty(t); });
            showHide.menu.AppendAction("Slide RightLeft", a => { showHide.text = "Slide RightLeft"; t.ShowEffect = ShowHideEffect.SlideRightLeft; EditorUtility.SetDirty(t); });
            showHide.menu.AppendAction("Slide Up", a => { showHide.text = "Slide up"; t.ShowEffect = ShowHideEffect.SlideUp; EditorUtility.SetDirty(t); });
            showHide.menu.AppendAction("None", a => { showHide.text = "None"; t.ShowEffect = ShowHideEffect.None; EditorUtility.SetDirty(t); });

            if (t.ShowEffect == ShowHideEffect.FadeInOut)
                showHide.text = "Fade InOut";
            else if (t.ShowEffect == ShowHideEffect.ZoomInOut)
                showHide.text = "Zoom InOut";
            else if (t.ShowEffect == ShowHideEffect.SlideLeftRight)
                showHide.text = "Slide LeftRight";
            else if (t.ShowEffect == ShowHideEffect.SlideRightLeft)
                showHide.text = "Slide RightLeft";
            else if (t.ShowEffect == ShowHideEffect.SlideUp)
                showHide.text = "Slide Up";
            else
                showHide.text = "None";

            var lblShowHide = new Label();
            lblShowHide.tooltip = "Type of effects when first enabling/disabling VDialogue";
            lblShowHide.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblShowHide.text = "Show/hide effect";
            showHideBox.Add(lblShowHide);
            showHideBox.Add(showHide);

            //ShowHide duration slot
            var rotationBox = new Box();
            rotationBox.style.marginTop = 5;
            rotationBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            rotationBox.style.flexDirection = FlexDirection.Row;

            var showDuration = new FloatField();
            showDuration.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            showDuration.value = t.ShowEffectDuration;
            if (!PortsUtils.PlayMode)
            {
                showDuration.RegisterValueChangedCallback((x) =>
                {
                    t.ShowEffectDuration = showDuration.value;
                    EditorUtility.SetDirty(t);
                });
            }
            var lblshowDuration = new Label();
            lblshowDuration.tooltip = "show/hide duration";
            lblshowDuration.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblshowDuration.text = "Show/hide duration";
            rotationBox.Add(lblshowDuration);
            rotationBox.Add(showDuration);

            //Write Indicator slot
            var writeInBox = new Box();
            writeInBox.style.marginTop = 5;
            writeInBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            writeInBox.style.flexDirection = FlexDirection.Row;

            var showWriteIn = new Toggle();
            showWriteIn.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            showWriteIn.value = t.EnableWritingIndicator;

            var lblshowIn = new Label();
            lblshowIn.tooltip = "Enable writing indicator";
            lblshowIn.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblshowIn.text = "Enable writing indicator";
            writeInBox.Add(lblshowIn);
            writeInBox.Add(showWriteIn);

            //Write Indicator text slot
            var writeTInBox = new Box();
            writeTInBox.style.marginTop = 5;
            writeTInBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            writeTInBox.style.flexDirection = FlexDirection.Row;

            var showWriteTIn = new TextField();
            showWriteTIn.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            showWriteTIn.value = t.WritingIndicatorText;
            if (!PortsUtils.PlayMode)
            {
                showWriteTIn.RegisterValueChangedCallback((x) =>
                {
                    t.WritingIndicatorText = showWriteTIn.value;
                    EditorUtility.SetDirty(t);
                });
            }
            var lblshowInT = new Label();
            lblshowInT.tooltip = "Writing indicator text";
            lblshowInT.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblshowInT.text = "Writing indicator text";
            writeTInBox.Add(lblshowInT);
            writeTInBox.Add(showWriteTIn);
            if (!PortsUtils.PlayMode)
            {
                showWriteIn.RegisterValueChangedCallback((x) =>
                {
                    writeTInBox.SetEnabled(showWriteIn.value);
                    t.EnableWritingIndicator = showWriteIn.value;
                    EditorUtility.SetDirty(t);

                });
            }
            ///////////////////////
            parentBox.Add(tmpBox);
            parentBox.Add(boxSpeed);
            parentBox.Add(writeInBox);
            parentBox.Add(writeTInBox);
            parentBox.Add(boxEffect);
            parentBox.Add(continueEffect);
            parentBox.Add(sprite);
            parentBox.Add(auclipConTypingBox);
            parentBox.Add(auclipBox);
            parentBox.Add(auclipConBoolBox);
            parentBox.Add(auclipConBox);
            parentBox.Add(pauseBox);
            parentBox.Add(showHideBox);
            parentBox.Add(rotationBox);
            parentBox.Add(DrawPortraitDuration(t));
            parentBox.Add(DrawThumbnailDuration(t));
            parentBox.Add(DrawClickTarget(t));
            return parentBox;
        }
        private Box DrawPortraitDuration(VelvieDialogue t)
        {
            var vis = new Box();
            vis.style.marginTop = 5;
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lblName = new Label();
            lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblName.text = "Portrait fade duration : ";

            var fieldObj = new FloatField();
            fieldObj.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            fieldObj.value = t.SpritesDuration;
            if (!PortsUtils.PlayMode)
            {
                fieldObj.RegisterValueChangedCallback((x) =>
                {
                    t.SpritesDuration = fieldObj.value;
                    EditorUtility.SetDirty(t);
                });
            }
            vis.Add(lblName);
            vis.Add(fieldObj);
            return vis;
        }
        private Box DrawThumbnailDuration(VelvieDialogue t)
        {
            var vis = new Box();
            vis.style.marginTop = 5;
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lblName = new Label();
            lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblName.text = "Thumbnail fade duration : ";

            var fieldObj = new FloatField();
            fieldObj.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            fieldObj.value = t.ThumbnailDuration;
            if (!PortsUtils.PlayMode)
            {
                fieldObj.RegisterValueChangedCallback((x) =>
                {
                    t.ThumbnailDuration = fieldObj.value;
                    EditorUtility.SetDirty(t);
                });
            }
            vis.Add(lblName);
            vis.Add(fieldObj);
            return vis;
        }
        private Box DrawClickTarget(VelvieDialogue t)
        {
            var vis = new Box();
            vis.style.marginTop = 5;
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lblName = new Label();
            lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblName.text = "Click target : ";

            var fieldObj = new DropdownField();
            fieldObj.style.width = new StyleLength(new Length(59, LengthUnit.Percent));
            fieldObj.value = t.ClickTarget.ToString();

            var list = new List<string> { "ClickAnywhere", "ClickOnVDialogPanel" };
            fieldObj.choices = list;
            if (!PortsUtils.PlayMode)
            {
                fieldObj.RegisterCallback<ChangeEvent<string>>((x) =>
                {

                    if (x.newValue == "ClickAnywhere")
                    {
                        t.ClickTarget = ClickOnDialogue.ClickAnywhere;
                    }
                    else
                    {
                        t.ClickTarget = ClickOnDialogue.ClickOnVDialogPanel;
                    }

                    EditorUtility.SetDirty(t);

                });
            }
            vis.Add(lblName);
            vis.Add(fieldObj);
            return vis;
        }
    }
}