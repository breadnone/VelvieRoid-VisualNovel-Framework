using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using VelvieR;
using System;
using VIEditor;
using System.Linq;

[CustomEditor(typeof(SayWord))]
public class SayWordEditor : Editor
{
    private VisualElement toggleCharaAudio;
    private VisualElement delay;
    private VisualElement dummySlotProps;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();
        myInspector.style.flexDirection = FlexDirection.Column;
        var t = target as SayWord;

        //Animatableprops here
        dummySlotProps = new VisualElement();
        myInspector.Add(DrawCharacters(t));
        myInspector.Add(DrawSpriteField(t));
        myInspector.Add(DrawThumbnailField(t));
        myInspector.Add(DrawThumbnailEffects(t));
        myInspector.Add(DrawEffectMagnitude(t));
        myInspector.Add(DrawThumbnailLoopCount(t));
        myInspector.Add(dummySlotProps);
        myInspector.Add(DrawWordsField(t));
        myInspector.Add(DrawVDialogue(t));
        myInspector.Add(DrawWaitForClick(t));
        myInspector.Add(DrawDelay(t));

        var visualArray = DrawCharacterSound(t);

        myInspector.Add(visualArray[0]);
        myInspector.Add(visualArray[2]);
        myInspector.Add(visualArray[1]);
        myInspector.Add(DrawAudioClip(t));

        //Always add this at the end!
        VUITemplate.DrawSummary(myInspector, t, () => t.OnVSummary());
        return myInspector;
    }
    public Box DrawWordsField(SayWord t)
    {
        var boxStr = new Box();
        boxStr.style.marginTop = 5;
        boxStr.style.flexDirection = FlexDirection.Row;

        Label strLbl = new Label();
        strLbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strLbl.text = "Words : ";
        boxStr.Add(strLbl);


        var visCon = new VisualElement();
        visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        TextField strContent = new TextField();
        strContent.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        visCon.Add(strContent);

        if (!String.IsNullOrEmpty(t.Words))
        {
            strContent.value = t.Words;
        }

        strContent.style.flexDirection = FlexDirection.Column;
        strContent.style.overflow = Overflow.Visible;
        strContent.style.whiteSpace = WhiteSpace.Normal;
        strContent.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
        strContent.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
        strContent.multiline = true;

        if (!PortsUtils.PlayMode)
        {
            strContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                t.Words = strContent.value;
                EditorUtility.SetDirty(t.gameObject);
            });
        }
        boxStr.Add(visCon);
        return boxStr;
    }
    private DropdownField spriteList;
    public Box DrawThumbnailField(SayWord t)
    {
        var portrait = VUITemplate.PortraitTemplate(t.Character, "Thumbnail : ");
        //Character's Sprites

        if (t.Character == null)
        {
            portrait.root.SetEnabled(false);
        }
        portrait.root.SetEnabled(t.Character != null);
        thumbnailElement = portrait.root;
        thmList = portrait.child;
        RePoolThumbnail(t, portrait.child);
        return portrait.root;
    }
    private DropdownField thmList;
    private void RePoolThumbnail(SayWord t, DropdownField tbMenu)
    {
        if (tbMenu == null)
            return;

        if (t.Character != null && t.Character.charaPortrait.Count > 0)
        {
            if (t.Thumbnail != null && t.Thumbnail.portraitSprite != null && !String.IsNullOrEmpty(t.Thumbnail.portraitSprite.name))
            {
                tbMenu.value = t.Thumbnail.portraitSprite.name;
            }
            else
            {
                tbMenu.value = "<None>";
                t.Thumbnail = null;
                EditorUtility.SetDirty(t.gameObject);
            }
            if (!PortsUtils.PlayMode)
            {
                tbMenu.RegisterValueChangedCallback((evt) =>
                {
                    if (String.IsNullOrEmpty(evt.newValue) || evt.newValue == "<None>")
                    {
                        tbMenu.value = "<None>";
                        t.Thumbnail = null;
                        EnablePortraitBoxes(false, t);
                        EditorUtility.SetDirty(t.gameObject);
                    }
                    else
                    {
                        t.Thumbnail = t.Character.charaPortrait.Find(x => x.portraitSprite.name == evt.newValue);
                        EnablePortraitBoxes(true, t);
                        EditorUtility.SetDirty(t.gameObject);
                    }
                });
            }
            var varlist = new List<string>();

            if (t.Character.charaPortrait.Count > 0)
            {
                for (int i = 0; i < t.Character.charaPortrait.Count; i++)
                {
                    if (t.Character.charaPortrait[i] == null || t.Character.charaPortrait[i].portraitSprite == null)
                        continue;

                    var portrait = t.Character.charaPortrait[i].portraitSprite.name;

                    if (!String.IsNullOrEmpty(portrait))
                        varlist.Add(portrait);
                }
            }

            varlist.Add("<None>");
            tbMenu.choices.Clear();
            tbMenu.choices = varlist;
        }
        else
        {
            tbMenu.value = "<None>";
            t.Thumbnail = null;
            EditorUtility.SetDirty(t.gameObject);
        }

    }
    private VisualElement portraitElements;
    private VisualElement thumbnailElement;
    public Box DrawSpriteField(SayWord t)
    {
        var portrait = VUITemplate.PortraitTemplate(t.Character);
        //Character's Sprites
        if (t.Character == null)
        {
            portrait.root.SetEnabled(false);
        }
        portraitElements = portrait.root;
        portrait.root.SetEnabled(t.Character != null);
        spriteList = portrait.child;
        RePoolPortrait(t, portrait.child);

        return portrait.root;
    }
    private void RePoolPortrait(SayWord t, DropdownField tbMenu)
    {
        if (t.Character != null && t.Character.charaPortrait.Count > 0)
        {
            if (t.Portrait != null && t.Portrait.portraitSprite != null && !String.IsNullOrEmpty(t.Portrait.portraitSprite.name) && t.Character.charaPortrait.Exists(x => x.portraitSprite.name == t.Portrait.portraitSprite.name))
            {
                tbMenu.value = t.Portrait.portraitSprite.name;
            }
            else
            {
                tbMenu.value = "<None>";
                t.Portrait = null;
                EditorUtility.SetDirty(t.gameObject);
            }
            if (!PortsUtils.PlayMode)
            {
                tbMenu.RegisterValueChangedCallback((evt) =>
                {
                    if (String.IsNullOrEmpty(evt.newValue) || evt.newValue == "<None>")
                    {
                        tbMenu.value = "<None>";
                        t.Portrait = null;
                        EnablePortraitBoxes(false, t);
                        EditorUtility.SetDirty(t.gameObject);
                    }
                    else
                    {
                        t.Portrait = t.Character.charaPortrait.Find(x => x.portraitSprite.name == evt.newValue);
                        EnablePortraitBoxes(true, t);
                        EditorUtility.SetDirty(t.gameObject);
                    }
                });
            }
            var varlist = new List<string>();

            if (t.Character.charaPortrait.Count > 0)
            {
                for (int i = 0; i < t.Character.charaPortrait.Count; i++)
                {
                    if (t.Character.charaPortrait[i] == null || t.Character.charaPortrait[i].portraitSprite == null)
                        continue;

                    var portrait = t.Character.charaPortrait[i].portraitSprite.name;

                    if (!String.IsNullOrEmpty(portrait))
                        varlist.Add(portrait);
                }
            }

            varlist.Add("<None>");
            tbMenu.choices.Clear();
            tbMenu.choices = varlist;
        }
        else
        {
            tbMenu.value = "<None>";
            t.Portrait = null;
            EditorUtility.SetDirty(t.gameObject);
        }
    }
    public Box DrawWaitForClick(SayWord t)
    {
        //Wait for click
        var boxEnumWait = new Box();
        boxEnumWait.style.marginTop = 5;
        boxEnumWait.style.flexDirection = FlexDirection.Row;
        boxEnumWait.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        Label waitLbl = new Label();
        waitLbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        waitLbl.text = "Wait for click : ";
        boxEnumWait.Add(waitLbl);

        var visCon = new VisualElement();
        visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var wait = new DropdownField();
        wait.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        visCon.Add(wait);

        List<string> menus = new List<string> { "Enable", "Disable" };
        wait.choices = menus;

        if (!PortsUtils.PlayMode)
        {
            wait.RegisterValueChangedCallback((x) =>
            {
                if (x.newValue == "Enable")
                {
                    t.WaitForClick = WaitForClick.Enable;
                    delay.SetEnabled(false);
                    EditorUtility.SetDirty(t.gameObject);
                }
                else
                {
                    t.WaitForClick = WaitForClick.Disable;
                    delay.SetEnabled(true);
                    EditorUtility.SetDirty(t.gameObject);
                }
            });
        }

        boxEnumWait.Add(visCon);

        if (t.WaitForClick == WaitForClick.Enable)
        {
            wait.value = "Enable";

            if (delay != null)
                delay.SetEnabled(true);
        }
        else
        {
            wait.value = "Disable";

            if (delay != null)
                delay.SetEnabled(false);
        }
        return boxEnumWait;
    }

    public Box[] DrawCharacterSound(SayWord t)
    {
        Box[] bxx = new Box[3];
        //Character sound clip
        var boxCharaSound = new Box();
        boxCharaSound.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        boxCharaSound.style.marginTop = 5;
        boxCharaSound.style.flexDirection = FlexDirection.Row;

        Label strCharaClip = new Label();
        strCharaClip.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strCharaClip.text = "Play character audio : ";
        boxCharaSound.Add(strCharaClip);

        var visCon = new VisualElement();
        visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var auCharaClip = new ObjectField();
        auCharaClip.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        auCharaClip.objectType = typeof(AudioClip);
        visCon.Add(auCharaClip);
        boxCharaSound.Add(visCon);

        if (t.CharacterSound != null)
            auCharaClip.value = t.CharacterSound;

        if (!PortsUtils.PlayMode)
        {
            auCharaClip.RegisterValueChangedCallback((x) =>
            {
                if (auCharaClip.value != null)
                {
                    t.CharacterSound = auCharaClip.value as AudioClip;
                    toggleCharaAudio.SetEnabled(true);
                }
                else
                {
                    t.CharacterSound = null;
                    toggleCharaAudio.SetEnabled(false);
                }
                EditorUtility.SetDirty(t.gameObject);
            });
        }
        bxx[0] = boxCharaSound;

        //LOOP Character sound clip
        var boxLoopCharaSound = new Box();
        toggleCharaAudio = boxLoopCharaSound;
        boxLoopCharaSound.style.marginTop = 5;
        boxLoopCharaSound.style.flexDirection = FlexDirection.Row;

        Label strLoopCharaClip = new Label();
        strLoopCharaClip.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strLoopCharaClip.text = "Loop chara audio : ";
        boxLoopCharaSound.Add(strLoopCharaClip);

        var auLoopCharaClip = new Toggle();
        auLoopCharaClip.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        boxLoopCharaSound.Add(auLoopCharaClip);
        auLoopCharaClip.value = t.LoopCharacterAudio;
        if (!PortsUtils.PlayMode)
        {
            auLoopCharaClip.RegisterValueChangedCallback((x) =>
            {
                t.LoopCharacterAudio = auLoopCharaClip.value;
                EditorUtility.SetDirty(t.gameObject);
            });
        }
        bxx[1] = boxLoopCharaSound;

        if (auCharaClip.value == null)
        {
            boxLoopCharaSound.SetEnabled(false);
        }
        else
        {
            boxLoopCharaSound.SetEnabled(true);
        }

        //Slider Box sound clip
        var boxSlider = new Box();
        boxSlider.style.marginTop = 5;
        boxSlider.style.flexDirection = FlexDirection.Row;

        Label strSlider = new Label();
        strSlider.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strSlider.text = "Chara audio vol : ";
        boxSlider.Add(strSlider);

        var visConSlider = new VisualElement();
        visConSlider.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var slider = new Slider();
        slider.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        slider.lowValue = 0f;
        slider.highValue = 1f;

        visConSlider.Add(slider);

        boxSlider.Add(visConSlider);
        bxx[2] = boxSlider;

        slider.value = t.CharacterSoundVolume;

        if (t.CharacterSound == null)
        {
            boxSlider.SetEnabled(false);
        }
        else
        {
            boxSlider.SetEnabled(true);
        }
        if (!PortsUtils.PlayMode)
        {
            slider.RegisterValueChangedCallback((x) =>
            {
                t.CharacterSoundVolume = slider.value;
                EditorUtility.SetDirty(t.gameObject);
            });
        }
        return bxx;
    }
    private void WarningCheck(SayWord t)
    {
        int currentIndex = PortsUtils.VGraph.listV.selectedIndex;
        PortsUtils.VGraph.RefreshListV();
        PortsUtils.VGraph.listV.SetSelection(currentIndex);
        PortsUtils.VGraph?.ShowSelectedVblockSerializedFields();
    }
    public Box DrawAudioClip(SayWord t)
    {
        //Audio clip
        var boxAu = new Box();
        boxAu.style.marginTop = 5;
        boxAu.style.flexDirection = FlexDirection.Column;

        var boxSub = new Box();
        boxSub.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        boxSub.style.marginTop = 5;
        boxSub.style.flexDirection = FlexDirection.Row;
        boxAu.Add(boxSub);

        Label strAu = new Label();
        strAu.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strAu.text = "Play oneshot audio : ";
        boxSub.Add(strAu);

        var visConOneShot = new VisualElement();
        visConOneShot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var auClip = new ObjectField();
        auClip.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        auClip.objectType = typeof(AudioClip);
        visConOneShot.Add(auClip);
        boxSub.Add(visConOneShot);

        if (t.PlayAudioClip != null)
            auClip.value = t.PlayAudioClip;

        if (!PortsUtils.PlayMode)
        {
            auClip.RegisterValueChangedCallback((x) =>
            {
                if (auClip.value != null)
                    t.PlayAudioClip = auClip.value as AudioClip;
                else
                    t.PlayAudioClip = null;

                EditorUtility.SetDirty(t.gameObject);
            });
        }
        var boxSubTwo = new Box();
        boxSubTwo.style.marginTop = 5;
        boxSubTwo.style.flexDirection = FlexDirection.Row;
        boxAu.Add(boxSubTwo);

        Label lbl = new Label();
        lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        lbl.text = "Custom symbol : ";
        boxSubTwo.Add(lbl);

        var visConSymbol = new VisualElement();
        visConSymbol.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var sym = new TextField();
        sym.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        sym.value = t.CustomSymbols;
        visConSymbol.Add(sym);

        if (!PortsUtils.PlayMode)
        {
            sym.RegisterValueChangedCallback((x) =>
            {
                t.CustomSymbols = sym.value;
            });
        }

        boxSubTwo.Add(visConSymbol);
        return boxAu;
    }
    public Box DrawVDialogue(SayWord t)
    {
        var vdialog = VUITemplate.VDialogTemplate("VDialog : ");
        var vdialogueCom = VEditorFunc.EditorGetVDialogues();

        if (t.VDialogue != null)
        {
            if (Array.Exists(vdialogueCom, x => x.velvieDialogueName == t.VDialogue.velvieDialogueName))
            {
                vdialog.child.value = t.VDialogue.velvieDialogueName;
            }
            else
            {
                vdialog.child.value = "<None>";
                t.VDialogue = null;
                WarningCheck(t);
                EditorUtility.SetDirty(t);
            }
        }
        else
        {
            vdialog.child.value = "<None>";
        }

        if (!PortsUtils.PlayMode)
        {
            vdialog.child.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                if (String.IsNullOrEmpty(evt.newValue) || evt.newValue == "<None>")
                {
                    vdialog.child.value = "<None>";
                    t.VDialogue = null;
                    WarningCheck(t);
                    EditorUtility.SetDirty(t);
                }
                else
                {
                    t.VDialogue = Array.Find(VEditorFunc.EditorGetVDialogues(), x => x.velvieDialogueName == evt.newValue);

                    if (t.VDialogue == null)
                    {
                        vdialog.child.value = "<None>";
                    }

                    WarningCheck(t);
                    EditorUtility.SetDirty(t);
                }
            });
        }

        return vdialog.root;
    }
    public Box DrawDelay(SayWord t)
    {
        var boxEnder = new Box();
        delay = boxEnder;
        boxEnder.style.marginTop = 5;
        boxEnder.style.flexDirection = FlexDirection.Row;

        Label strEnder = new Label();
        strEnder.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strEnder.text = "Ender delay : ";
        boxEnder.Add(strEnder);

        var visConEnder = new VisualElement();
        visConEnder.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var ender = new FloatField();
        ender.style.marginLeft = 4;
        ender.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        ender.value = t.EnderDelay;
        visConEnder.Add(ender);
        boxEnder.Add(visConEnder);
        boxEnder.SetEnabled(false);

        if (!PortsUtils.PlayMode)
        {
            ender.RegisterValueChangedCallback((x) =>
            {
                t.EnderDelay = ender.value;
                EditorUtility.SetDirty(t.gameObject);
            });
        }

        return boxEnder;
    }

    public Box DrawCharacters(SayWord t)
    {
        var vis = VUITemplate.CharacterTemplate();

        if (t.Character != null && !String.IsNullOrEmpty(t.Character.name))
        {
            var getchars = PortsUtils.GetCharacters();

            if (Array.Exists(getchars, x => x.name == t.Character.name))
            {
                vis.child.value = t.Character.name;
            }
            else
            {
                vis.child.value = "<None>";
                t.Character = null;
                t.Portrait = null;
                t.Thumbnail = null;
                RePoolPortrait(t, spriteList);
                RePoolThumbnail(t, thmList);
                thumbnailElement.SetEnabled(false);
                portraitElements.SetEnabled(false);
                EnablePortraitBoxes(false, t);
                thmAnim?.SetEnabled(false);
                thmAnimTb.value = "<None>";
                EditorUtility.SetDirty(t.gameObject);
            }
        }
        else
        {
            vis.child.value = "<None>";
        }

        var vcharas = VEditorFunc.EditorGetVCharacterUtils();

        ComboBox();

        void ComboBox()
        {
            var asList = new List<string>();
            Array.ForEach(vcharas, x => asList.Add(x.character.name));
            asList.Add("<None>");
            vis.child.choices = asList;
            if (!PortsUtils.PlayMode)
            {
                vis.child.RegisterValueChangedCallback((evt) =>
                {
                    if (String.IsNullOrEmpty(evt.newValue) || evt.newValue == "<None>")
                    {
                        vis.child.value = "<None>";
                        Resets(t);
                        RePoolPortrait(t, spriteList);
                        RePoolThumbnail(t, thmList);
                        thumbnailElement.SetEnabled(false);
                        portraitElements.SetEnabled(false);
                        EnablePortraitBoxes(false, t);
                        thmAnim?.SetEnabled(false);
                        EditorUtility.SetDirty(t.gameObject);
                    }
                    else
                    {
                        //Reset first
                        Resets(t);
                        /////

                        t.Character = Array.Find(VEditorFunc.EditorGetVCharacterUtils(), x => x.character.name == evt.newValue).character;
                        RePoolPortrait(t, spriteList);
                        RePoolThumbnail(t, thmList);
                        thumbnailElement.SetEnabled(true);
                        portraitElements.SetEnabled(true);
                        EnablePortraitBoxes(true, t);
                        thmAnim?.SetEnabled(true);
                        EditorUtility.SetDirty(t.gameObject);
                    }
                });
            }
        }


        return vis.root;
    }
    private void Resets(SayWord t)
    {
        t.Character = null;
        t.Portrait = null;
        t.Thumbnail = null;
    }
    private DropdownField thmAnimTb;
    private Box DrawThumbnailEffects(SayWord t)
    {
        var vis = new Box();
        vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        thmAnim = vis;

        if (t.Character == null)
        {
            Resets(t);
            vis.SetEnabled(false);
        }

        vis.style.flexDirection = FlexDirection.Row;

        var lblName = new Label();
        lblName.text = "Thm animation : ";
        lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

        var visCon = new VisualElement();
        visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var objElement = new DropdownField();
        objElement.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        thmAnimTb = objElement;

        visCon.Add(objElement);

        if (t.EffectThm == ThumbnailEffects.Punch)
        {
            objElement.value = "Punch";
            EnablePortraitBoxes(true, t);
            AddRemoveAnimatableProps(t, false);
        }
        else if (t.EffectThm == ThumbnailEffects.None)
        {
            objElement.value = "<None>";
            EnablePortraitBoxes(false, t);
            AddRemoveAnimatableProps(t, false);
        }
        else if (t.EffectThm == ThumbnailEffects.RoundAndRound)
        {
            objElement.value = "RoundAndRound";
            EnablePortraitBoxes(true, t);
            AddRemoveAnimatableProps(t, false);
        }
        else if (t.EffectThm == ThumbnailEffects.BlinkAlpha)
        {
            objElement.value = "BlinkAlpha";
            EnablePortraitBoxes(true, t);
            AddRemoveAnimatableProps(t, false);
        }
        else if (t.EffectThm == ThumbnailEffects.PlayAnimatableProps)
        {
            if (t.Character != null)
            {
                objElement.value = "AnimatableProps";
                EnablePortraitBoxes(true, t);
                AddRemoveAnimatableProps(t, true);
            }
            else
            {
                objElement.value = "<None>";
                t.EffectThm = ThumbnailEffects.None;
            }
        }
        List<string> str = new List<string>();

        foreach (var enumstr in Enum.GetValues(typeof(ThumbnailEffects)))
        {
            var asnumstrtype = (ThumbnailEffects)enumstr;
            str.Add(asnumstrtype.ToString());
        }


        objElement.choices = str;
        if (!PortsUtils.PlayMode)
        {
            objElement.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                if (String.IsNullOrEmpty(evt.newValue) || evt.newValue == "None")
                {
                    objElement.value = "<None>";
                    t.EffectThm = ThumbnailEffects.None;
                    EnablePortraitBoxes(false, t);
                    AddRemoveAnimatableProps(t, false);
                }
                else
                {
                    if (ThumbnailEffects.PlayAnimatableProps.ToString() == evt.newValue)
                    {
                        if (t.Character != null)
                        {
                            t.EffectThm = ThumbnailEffects.PlayAnimatableProps;
                            EnablePortraitBoxes(true, t);
                            AddRemoveAnimatableProps(t, true);
                            EditorUtility.SetDirty(t.gameObject);
                        }
                        else
                        {
                            objElement.value = "<None>";
                            t.EffectThm = ThumbnailEffects.None;
                            EnablePortraitBoxes(false, t);
                            AddRemoveAnimatableProps(t, false);
                            EditorUtility.SetDirty(t.gameObject);
                        }
                    }
                    else
                    {
                        foreach (var nume in Enum.GetValues(typeof(ThumbnailEffects)))
                        {
                            var asnumetype = (ThumbnailEffects)nume;

                            if (asnumetype.ToString() == evt.newValue)
                            {
                                t.EffectThm = asnumetype;
                                t.AnimatableThumbnailProp = null;
                                EnablePortraitBoxes(true, t);
                                AddRemoveAnimatableProps(t, false);
                                EditorUtility.SetDirty(t.gameObject);
                                break;
                            }
                        }

                    }
                }
            });
        }
        vis.Add(lblName);
        vis.Add(visCon);
        return vis;
    }

    private void AddRemoveAnimatableProps(SayWord t, bool state)
    {
        if (dummySlotProps.childCount > 0)
        {
            foreach (var child in dummySlotProps.Children().ToList())
            {
                child.RemoveFromHierarchy();
            }
        }

        if (state)
        {
            dummySlotProps.Add(DrawAnimatableProps(t));
        }
    }
    private Box DrawEffectMagnitude(SayWord t)
    {
        var vis = new Box();
        if (t.Character == null)
        {
            vis.SetEnabled(false);
        }

        magnitudeBox = vis;
        vis.SetEnabled(t.EffectThm != ThumbnailEffects.None);
        vis.style.flexDirection = FlexDirection.Row;

        var lblName = new Label();
        lblName.text = "Thm anim power : ";
        lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

        var visConPower = new VisualElement();
        visConPower.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var objElement = new FloatField();
        objElement.style.marginLeft = 4;
        objElement.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        objElement.value = t.Magnitude;

        visConPower.Add(objElement);
        if (!PortsUtils.PlayMode)
        {
            objElement.RegisterValueChangedCallback((x) =>
            {
                t.Magnitude = objElement.value;
                EditorUtility.SetDirty(t.gameObject);
            });
        }
        vis.Add(lblName);
        vis.Add(visConPower);
        return vis;
    }
    private Box loopCountBox;
    private Box magnitudeBox;
    private Box thmAnim;

    private void EnablePortraitBoxes(bool state, SayWord t)
    {
        if (t.Character != null)
        {
            loopCountBox?.SetEnabled(state);
            magnitudeBox?.SetEnabled(state);

            AddRemoveAnimatableProps(t, true);

            if (t.EffectThm == ThumbnailEffects.PlayAnimatableProps)
            {
                dummySlotProps.SetEnabled(true);
            }
            else
            {
                dummySlotProps.SetEnabled(false);
            }

        }
        else
        {
            loopCountBox?.SetEnabled(false);
            magnitudeBox?.SetEnabled(false);

            if (dummySlotProps.childCount > 0)
            {
                foreach (var child in dummySlotProps.Children().ToList())
                {
                    child.RemoveFromHierarchy();
                }
            }

            dummySlotProps.SetEnabled(false);
        }

    }
    private Box DrawThumbnailLoopCount(SayWord t)
    {
        var vis = new Box();
        if (t.Character == null)
        {
            vis.SetEnabled(false);
        }

        loopCountBox = vis;
        vis.SetEnabled(t.EffectThm != ThumbnailEffects.None);
        vis.style.flexDirection = FlexDirection.Row;

        var lblName = new Label();
        lblName.text = "Loop count : ";
        lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

        var visConCount = new VisualElement();
        visConCount.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var objElement = new IntegerField();
        objElement.style.marginLeft = 4;
        objElement.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        objElement.value = t.LoopCount;

        visConCount.Add(objElement);

        if (!PortsUtils.PlayMode)
        {
            objElement.RegisterValueChangedCallback((x) =>
            {
                t.LoopCount = objElement.value;
                EditorUtility.SetDirty(t.gameObject);
            });
        }

        vis.Add(lblName);
        vis.Add(visConCount);
        return vis;
    }
    private ToolbarMenu animatableTb;
    private VisualElement animatableVis;
    private VisualElement DrawAnimatableProps(SayWord t)
    {
        var rootBox = new Box();
        animatableVis = rootBox;

        if (t.Character == null)
        {
            rootBox.SetEnabled(false);
        }

        rootBox.style.flexDirection = FlexDirection.Column;

        var vis = new Box();
        vis.style.flexDirection = FlexDirection.Row;

        var lblName = new Label();
        lblName.style.marginLeft = 5;
        lblName.text = "AnimatableProps : ";
        lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

        var objElement = new ToolbarMenu();
        animatableTb = objElement;
        objElement.style.marginLeft = 4;
        objElement.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var chars = VEditorFunc.EditorGetVCharacterUtils();

        if (t.AnimatableThumbnailProp != null && !String.IsNullOrEmpty(t.AnimatableThumbnailProp.name))
        {
            objElement.text = t.AnimatableThumbnailProp.name;
        }
        else
        {
            objElement.text = "<None>";
            t.AnimatableThumbnailProp = null;
        }

        objElement.menu.AppendAction("<None>", (x) =>
        {
            t.AnimatableThumbnailProp = null;
            objElement.text = "<None>";
            EditorUtility.SetDirty(t.gameObject);
        });

        foreach (var child in chars)
        {
            if (child.character.name == t.Character.name)
            {
                foreach (var prop in child.animatableThumbnail)
                {
                    //1-2 frames doesn't make sense, thus we limit it to 3
                    if (prop != null && prop.sprites.Count > 2 && !String.IsNullOrEmpty(prop.name))
                    {
                        objElement.menu.AppendAction(prop.name, (x) =>
                        {
                            t.AnimatableThumbnailProp = prop;
                            objElement.text = prop.name;
                            EditorUtility.SetDirty(t.gameObject);
                        });
                    }
                }
            }
        }

        var visTwo = new Box();
        visTwo.style.flexDirection = FlexDirection.Row;

        var lblNameTwo = new Label();
        lblNameTwo.style.marginLeft = 5;
        lblNameTwo.text = "Framerate : ";
        lblNameTwo.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

        var objElementTwo = new IntegerField();
        objElementTwo.style.marginLeft = 4;
        objElementTwo.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        objElementTwo.value = t.FrameRate;

        if (!PortsUtils.PlayMode)
        {
            objElementTwo.RegisterValueChangedCallback((x) =>
            {
                t.FrameRate = objElementTwo.value;
                EditorUtility.SetDirty(t.gameObject);
            });
        }

        var visThree = new Box();
        visThree.style.flexDirection = FlexDirection.Row;

        var lblNameThree = new Label();
        lblNameThree.style.marginLeft = 5;
        lblNameThree.text = "Loop type : ";
        lblNameThree.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

        var objElementThree = new ToolbarMenu();
        objElementThree.style.marginLeft = 4;
        objElementThree.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        if (t.AnimatableThumbnailProp != null && t.AnimatableThumbnailProp.loopClamp)
        {
            objElementThree.text = "Clamp";
        }
        else
        {
            objElementThree.text = "PingPong";
        }

        if (!PortsUtils.PlayMode)
        {
            objElementThree.menu.AppendAction("Clamp", (x) =>
            {
                if (t.AnimatableThumbnailProp != null)
                {
                    objElementThree.text = "Clamp";
                    t.AnimatableThumbnailProp.loopClamp = true;
                    EditorUtility.SetDirty(t.gameObject);
                }
            });
            objElementThree.menu.AppendAction("PingPong", (x) =>
            {
                if (t.AnimatableThumbnailProp != null)
                {
                    objElementThree.text = "PingPong";
                    t.AnimatableThumbnailProp.loopClamp = false;
                    EditorUtility.SetDirty(t.gameObject);
                }
            });
        }

        ////
        var visHumanLike = new Box();
        visHumanLike.style.flexDirection = FlexDirection.Row;

        var lblNameHuman = new Label();
        lblNameHuman.style.marginLeft = 5;
        lblNameHuman.text = "Sync timing : ";
        lblNameHuman.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

        var objElementHuman = new ToolbarMenu();
        objElementHuman.style.marginLeft = 4;
        objElementHuman.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        if (t.HumanLikePause)
        {
            objElementHuman.text = "NaturalPauses";
        }
        else
        {
            objElementHuman.text = "Linear";
        }

        if (!PortsUtils.PlayMode)
        {
            objElementHuman.menu.AppendAction("NaturalPauses", (x) =>
            {
                objElementHuman.text = "NaturalPauses";
                t.HumanLikePause = true;
                EditorUtility.SetDirty(t.gameObject);
            });
            objElementHuman.menu.AppendAction("Linear", (x) =>
            {
                objElementHuman.text = "Linear";
                t.HumanLikePause = false;
                EditorUtility.SetDirty(t.gameObject);
            });
        }
        ///

        var thumbnailSummary = new Box();
        thumbnailSummary.style.backgroundColor = Color.blue;

        var lblThmSummary = new Label();
        lblThmSummary.style.marginTop = 5;
        lblThmSummary.style.marginLeft = 5;
        lblThmSummary.text = "\n<b>NOTE: </b>\nAnimatableProps needs higher framerates and \nmore sprites to work properly.\n\n12 frames and 12 sprite are recommended.\n";
        thumbnailSummary.Add(lblThmSummary);

        visHumanLike.Add(lblNameHuman);
        visHumanLike.Add(objElementHuman);
        visThree.Add(lblNameThree);
        visThree.Add(objElementThree);
        visTwo.Add(lblNameTwo);
        visTwo.Add(objElementTwo);
        vis.Add(lblName);
        vis.Add(objElement);
        rootBox.Add(vis);
        rootBox.Add(visTwo);
        rootBox.Add(visThree);
        rootBox.Add(visHumanLike);
        rootBox.Add(thumbnailSummary);
        return rootBox;
    }
}
