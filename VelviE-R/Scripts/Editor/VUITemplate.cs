using System.Collections.Generic;
using UnityEngine;
using VelvieR;
using UnityEngine.UIElements;
using System.Linq;
using System;

//NOTE: 
//Common templates
//Most scene callbacks will refresh most of the stuff here!.
/////

namespace VIEditor
{
    [System.Serializable]
    public class ActivePortraitEditor
    {
        public DropdownField dropD;
        public VCharacterV vchara;
    }
    [System.Serializable]
    public class ActiveVariableClass
    {
        public DropdownField vfield;
        public VTypes vtype = VTypes.None;
    }
    public static class VUITemplate
    {
        public static List<DropdownField> ActiveMenuDialog = new List<DropdownField>();
        public static List<ActivePortraitEditor> ActivePortrait = new List<ActivePortraitEditor>();
        public static List<ActiveVariableClass> ActiveVariable = new List<ActiveVariableClass>();
        public static List<DropdownField> ActiveCharacter = new List<DropdownField>();
        public static List<DropdownField> ActiveStage = new List<DropdownField>();
        public static List<DropdownField> ActiveDialog = new List<DropdownField>();
        public static List<DropdownField> ActiveVgraph = new List<DropdownField>();
        public static List<DropdownField> ActiveVnode = new List<DropdownField>();
        private static Label activeSummary;
        private static VisualElement summaryRoot;
        private static float ticks = 0f;
        public static void RegSummaryCallBack(MouseMoveEvent evt)
        {
            if (!PortsUtils.PlayMode)
            {
                if (ticks + 0.5f > Time.realtimeSinceStartup && PortsUtils.activeVGraphAssets != null && PortsUtils.activeVNode != null)
                    return;

                ticks = Time.realtimeSinceStartup;
                TriggerOnChangedSummary();
            }
        }
        public static void TriggerOnChangedSummary()
        {
            if (activeSummary != null)
            {
                var a = activeSummary.userData as Func<string>;
                var str = a.Invoke();

                if (!String.IsNullOrEmpty(str))
                {
                    summaryRoot.style.display = DisplayStyle.Flex;
                    activeSummary.text = "VError : " + str;
                }
                else
                {
                    summaryRoot.style.display = DisplayStyle.None;
                }
            }
        }
        public static void DrawSummary(VisualElement root, object vclass, Func<string> onSummary, bool summaryCheck = false)
        {
            if (!PortsUtils.PlayMode)
            {
                string attrString = string.Empty;
                var attr = (VTagAttribute)Attribute.GetCustomAttribute((vclass as VBlockCore).GetType(), typeof(VTagAttribute), false);

                if (attr != null)
                {
                    attrString = attr.vtooltip;
                }

                var template = SummaryTemplate(Color.red, content: onSummary.Invoke(), vtooltip: attrString);
                template.child.text = "VError : " + onSummary.Invoke();
                summaryRoot = template.root.userData as VisualElement;

                activeSummary = template.child;
                activeSummary.userData = onSummary as Func<string>;
                root.Add(template.root);
                TriggerOnChangedSummary();
            }
        }
        public static void SetLblWidth(VisualElement vis)
        {
            vis.style.marginBottom = 5;
            vis.style.width = 120;
        }
        public static void SetFieldWidth(VisualElement vis)
        {
            vis.style.marginBottom = 5;
            vis.style.width = 220;
        }
        public static Box GetTemplate(string lblTitle, FlexDirection flexDirection = FlexDirection.Row)
        {
            var root = new Box();
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            root.name = "root";
            root.style.flexDirection = FlexDirection.Row;
            root.style.marginBottom = 5;

            var lbl = new Label();
            lbl.name = "lbl";
            lbl.text = lblTitle;
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));

            var field = new VisualElement();
            field.name = "field";
            field.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            root.userData = field as VisualElement;

            root.Add(lbl);
            root.Add(field);
            return root;
        }
        public static VisualElement GetRoot(string name = "superRoot", FlexDirection flex = FlexDirection.Column)
        {
            var box = new Box();
            box.name = name;
            box.style.flexDirection = flex;
            return box;
        }
        public static Label GetLabel(Box root, string setText = "")
        {
            foreach (var lbl in root.Children())
            {
                if (lbl is Label asLabel && lbl.name == "lbl")
                {
                    if (!String.IsNullOrEmpty(setText))
                    {
                        asLabel.text = setText;
                    }

                    return asLabel;
                }
            }
            return null;
        }
        public static VisualElement GetField(Box root)
        {
            foreach (var fld in root.Children())
            {
                if (fld.name == "field")
                {
                    return fld;
                }
            }
            return null;
        }
        public static (VisualElement root, DropdownField child) LeanTEase(string labelName = "Ease : ")
        {
            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = labelName;

            var vis = new DropdownField();
            vis.name = "ease";
            vis.style.width = 220;

            if (PortsUtils.variable.ivar.Count > 0)
            {
                vis.choices = Enum.GetNames(typeof(LeanTweenType)).ToList();
                vis.choices.Add("<None>");
            }

            subroot.Add(txtLabel);
            subroot.Add(vis);
            return (subroot, vis);
        }
        public static (VisualElement root, DropdownField child, VTypes type) VariableTemplate(string labelName = "Variable : ", VTypes type = VTypes.None)
        {
            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width =  new StyleLength(new Length(40, LengthUnit.Percent));
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = "variable";
            vis.style.width =  new StyleLength(new Length(100, LengthUnit.Percent));
            visCon.Add(vis);

            var addActiveVar = new ActiveVariableClass { vfield = vis, vtype = type };
            ActiveVariable.Add(addActiveVar);

            if (PortsUtils.variable.ivar.Count > 0)
            {
                var varlist = new List<string>();
                PortsUtils.variable.ivar.ForEach((x) =>
                {
                    if (type == VTypes.None)
                    {
                        varlist.Add(x.Name);
                    }
                    else
                    {
                        if (type == VTypes.String)
                        {
                            if (x.GetVtype() == VTypes.String)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Boolean)
                        {
                            if (x.GetVtype() == VTypes.Boolean)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Double)
                        {
                            if (x.GetVtype() == VTypes.Double)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Float)
                        {
                            if (x.GetVtype() == VTypes.Float)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Integer)
                        {
                            if (x.GetVtype() == VTypes.Integer)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.GameObject)
                        {
                            if (x.GetVtype() == VTypes.GameObject)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Transform)
                        {
                            if (x.GetVtype() == VTypes.Transform)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Vector2)
                        {
                            if (x.GetVtype() == VTypes.Vector2)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Vector3)
                        {
                            if (x.GetVtype() == VTypes.Vector3)
                                varlist.Add(x.Name);
                        }
                        else if (type == VTypes.Vector4)
                        {
                            if (x.GetVtype() == VTypes.Vector4)
                                varlist.Add(x.Name);
                        }
                    }
                });

                varlist.Add("<None>");
                vis.choices = varlist;
            }
            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActiveVariable.Remove(addActiveVar);
                });
            }

            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis, type);
        }

        public static (Box root, DropdownField child) CharacterTemplate(string labelName = "Character : ")
        {
            Box subroot = new Box();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = "character";
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            visCon.Add(vis);

            ActiveCharacter.Add(vis);
            subroot.userData = visCon as VisualElement;

            var chars = Resources.FindObjectsOfTypeAll<VCharacterUtil>();

            if (chars.Length > 0)
            {
                var varlist = new List<string>();

                Array.ForEach(chars, x =>
                {
                    varlist.Add(x.character.name);
                });
                varlist.Add("<None>");
                vis.choices = varlist;
            }

            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActiveCharacter.Remove(vis);
                });
            }
            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis);
        }
        public static (Box root, DropdownField child) VDialogTemplate(string labelName = "Vdialog : ")
        {
            Box subroot = new Box();
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = "vdialog";
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            visCon.Add(vis);

            ActiveDialog.Add(vis);
            var chars = Resources.FindObjectsOfTypeAll<VelvieDialogue>();

            if (chars.Length > 0)
            {
                var varlist = new List<string>();
                Array.ForEach(chars, x => { varlist.Add(x.velvieDialogueName); });
                varlist.Add("<None>");
                vis.choices = varlist;
            }
            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActiveDialog.Remove(vis);
                });
            }
            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis);
        }
        public static (Box root, DropdownField child) VStageTemplate(string labelName = "Vstage : ", bool twoDstage = true)
        {
            Box subroot = new Box();
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = "vdialog";
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            visCon.Add(vis);

            ActiveStage.Add(vis);
            var chars = Resources.FindObjectsOfTypeAll<VStageUtil>();

            if (chars.Length > 0)
            {
                var varlist = new List<string>();
                Array.ForEach(chars, x => { varlist.Add(x.vstageName); });
                varlist.Add("<None>");
                vis.choices = varlist;
            }
            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActiveStage.Remove(vis);
                });
            }
            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis);
        }
        public static (VisualElement root, DropdownField child) MenuTemplate(string labelName = "Menu : ")
        {
            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.flexDirection = FlexDirection.Row;
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = "menu";
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            visCon.Add(vis);

            ActiveMenuDialog.Add(vis);
            var chars = Resources.FindObjectsOfTypeAll<VMenuOption>();

            if (chars.Length > 0)
            {
                var varlist = new List<string>();
                Array.ForEach(chars, x => { varlist.Add(x.VmenuName); });
                varlist.Add("<None>");
                vis.choices = varlist;
            }
            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActiveMenuDialog.Remove(vis);
                });
            }
            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis);
        }
        public static (Box root, DropdownField child) VDropDownFieldTemplate( List<string> list, string labelName, bool defaulNone = true)
        {
            Box subroot = new Box();
            subroot.style.marginBottom = 5;
            subroot.style.flexDirection = FlexDirection.Row;
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            txtLabel.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = labelName;
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            visCon.Add(vis);
            vis.choices = list;
            
            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActiveVgraph.Remove(vis);
                });
            }

            if(defaulNone)
                list.Add("<None>");

            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis);
        }
        public static (Box root, DropdownField child) VGraphTemplate(string labelName = "Portrait : ")
        {
            Box subroot = new Box();
            subroot.style.marginBottom = 5;
            subroot.style.flexDirection = FlexDirection.Row;
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = "VGraph";
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            var varlist = new List<string>();

            visCon.Add(vis);

            //pool this
            ActiveVgraph.Add(vis);
            var graphs = Resources.FindObjectsOfTypeAll<VCoreUtil>();

            if (graphs != null && graphs.Length > 0)
            {
                if (graphs.Length > 0)
                {
                    for (int i = 0; i < graphs.Length; i++)
                    {
                        if (graphs[i] == null)
                            continue;

                        var graph = graphs[i].vcorename;

                        if (!String.IsNullOrEmpty(graph))
                            varlist.Add(graph);
                    }
                }
            }
            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActiveVgraph.Remove(vis);
                });
            }
            varlist.Add("<None>");
            vis.choices = varlist;
            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis);
        }
        public static (Box root, DropdownField child) PortraitTemplate(VCharacterV chara, string labelName = "Portrait : ")
        {
            Box subroot = new Box();
            subroot.style.marginBottom = 5;
            subroot.style.flexDirection = FlexDirection.Row;
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = labelName;

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new DropdownField();
            vis.name = "portrait";
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            var varlist = new List<string>();

            visCon.Add(vis);

            //pool this
            var porVar = new ActivePortraitEditor { dropD = vis, vchara = chara };
            ActivePortrait.Add(porVar);

            if (chara != null)
            {
                if (chara.charaPortrait.Count > 0)
                {
                    for (int i = 0; i < chara.charaPortrait.Count; i++)
                    {
                        if (chara.charaPortrait[i] == null || chara.charaPortrait[i].portraitSprite == null)
                            continue;

                        var portrait = chara.charaPortrait[i].portraitSprite.name;

                        if (!String.IsNullOrEmpty(portrait))
                            varlist.Add(portrait);
                    }
                }
            }
            if (!PortsUtils.PlayMode)
            {
                vis.RegisterCallback<DetachFromPanelEvent>((evt) =>
                {
                    ActivePortrait.Remove(porVar);
                });
            }
            varlist.Add("<None>");
            vis.choices = varlist;
            subroot.Add(txtLabel);
            subroot.Add(visCon);
            return (subroot, vis);
        }
        public static (Box root, Label child) SummaryTemplate(StyleColor bcgColor, string labelName = "VLog : ", string content = "", string vtooltip = "", VisualElement summaryCallback = null)
        {
            VisualElement dummyHost = new VisualElement();
            dummyHost.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Box subroot = new Box();
            subroot.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            subroot.name = "summary";
            subroot.style.marginBottom = 5;
            subroot.style.flexDirection = FlexDirection.Column;

            var visConLblTwo = new VisualElement();
            visConLblTwo.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabel = new Label();
            visConLblTwo.Add(txtLabel);
            txtLabel.style.color = Color.white;
            txtLabel.style.backgroundColor = Color.red;
            txtLabel.style.marginTop = 5;
            txtLabel.style.marginLeft = 10;
            txtLabel.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            txtLabel.text = "<b>" + labelName + "</b>";

            var visConLbl = new VisualElement();
            visConLbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var vis = new Label();
            visConLbl.Add(vis);

            vis.text = content;
            vis.style.color = Color.white;
            vis.style.backgroundColor = bcgColor;
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            vis.style.marginTop = 5;
            vis.style.marginLeft = 10;

            var visConLblThree = new VisualElement();
            visConLblThree.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            Label txtLabelTooltip = new Label();
            visConLblThree.Add(txtLabelTooltip);
            txtLabelTooltip.style.marginTop = 20;
            txtLabelTooltip.style.marginLeft = 10;
            txtLabelTooltip.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            txtLabelTooltip.text = "<b>V-Note : </b>";

            var visConLblFour = new VisualElement();
            visConLblFour.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var tooltip = new Label();
            visConLblFour.Add(tooltip);
            tooltip.text = vtooltip;
            tooltip.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            tooltip.style.marginTop = 5;
            tooltip.style.marginLeft = 10;
            tooltip.style.marginBottom = 20;

            //Multiline and line wraping
            vis.style.flexDirection = FlexDirection.Column;
            vis.style.overflow = Overflow.Visible;
            vis.style.whiteSpace = WhiteSpace.Normal;
            vis.style.unityOverflowClipBox = OverflowClipBox.ContentBox;

            //Multiline and line wraping
            tooltip.style.flexDirection = FlexDirection.Column;
            tooltip.style.overflow = Overflow.Visible;
            tooltip.style.whiteSpace = WhiteSpace.Normal;
            tooltip.style.unityOverflowClipBox = OverflowClipBox.ContentBox;

            dummyHost.Add(visConLblTwo);
            dummyHost.Add(visConLbl);
            subroot.userData = dummyHost as VisualElement;
            subroot.Add(dummyHost);

            if (!String.IsNullOrEmpty(vtooltip))
            {
                subroot.Add(visConLblThree);
                subroot.Add(visConLblFour);
            }
            return (subroot, vis);
        }
        public static void RePoolCharacter()
        {
            var chars = Resources.FindObjectsOfTypeAll<VCharacterUtil>();
            var varlist = new List<string>();
            bool init = false;

            foreach (var activeChar in ActiveCharacter.ToList())
            {
                if (activeChar != null)
                {
                    if (chars.Length > 0)
                    {
                        activeChar.choices.Clear();

                        if (!init)
                        {
                            Array.ForEach(chars, x => { varlist.Add(x.character.name); });
                            varlist.Add("<None>");
                            init = true;
                        }
                        activeChar.choices = varlist;
                    }
                }
            }
        }
        public static void RePoolStage()
        {
            var chars = Resources.FindObjectsOfTypeAll<VStageUtil>();
            var varlist = new List<string>();
            bool init = false;

            foreach (var activeStage in ActiveStage.ToList())
            {
                if (activeStage != null)
                {
                    if (chars.Length > 0)
                    {
                        activeStage.choices.Clear();

                        if (!init)
                        {
                            Array.ForEach(chars, x => { varlist.Add(x.vstageName); });
                            varlist.Add("<None>");
                            init = true;
                        }
                        activeStage.choices = varlist;
                    }
                }
            }
        }
        public static void RePoolMenu()
        {
            var chars = Resources.FindObjectsOfTypeAll<VMenuOption>();
            var varlist = new List<string>();
            bool init = false;

            foreach (var activeMenu in ActiveMenuDialog.ToList())
            {
                if (activeMenu == null)
                    continue;

                if (chars.Length > 0)
                {
                    activeMenu.choices.Clear();

                    if (!init)
                    {
                        Array.ForEach(chars, x => { varlist.Add(x.VmenuName); });
                        varlist.Add("<None>");
                        init = true;
                    }
                }

                activeMenu.choices = varlist;
            }
        }
        public static void RePoolVariable()
        {
            var varlist = new List<string>();
            bool init = false;

            foreach (var activeVar in ActiveVariable.ToList())
            {
                if (activeVar.vfield == null)
                    continue;

                if (PortsUtils.variable.ivar.Count > 0)
                {
                    activeVar.vfield.choices.Clear();

                    if (!init)
                    {
                        PortsUtils.variable.ivar.ForEach((x) =>
                        {
                            if (activeVar.vtype == VTypes.None)
                                varlist.Add(x.Name);
                            else
                            {
                                if (activeVar.vtype == x.GetVtype())
                                    varlist.Add(x.Name);
                            }
                        });
                        varlist.Add("<None>");
                        init = true;
                    }

                    activeVar.vfield.choices = varlist;
                }
            }
        }
        public static void RePoolPortrait()
        {
            var varlist = new List<string>();
            bool init = false;

            foreach (var activePor in ActivePortrait.ToList())
            {
                if (activePor == null)
                    continue;

                if (activePor.vchara != null)
                {
                    if (activePor.vchara.charaPortrait.Count > 0)
                    {
                        activePor.dropD.choices.Clear();

                        if (!init)
                        {
                            for (int i = 0; i < activePor.vchara.charaPortrait.Count; i++)
                            {
                                if (activePor.vchara.charaPortrait[i] == null || activePor.vchara.charaPortrait[i].portraitSprite == null)
                                    continue;

                                var portrait = activePor.vchara.charaPortrait[i].portraitSprite.name;

                                if (!String.IsNullOrEmpty(portrait))
                                    varlist.Add(portrait);
                            }

                            init = true;
                        }
                        activePor.dropD.choices = varlist;
                    }
                }
            }
        }
        public static void RePoolDialog()
        {
            var varlist = new List<string>();
            bool init = false;
            var chars = Resources.FindObjectsOfTypeAll<VelvieDialogue>();

            foreach (var dial in ActiveDialog.ToList())
            {
                if (chars.Length > 0)
                {
                    dial.choices.Clear();

                    if (!init)
                    {
                        Array.ForEach(chars, x => { varlist.Add(x.velvieDialogueName); });
                        varlist.Add("<None>");
                        init = true;
                    }

                    dial.choices = varlist;
                }
            }
        }
        public static void RePoolVGraphs()
        {
            var varlist = new List<string>();
            bool init = false;
            var chars = Resources.FindObjectsOfTypeAll<VCoreUtil>();

            foreach (var dial in ActiveVgraph.ToList())
            {
                if (chars.Length > 0)
                {
                    dial.choices.Clear();

                    if (!init)
                    {
                        Array.ForEach(chars, x => { varlist.Add(x.vcorename); });
                        varlist.Add("<None>");
                        init = true;
                    }

                    dial.choices = varlist;
                }
            }
        }
        public static void RePolAll()
        {
            if(PortsUtils.PlayMode)
                return;

            if (ActiveCharacter.Count > 0)
                RePoolCharacter();
            if (ActiveDialog.Count > 0)
                RePoolDialog();
            if (ActiveMenuDialog.Count > 0)
                RePoolMenu();
            if (ActivePortrait.Count > 0)
                RePoolPortrait();
            if (ActiveStage.Count > 0)
                RePoolStage();
            if (ActiveVariable.Count > 0)
                RePoolVariable();
            if(ActiveVgraph.Count > 0)
                RePoolVGraphs();

        }
    }
}