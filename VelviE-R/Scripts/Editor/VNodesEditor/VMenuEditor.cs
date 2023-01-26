using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using VelvieR;
using System;

namespace VIEditor
{
    [CustomEditor(typeof(VMenu))]
    public class VMenuEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as VMenu;

            root.style.flexDirection = FlexDirection.Column;
            root.Add(DrawMenu(t));
            root.Add(DrawMainText(t));
            root.Add(DrawCustomMenus(t));
            root.Add(DrawRandomBool(t));
            root.Add(DrawContinueBool(t));
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private Box DrawMenu(VMenu t)
        {
            var menus = VEditorFunc.EditorGetVMenuUtils();
            var asList = new List<string>();
            Array.ForEach(menus, x => asList.Add(x.VmenuName));
            var root = VUITemplate.VDropDownFieldTemplate(asList, "Menu : ", true);

            if (t.Vmenu == null)
            {
                root.child.value = asList.Find(x => x == "<None>");
            }
            else
            {
                if (!Array.Exists(menus, x => x.VmenuId == t.Vmenu.VmenuId))
                {
                    root.child.value = asList.Find(x => x == "<None>");
                    t.Vmenu = null;
                }
                else
                {
                    root.child.value = asList.Find(x => x == t.Vmenu.VmenuName);
                }
            }

            root.child.RegisterValueChangedCallback((xx) =>
            {
                t.Vmenu = Array.Find(menus, x => x.VmenuName == xx.newValue);
            });

            return root.root;
        }
        private Box DrawMainText(VMenu t)
        {
            var root = VUITemplate.GetTemplate("Text : ");
            var tmp = root.userData as VisualElement;
            var obj = new TextField();
            tmp.Add(obj);
            obj.style.flexDirection = FlexDirection.Column;
            obj.style.overflow = Overflow.Visible;
            obj.style.whiteSpace = WhiteSpace.Normal;
            obj.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            obj.style.height = 50;
            obj.value = t.MainText;
            obj.multiline = true;
            obj.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            if (!PortsUtils.PlayMode)
            {
                obj.RegisterCallback<FocusOutEvent>((x) =>
                {
                    t.MainText = obj.value;
                });
            }

            return root;
        }
        private Box DrawRandomBool(VMenu t)
        {
            var box = new Box();
            box.style.marginTop = 5;
            box.style.marginBottom = 5;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.text = "Randomize : ";
            lbl.style.width = 120;
            box.Add(lbl);

            var obj = new Toggle();
            obj.style.width = 230;
            box.Add(obj);
            obj.value = t.Randomize;

            if (!PortsUtils.PlayMode)
            {
                obj.RegisterValueChangedCallback((x) =>
                {
                    t.Randomize = obj.value;
                });
            }

            return box;
        }
        private Box DrawContinueBool(VMenu t)
        {
            var box = new Box();
            box.style.marginTop = 5;
            box.style.marginBottom = 5;
            box.style.flexDirection = FlexDirection.Row;

            var lbl = new Label();
            lbl.text = "Continue this block : ";
            lbl.style.width = 120;
            box.Add(lbl);

            var obj = new Toggle();
            obj.style.width = 230;
            box.Add(obj);
            obj.value = t.ContinueThisBlock;

            if (!PortsUtils.PlayMode)
            {
                obj.RegisterValueChangedCallback((x) =>
                {
                    t.ContinueThisBlock = obj.value;
                });
            }

            return box;
        }
        private Box DrawCustomMenus(VMenu t)
        {
            var box = new Box();
            box.style.marginTop = 5;
            box.style.marginBottom = 5;
            box.style.flexDirection = FlexDirection.Column;
            box.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.style.marginLeft = 5;
            lbl.style.marginTop = 5;
            lbl.style.marginBottom = 5;
            lbl.text = "<b>ADD MENU : </b>";
            lbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            box.Add(lbl);

            Func<VisualElement> makeItem = () =>
            {
                var lbl = new VisualElement();
                lbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                lbl.style.flexDirection = FlexDirection.Column;

                //////////////////////////
                var boxMenutext = new Box();
                boxMenutext.style.marginTop = 5;
                boxMenutext.style.marginBottom = 5;
                boxMenutext.style.marginLeft = 10;
                boxMenutext.style.flexDirection = FlexDirection.Row;

                var lblName = new Label();
                lblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lblName.text = "Menu text : ";

                var lblObj = new TextField();
                lblObj.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                lblObj.name = "menuObj";
                boxMenutext.Add(lblName);
                boxMenutext.Add(lblObj);
                lbl.Add(boxMenutext);
                //////////////////////////

                var boxMenuGraph = new Box();
                boxMenuGraph.style.marginLeft = 10;
                boxMenuGraph.style.flexDirection = FlexDirection.Row;

                var lbllblGr = new Label();
                lbllblGr.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lbllblGr.text = "VGraph : ";

                var lbllblggr = new DropdownField();
                lbllblggr.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                lbllblggr.name = "menuGraph";
                boxMenuGraph.Add(lbllblGr);
                boxMenuGraph.Add(lbllblggr);
                lbl.Add(boxMenuGraph);
                //////////////////////////

                var boxMenuNode = new Box();
                boxMenuNode.style.marginLeft = 10;
                boxMenuNode.style.flexDirection = FlexDirection.Row;

                var lblVnodeName = new Label();
                lblVnodeName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lblVnodeName.text = "VNode : ";

                var lblVnode = new DropdownField();
                lblVnode.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                lblVnode.name = "menuNode";
                boxMenuNode.Add(lblVnodeName);
                boxMenuNode.Add(lblVnode);
                lbl.Add(boxMenuNode);
                //////////////////////////

                var boxMenuLbl = new Box();
                boxMenuLbl.style.marginLeft = 10;
                boxMenuLbl.style.flexDirection = FlexDirection.Row;

                var lbllblName = new Label();
                lbllblName.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lbllblName.text = "Jump to label : ";

                var lbllbl = new TextField();
                lbllbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                lbllbl.name = "menuJump";
                lbllbl.userData = boxMenuLbl as Box;
                boxMenuLbl.Add(lbllblName);
                boxMenuLbl.Add(lbllbl);
                lbl.Add(boxMenuLbl);
                //////////////////////////

                var boxMeneEx = new Box();
                boxMeneEx.style.marginLeft = 10;
                boxMeneEx.style.flexDirection = FlexDirection.Row;

                var lbllblEx = new Label();
                lbllblEx.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lbllblEx.text = "Exclude after : ";

                var lbllblExx = new DropdownField();
                lbllblExx.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                lbllblExx.name = "menuExclude";
                boxMeneEx.Add(lbllblEx);
                boxMeneEx.Add(lbllblExx);
                lbl.Add(boxMeneEx);
                //////////////////////////
                return lbl;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                if (t.VmenuPools[i] == null)
                {
                    t.VmenuPools[i] = new VMenuPools();
                    t.VmenuPools[i].id = Guid.NewGuid().ToString();
                }

                RePoolJumpsAndNodes(e, i, t);
            };

            const int itemHeight = 120;
            var obj = new ListView(t.VmenuPools, itemHeight, makeItem, bindItem);
            obj.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            obj.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            box.Add(obj);

            var btnContainer = new Box();
            btnContainer.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            btnContainer.style.flexDirection = FlexDirection.Column;

            var btnAdd = new Button { text = "ADD" };
            btnAdd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            btnAdd.style.height = 20;

            if (!PortsUtils.PlayMode)
            {
                btnAdd.clicked += () =>
                {
                    t.VmenuPools.Add(new VMenuPools());
                    obj.Rebuild();
                };
            }
            var btnRem = new Button { text = "REMOVE" };
            btnRem.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            btnRem.style.height = 20;
            if (!PortsUtils.PlayMode)
            {
                btnRem.clicked += () =>
                {
                    t.VmenuPools.RemoveAt(obj.selectedIndex);
                    obj.Rebuild();
                };
            }
            btnContainer.Add(btnAdd);
            btnContainer.Add(btnRem);
            box.Add(btnContainer);

            return box;
        }

        private void RePoolJumpsAndNodes(VisualElement e, int i, VMenu t)
        {
            foreach (var parent in e.Children())
            {
                foreach (var child in parent.Children())
                {
                    if (child.name == "menuObj")
                    {
                        var txt = child as TextField;
                        txt.value = t.VmenuPools[i].menuText;

                        txt.RegisterValueChangedCallback((x) =>
                        {
                            t.VmenuPools[i].menuText = txt.value;
                        });
                    }
                    else if (child.name == "menuJump")
                    {
                        var txt = child as TextField;

                        if (t.VmenuPools[i].vgraph == null)
                        {
                            parent.SetEnabled(false);
                            t.VmenuPools[i].jumpToLabel = string.Empty;
                        }
                        else
                        {
                            parent.SetEnabled(true);
                        }

                        txt.value = t.VmenuPools[i].jumpToLabel;

                        txt.RegisterValueChangedCallback((x) =>
                        {
                            t.VmenuPools[i].jumpToLabel = txt.value;
                        });
                    }
                    else if (child.name == "menuNode")
                    {
                        if (t.VmenuPools[i].vgraph == null)
                        {
                            t.VmenuPools[i].vnode = string.Empty;
                            t.VmenuPools[i].vnodeId = string.Empty;
                            parent.SetEnabled(false);
                        }
                        else
                        {
                            parent.SetEnabled(true);
                        }

                        var txt = child as DropdownField;
                        txt.choices = null;

                        var allVnodes = new List<VNodes>();
                        VPortsInstance vport = null;

                        foreach (var port in PortsUtils.VGraph.graphView.nodes)
                        {
                            var asnodes = port as VNodes;
                            allVnodes.Add(asnodes);
                        }

                        if (t.VmenuPools[i].vnode != null)
                        {
                            vport = FindNode(id: t.VmenuPools[i].vnodeId);

                            if (vport != null)
                            {
                                txt.value = t.VmenuPools[i].vnode;
                            }
                            else
                            {
                                txt.value = "<None>";
                                t.VmenuPools[i].vnode = string.Empty;
                                t.VmenuPools[i].vnodeId = string.Empty;
                            }
                        }
                        else
                        {
                            txt.value = "<None>";
                            t.VmenuPools[i].vnode = string.Empty;
                            t.VmenuPools[i].vnodeId = string.Empty;
                        }

                        if (t.VmenuPools[i].vgraph != null)
                        {
                            var asList = new List<string>();
                            var asListId = new List<(string, string)>();
                            allVnodes.ForEach(x =>
                            {
                                var asvp = x.userData as VPortsInstance;
                                asList.Add(asvp.vnodeProperty.nodeName);
                                asListId.Add((asvp.vnodeProperty.nodeName, asvp.vnodeProperty.nodeId));
                            });

                            asList.Add("<None>");
                            txt.choices = asList;

                            if (!PortsUtils.PlayMode)
                            {
                                txt.RegisterCallback<ChangeEvent<string>>((evt) =>
                                {
                                    var getlist = FindNode(name: evt.newValue);
                                    if (getlist != null)
                                    {
                                        t.VmenuPools[i].vnode = getlist.vnodeProperty.nodeName;
                                        t.VmenuPools[i].vnodeId = getlist.vnodeProperty.nodeId;
                                    }
                                    else
                                    {
                                        t.VmenuPools[i].vnode = string.Empty;
                                        t.VmenuPools[i].vnodeId = string.Empty;
                                    }
                                });
                            }
                        }
                    }
                    else if (child.name == "menuExclude")
                    {
                        if (t.VmenuPools[i].vgraph == null)
                        {
                            parent.SetEnabled(false);
                            t.VmenuPools[i].exclude = false;
                        }
                        else
                        {
                            parent.SetEnabled(true);
                        }

                        var asexc = child as DropdownField;
                        asexc.choices.Clear();
                        asexc.choices = new List<string> { "Selected", "None" };

                        if (t.VmenuPools[i].exclude)
                        {
                            asexc.value = asexc.choices[0];
                        }
                        else
                        {
                            asexc.value = asexc.choices[1];
                        }

                        if (!PortsUtils.PlayMode)
                        {
                            asexc.RegisterCallback<ChangeEvent<string>>((evt) =>
                            {
                                if ((string)evt.newValue == "Selected")
                                {
                                    t.VmenuPools[i].exclude = true;
                                }
                                else
                                {
                                    t.VmenuPools[i].exclude = false;
                                }
                            });
                        }
                    }
                    else if (child.name == "menuGraph")
                    {
                        var tb = child as DropdownField;
                        tb.choices.Clear();
                        var instMenu = VEditorFunc.EditorGetVCoreUtils();
                        var asList = new List<string>();

                        Array.ForEach(instMenu, x => asList.Add(x.vcorename));
                        tb.choices = asList;
                        asList.Add("<None>");

                        if (t.VmenuPools[i] != null && t.VmenuPools[i].vgraph != null)
                        {
                            if (!Array.Exists(instMenu, x => x.vcoreid == t.VmenuPools[i].vgraph.vcoreid))
                            {
                                t.VmenuPools[i].vgraph = null;
                                tb.value = "<None>";
                                t.VmenuPools[i].vnode = string.Empty;
                                t.VmenuPools[i].vnodeId = string.Empty;
                            }
                            else
                            {
                                var tmp = t.VmenuPools[i].vgraph.vcoreid.ToString();
                                var str = t.VmenuPools[i].vgraph.vcorename + " - " + tmp[0..2];
                                tb.value = tb.choices.Find(x => x == t.VmenuPools[i].vgraph.vcorename);
                            }
                        }
                        else
                        {
                            t.VmenuPools[i].vgraph = null;
                            tb.value = tb.choices.Find(x => x == "<None>");
                            t.VmenuPools[i].vnode = string.Empty;
                            t.VmenuPools[i].vnodeId = string.Empty;
                        }

                        if (!PortsUtils.PlayMode)
                        {
                            tb.RegisterCallback<ChangeEvent<string>>((evt) =>
                            {
                                if (evt.newValue != "<None>")
                                {
                                    t.VmenuPools[i].vgraph = Array.Find(instMenu, x => x.vcorename == (string)evt.newValue);
                                }
                                else
                                {
                                    t.VmenuPools[i].vnode = string.Empty;
                                    t.VmenuPools[i].vnodeId = string.Empty;
                                }

                                RePoolJumpsAndNodes(e, i, t);
                            });
                        }
                    }
                }
            }
        }
        private VPortsInstance FindNode(string name = null, string id = null)
        {
            foreach (var node in PortsUtils.VGraph.graphView.nodes)
            {
                var astype = (VNodes)node;
                var udat = astype.userData as VPortsInstance;

                if (name != null)
                {
                    if (udat.vnodeProperty.nodeName == name)
                    {
                        return udat;
                    }
                }
                else
                {
                    if (udat.vnodeProperty.nodeId == id)
                    {
                        return udat;
                    }
                }
            }
            return null;
        }
    }
}