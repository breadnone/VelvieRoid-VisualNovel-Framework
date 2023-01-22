using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using VelvieR;
using UnityEditor;
using System.Linq;

namespace VIEditor
{
    public class VToolbars
    {
        public VGraphs vgraph { get; set; }
        public List<Toolbar> Toolbars = new List<Toolbar>();
        public Toolbar entityToolbar;
        public List<Label> buttons = new List<Label>();
        public Toolbar mainToolbar { get; set; }
        private StyleColor defaultBorderColor;
        public void ResetButtonList()
        {
            if (buttons != null && buttons.Count > 0)
            {
                for (int i = buttons.Count; i-- > 0;)
                {
                    if (buttons[i] == null)
                        continue;

                    buttons[i].RemoveFromHierarchy();
                    buttons[i].RemoveManipulator(contexts[i]);
                }
            }

            buttons = new List<Label>();
        }
        private List<ContextualMenuManipulator> contexts = new List<ContextualMenuManipulator>();

        private void ButtonSubs(VGraphsContainer vgContainer)
        {
            if (!PortsUtils.PlayMode)
            {
                PortsUtils.LoadAssets(vgContainer, false);
                vgContainer.graphState.InspectorBtnStates = true;
            }
        }
        public void InsertToEntityToolbar(Box box, VGraphsContainer vgContainer)
        {
            var btn = new Label();

            if (String.IsNullOrEmpty(vgContainer.graphState.graphStateId))
            {
                var nuGraph = new GraphStates();
                nuGraph.graphStateId = vgContainer.govcoreid.ToString();
                nuGraph.entityDefaultBackgroundColor = btn.style.backgroundColor;
                vgContainer.graphState = nuGraph;
                EditorUtility.SetDirty(vgContainer);
            }

            if (defaultBorderColor == null)
                defaultBorderColor = btn.style.borderBottomColor;

            btn.style.marginTop = 6;
            btn.style.marginLeft = 3;
            btn.style.unityTextAlign = TextAnchor.MiddleCenter;
            btn.style.backgroundColor = vgContainer.graphState.entityBackgroundColor;
            btn.style.height = 40;
            btn.style.width = 120;
            btn.text = vgContainer.vgraphGOname;
            btn.name = vgContainer.entityIndex.ToString();

            btn.style.flexDirection = FlexDirection.Column;
            btn.style.overflow = Overflow.Visible;
            btn.style.whiteSpace = WhiteSpace.Normal;
            btn.style.unityOverflowClipBox = OverflowClipBox.ContentBox;

            //Clicked state
            btn.userData = vgContainer.graphState as GraphStates;
            if (!PortsUtils.PlayMode)
            {
                btn.RegisterCallback<MouseDownEvent>((evt) => ButtonSubs(vgContainer));
            }

            ContextualMenuManipulator cntx = null;

            if (!PortsUtils.PlayMode)
            {
                cntx = new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {

                evt.menu.AppendAction("Set Color/Red", (x) =>
                {
                    btn.style.backgroundColor = Color.red;
                    vgContainer.graphState.entityBackgroundColor = Color.red;
                    PortsUtils.SetActiveAssetDirty();
                });
                evt.menu.AppendAction("Set Color/Blue", (x) =>
                {
                    btn.style.backgroundColor = Color.blue;
                    vgContainer.graphState.entityBackgroundColor = Color.blue;
                    PortsUtils.SetActiveAssetDirty();
                });
                evt.menu.AppendAction("Set Color/Magenta", (x) =>
                {
                    btn.style.backgroundColor = Color.magenta;
                    vgContainer.graphState.entityBackgroundColor = Color.magenta;
                    PortsUtils.SetActiveAssetDirty();
                });
                evt.menu.AppendAction("Set Color/Grey", (x) =>
                {
                    btn.style.backgroundColor = Color.grey;
                    vgContainer.graphState.entityBackgroundColor = Color.grey;
                    PortsUtils.SetActiveAssetDirty();
                });
                evt.menu.AppendAction("Set Color/Black", (x) =>
                {
                    btn.style.backgroundColor = Color.black;
                    vgContainer.graphState.entityBackgroundColor = Color.black;
                    PortsUtils.SetActiveAssetDirty();
                });
                evt.menu.AppendAction("Set Color/Yellow", (x) =>
                {
                    btn.style.backgroundColor = Color.yellow;
                    vgContainer.graphState.entityBackgroundColor = Color.yellow;
                    PortsUtils.SetActiveAssetDirty();
                });
                evt.menu.AppendAction("Set Color/Green", (x) =>
                {
                    btn.style.backgroundColor = Color.green;
                    vgContainer.graphState.entityBackgroundColor = Color.green;
                    PortsUtils.SetActiveAssetDirty();
                });
                evt.menu.AppendAction("Set Color/White", (x) =>
                {
                    btn.style.backgroundColor = Color.white;
                    vgContainer.graphState.entityBackgroundColor = Color.white;
                    PortsUtils.SetActiveAssetDirty();
                });

                evt.menu.AppendAction("Delete", (x) =>
                {
                    GameObject obj = GameObject.Find(vgContainer.vgraphGOname);

                    if (obj != null)
                        GameObject.DestroyImmediate(obj);

                    box.Remove(btn);
                    PortsUtils.LoadAssets(PortsUtils.activeVGraphAssets, false);
                });

                });
            }

            if (cntx != null)
            {
                contexts.Add(cntx);
                btn.AddManipulator(cntx);
            }

            buttons.Add(btn);
            box.Add(btn);
        }
        public void ButtonSelectedState(Label btn, bool state)
        {
            if (btn != null)
            {
                Color cols;

                int widths = 0;

                if (state)
                {
                    cols = Color.white;
                    widths = 2;
                }
                else
                {
                    cols = Color.black;
                }

                btn.style.borderBottomColor = cols;
                btn.style.borderTopColor = cols;
                btn.style.borderLeftColor = cols;
                btn.style.borderRightColor = cols;
                btn.style.borderBottomWidth = widths;
                btn.style.borderTopWidth = widths;
                btn.style.borderLeftWidth = widths;
                btn.style.borderRightWidth = widths;
            }
        }
        public virtual void InsertToolBar()
        {
            //1st toolbar
            var toolbar = new Toolbar();
            toolbar.pickingMode = PickingMode.Ignore;
            toolbar.style.position = Position.Relative;
            toolbar.style.borderBottomColor = Color.grey;
            toolbar.style.borderBottomWidth = 3;
            toolbar.style.height = 40;
            vgraph = PortsUtils.VGraph;

            mainToolbar = toolbar;

            var t = new Vector2(vgraph.rootVisualElement.resolvedStyle.width / 2, vgraph.rootVisualElement.resolvedStyle.height / 2);
            var toolbarCharacter = new ToolbarMenu { text = "VCharacter" };
            toolbarCharacter.name = "VCharacter";
            toolbar.Add(toolbarCharacter);

            var toolbarVprop = new ToolbarMenu { text = "VProps" };
            toolbarVprop.name = "VProps";
            toolbar.Add(toolbarVprop);

            //Flow
            var toolbarFlow = new ToolbarMenu { text = "Flow", variant = ToolbarMenu.Variant.Default };
            toolbarFlow.name = "Flow";
            toolbar.Add(toolbarFlow);

            //UI
            var toolbarUi = new ToolbarMenu { text = "UIUX", variant = ToolbarMenu.Variant.Default };
            toolbarUi.name = "UIUX";
            toolbar.Add(toolbarUi);

            //Animation
            var toolbarAnim = new ToolbarMenu { text = "Animation", variant = ToolbarMenu.Variant.Default };
            toolbarAnim.name = "Animation";
            toolbar.Add(toolbarAnim);

            //GameObject
            var popupObj = new ToolbarMenu { text = "GameObject", variant = ToolbarMenu.Variant.Default };
            popupObj.name = "GameObject";
            toolbar.Add(popupObj);

            //Events
            var toolbarEvent = new ToolbarMenu { text = "Events", variant = ToolbarMenu.Variant.Default };
            toolbarEvent.name = "Events";
            toolbar.Add(toolbarEvent);

            //Camera
            var toolbarCamera = new ToolbarMenu { text = "Camera", variant = ToolbarMenu.Variant.Default };
            toolbarCamera.name = "Camera";
            toolbar.Add(toolbarCamera);

            //Variables
            var toolbarVariable = new ToolbarMenu { text = "Variables", variant = ToolbarMenu.Variant.Default };
            toolbarVariable.name = "Variables";
            toolbar.Add(toolbarVariable);

            //Collection
            var toolbarGenerics = new ToolbarMenu { text = "Collection", variant = ToolbarMenu.Variant.Default };
            toolbarGenerics.name = "Collection";

            if (!PortsUtils.PlayMode)
            {
                toolbarGenerics.menu.AppendAction("CreateList", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("AddToList", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("RemoveFromList", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("ClearList", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("CreateArray", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("AddToArray", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("ClearArray", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("CreateQueue", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("Queue", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("DeQueue", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("ClearQueue", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("ArrayToList", a => { }, a => DropdownMenuAction.Status.Normal);
                toolbarGenerics.menu.AppendAction("ListToArray", a => { }, a => DropdownMenuAction.Status.Normal);
            }

            toolbar.Add(toolbarGenerics);

            //Math
            var popupMath = new ToolbarMenu { text = "Maths", variant = ToolbarMenu.Variant.Default };
            popupMath.name = "Maths";
            toolbar.Add(popupMath);

            //MultiMedia
            var popupMedia = new ToolbarMenu { text = "Multimedia", variant = ToolbarMenu.Variant.Default };
            popupMedia.name = "Multimedia";
            toolbar.Add(popupMedia);

            //SaveSystem
            var popupSave = new ToolbarMenu { text = "SaveSystem", variant = ToolbarMenu.Variant.Default };
            popupSave.menu.AppendAction("Add Save System", a => { }, a => DropdownMenuAction.Status.Normal);
            toolbar.Add(popupSave);

            //CustomVNodes
            var customVNode = new ToolbarMenu { text = "Custom VNode", variant = ToolbarMenu.Variant.Default };
            customVNode.menu.AppendAction("Sample", a => { });
            toolbar.Add(customVNode);

            //CustomVNodes
            var debug = new ToolbarMenu { text = "Debug", variant = ToolbarMenu.Variant.Default };
            toolbar.Add(debug);

            Toolbars.Add(toolbar);

            //2nd toolbar
            var toolbarTwo = new Toolbar();
            toolbarTwo.pickingMode = PickingMode.Ignore;
            toolbarTwo.style.borderBottomColor = Color.grey;
            toolbarTwo.style.borderBottomWidth = 3;
            toolbarTwo.style.position = Position.Relative;
            toolbarTwo.style.height = 30;
            toolbarTwo.style.backgroundColor = Color.yellow;

            var nodeInspectorBtn = new Button(() => PortsUtils.VGraph.HideInspector());
            nodeInspectorBtn.style.backgroundColor = Color.blue;
            nodeInspectorBtn.text = "Show/Hide Inspector";
            toolbarTwo.Add(nodeInspectorBtn);

            var minimapBtn = new Button(() => PortsUtils.VGraph.EnableMinimap());
            minimapBtn.style.backgroundColor = Color.blue;
            minimapBtn.text = "Enable MiniMap";
            toolbarTwo.Add(minimapBtn);

            var menuTwo = new ToolbarMenu { text = "Create VDialogue" };
            menuTwo.style.backgroundColor = Color.green;
            menuTwo.style.color = Color.black;

            if (!PortsUtils.PlayMode)
            {
                menuTwo.menu.AppendAction("Add VDialogue", a =>
                {
                    GameObject go = PrefabUtility.InstantiatePrefab(Resources.Load("VProps/Prefab/VDialogPanel")) as GameObject;
                    PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                });
            }

            toolbarTwo.Add(menuTwo);
            var popupTwo = new ToolbarMenu { text = "Create VStage", variant = ToolbarMenu.Variant.Default };
            popupTwo.style.backgroundColor = Color.green;
            popupTwo.style.color = Color.black;

            if (!PortsUtils.PlayMode)
            {
                popupTwo.menu.AppendAction("Add VStage", a =>
                {
                    GameObject go = PrefabUtility.InstantiatePrefab(Resources.Load("VProps/Prefab/VStage")) as GameObject;
                    PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }, a => DropdownMenuAction.Status.Normal);
            }

            toolbarTwo.Add(popupTwo);
            var popupMenu = new ToolbarMenu { text = "Create VMenu", variant = ToolbarMenu.Variant.Default };
            popupMenu.style.backgroundColor = Color.green;
            popupMenu.style.color = Color.black;

            if (!PortsUtils.PlayMode)
            {
                popupMenu.menu.AppendAction("Add VMenu", a =>
                {
                    GameObject go = PrefabUtility.InstantiatePrefab(Resources.Load("VProps/Prefab/VMenu")) as GameObject;
                    PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }, a => DropdownMenuAction.Status.Normal);
            }

            toolbarTwo.Add(popupMenu);
            var clickableMenu = new ToolbarMenu { text = "Create Clickables", variant = ToolbarMenu.Variant.Default };
            clickableMenu.style.backgroundColor = Color.green;
            clickableMenu.style.color = Color.black;

            if (!PortsUtils.PlayMode)
            {
                clickableMenu.menu.AppendAction("Add Clickables", a => { }, a => DropdownMenuAction.Status.Normal);
            }

            toolbarTwo.Add(clickableMenu);
            var inventoryMenu = new ToolbarMenu { text = "Create VInventorySystem", variant = ToolbarMenu.Variant.Default };
            inventoryMenu.style.backgroundColor = Color.green;
            inventoryMenu.style.color = Color.black;
            if (!PortsUtils.PlayMode)
            {
                inventoryMenu.menu.AppendAction("Add VInventory", a => { }, a => DropdownMenuAction.Status.Normal);
                inventoryMenu.menu.AppendAction("VInventory List/SomeInventory", a => { }, a => DropdownMenuAction.Status.Normal);
            }

            toolbarTwo.Add(inventoryMenu);
            var questMenu = new ToolbarMenu { text = "Create VQuestSystem", variant = ToolbarMenu.Variant.Default };
            questMenu.style.backgroundColor = Color.green;
            questMenu.style.color = Color.black;

            if (!PortsUtils.PlayMode)
            {
                questMenu.menu.AppendAction("Add VQuestSystem", a => { }, a => DropdownMenuAction.Status.Normal);
                questMenu.menu.AppendAction("VQuestSystem List/SomeQuestSystem", a => { }, a => DropdownMenuAction.Status.Normal);
            }

            toolbarTwo.Add(questMenu);
            Toolbars.Add(toolbarTwo);

            //3rd toolbar
            var toolbarThree = new Toolbar();
            toolbarThree.pickingMode = PickingMode.Ignore;
            toolbarThree.style.borderBottomColor = Color.grey;
            toolbarThree.style.borderBottomWidth = 3;
            toolbarThree.style.position = Position.Relative;
            toolbarThree.style.height = 30;
            toolbarThree.style.backgroundColor = Color.gray;

            var addVNode = new Button(() => VGraphUtil.CreateVGraph());
            addVNode.style.width = 90;
            addVNode.style.backgroundColor = Color.grey;
            addVNode.text = "ADD VGraph";
            toolbarThree.Add(addVNode);

            var addvnodeTool = new Button(AddVNode);
            addvnodeTool.style.width = 60;
            addvnodeTool.style.backgroundColor = Color.grey;
            addvnodeTool.text = "+ VNode";
            toolbarThree.Add(addvnodeTool);

            var delvnodeTool = new Button(() => PortsUtils.VGraph?.graphView?.DeleteSelectedVNodes("singleDeletion"));
            delvnodeTool.style.width = 60;
            delvnodeTool.style.backgroundColor = Color.grey;
            delvnodeTool.text = "- VNode";
            toolbarThree.Add(delvnodeTool);

            var tools = new ToolbarMenu { text = "Tools", variant = ToolbarMenu.Variant.Default };
            if (!PortsUtils.PlayMode)
            {
                tools.menu.AppendAction("Group", a => { vgraph.AddGroup(); }, a => DropdownMenuAction.Status.Normal);
            }
            toolbarThree.Add(tools);

            var toolbarSearchField = new ToolbarSearchField();
            toolbarSearchField.style.cursor = default(UnityEngine.UIElements.Cursor);

            if (!PortsUtils.PlayMode)
            {
                toolbarSearchField.RegisterCallback<KeyDownEvent>((x) =>
                {
                    if (x.keyCode == KeyCode.Return)
                    {
                        if (!String.IsNullOrEmpty(toolbarSearchField.value))
                            SearchVNodes(toolbarSearchField.value);
                    }
                });
            }

            toolbarThree.Add(toolbarSearchField);
            var variableBtn = new Button();

            if (!PortsUtils.PlayMode)
            {
                variableBtn.clicked += () =>
                {
                    if (!EditorWindow.HasOpenInstances<VariableDrawerWindow>())
                    {
                        VariableDrawerWindow vg = EditorWindow.GetWindow<VariableDrawerWindow>();
                        vg.Repaint();
                    }
                    else
                    {
                        EditorWindow.GetWindow<VariableDrawerWindow>().Close();
                    }
                };
            }

            variableBtn.text = "Variable Drawer";
            variableBtn.style.backgroundColor = Color.grey;
            toolbarThree.Add(variableBtn);

            var bwBtn = new Button();

            if (!PortsUtils.PlayMode)
            {
                bwBtn.clicked += () =>
                {
                    if (!EditorWindow.HasOpenInstances<BackgroundWorkerWindow>())
                    {
                        BackgroundWorkerWindow vg = EditorWindow.GetWindow<BackgroundWorkerWindow>();
                        vg.Repaint();
                    }
                    else
                    {
                        EditorWindow.GetWindow<BackgroundWorkerWindow>().Close();
                    }
                };
            }

            bwBtn.text = "Background Scheduler";
            bwBtn.style.backgroundColor = Color.grey;
            toolbarThree.Add(bwBtn);

            Toolbars.Add(toolbarThree);

            //4th toolbar
            var toolbarFourth = new Toolbar();
            toolbarFourth.pickingMode = PickingMode.Ignore;
            toolbarFourth.style.borderBottomColor = Color.gray;
            toolbarFourth.style.borderBottomWidth = 5;
            toolbarFourth.style.position = Position.Relative;
            toolbarFourth.style.height = 70;
            toolbarFourth.style.backgroundColor = Color.grey;
            toolbarFourth.name = "toolbarFourth";
            Toolbars.Add(toolbarFourth);

            //Add last toolbar
            entityToolbar = toolbarFourth;
        }
        public void SearchVNodes(string str)
        {
            if (PortsUtils.VGraph != null)
            {
                foreach (var t in PortsUtils.VGraph.graphView.nodes)
                {
                    var vnode = t as VNodes;
                    if (t.title.Contains(str, StringComparison.OrdinalIgnoreCase) &&
                    vnode.selected == false)
                    {
                        PortsUtils.VGraph.graphView.ClearSelection();
                        vnode.OnSelected();
                        vnode.Focus();
                        vnode.MarkDirtyRepaint();

                        PortsUtils.VGraph.graphView.MarkDirtyRepaint();
                        break;
                    }
                }
            }
        }

        public void InsertVNodeComponentMenu(string headerString, string componentName, VColor color)
        {
            //NOTE: The convention for VNode naming is : VCharacter/NameOfVNode
            if (!String.IsNullOrEmpty(headerString) && !String.IsNullOrEmpty(componentName))
            {
                var split = headerString.Split('/');
                var splitOne = string.Empty;
                var splitTwo = string.Empty;

                if (split.Length != 0)
                {
                    splitOne = split[0];
                }
                else
                {
                    splitOne = headerString;
                }

                if (split.Length > 1)
                {
                    splitTwo = split[1];
                }
                else
                {
                    splitTwo = headerString;
                }

                foreach (var menuItem in mainToolbar.Children())
                {
                    if (menuItem.name.Equals(componentName))
                        break;

                    if (!menuItem.name.Equals(splitOne))
                        continue;

                    var tb = menuItem as ToolbarMenu;

                    tb.menu.AppendAction(splitTwo, a =>
                    {
                        if (PortsUtils.activeVGraphAssets != null && PortsUtils.activeVNode != null)
                        {
                            if (PortsUtils.activeVGraphAssets.vports.Exists(x => x.vport.vParentNodeId.Equals(PortsUtils.activeVNode.VNodeId)))
                            {
                                vgraph.AddVBlockLabel(headerString, color, componentName);
                            }
                        }
                    });

                    break;
                }
            }
        }

        public void AddVNode()
        {
            if (PortsUtils.activeVGraphAssets != null)
            {
                PortsUtils.VGraph.graphView?.AddVNode("VNode", PortsUtils.VGraph, Vector2.zero, true);
                EditorUtility.SetDirty(PortsUtils.activeVGraphAssets);
                GameObject go = PortsUtils.FindVGraphObject();
                EditorUtility.SetDirty(go);
            }
        }
    }
}