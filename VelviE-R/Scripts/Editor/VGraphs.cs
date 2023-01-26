using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;
using VelvieR;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;

namespace VIEditor
{
    public class VGraphs : GraphViewEditorWindow
    {
        public VViews graphView { get; set; }
        public VToolbars activeVToolbar { get; set; }
        private MiniMap miniMap;
        private ScrollView graphEntity_ScrollView;
        public Box graphEntity_ScrollView_Box { get; set; }
        private Box activeBoxPopup;
        public VisualElement inspectorWindow { get; set; }
        public bool inspectorIsActive { get; set; }
        public VisualElement parentInspectorBox { get; set; }
        private Box dummyTargetBox;

        [MenuItem("VelviE-R/VGraphs")]
        public static void CreateVGraphsWindow()
        {
            var window = GetWindow<VGraphs>(typeof(HostGuiWindow));
            window.titleContent = new GUIContent("Velvie Graph");
            window.titleContent.tooltip = "Flowchart Graph";
        }
        public void Refresh(bool repaintActiveInspector = false)
        {
            if (!repaintActiveInspector)
            {
                this.Repaint();

                if (activeVToolbar != null)
                {
                    activeVToolbar.Toolbars.ForEach(x => x.MarkDirtyRepaint());
                }

                if (graphView != null)
                {
                    foreach (var nodes in graphView.nodes)
                    {
                        nodes.MarkDirtyRepaint();
                    }

                    graphView.MarkDirtyRepaint();
                }

                rootVisualElement.MarkDirtyRepaint();
            }
            else
            {
                if (PortsUtils.activeVGraphAssets != null)
                    PortsUtils.LoadAssets(PortsUtils.activeVGraphAssets, false);
            }
        }

        void OnEnable()
        {
            //This for Summary 
            if (!PortsUtils.PlayMode)
                this.rootVisualElement.RegisterCallback<MouseMoveEvent>(VUITemplate.RegSummaryCallBack);

            PortsUtils.VGraph = this;
            var graphs = PortsUtils.GetVGprahsScriptableObjects();
            var getIvar = PortsUtils.GetVariableScriptableObjects();

            if (getIvar == null || getIvar.Count == 0)
            {
                PortsUtils.CreateVariableAsset();
            }
            else
            {
                PortsUtils.variable = getIvar[0];
            }

            if (graphs.Count == 0)
            {
                VGraphUtil.CreateVGraph();
            }
            else
            {
                int counter = 0;
                VGraphsContainer foundGraph = null;

                foreach (var tgraph in graphs)
                {
                    var tgraphData = tgraph.graphState;

                    if (AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() == tgraph.sceneGuid)
                    {
                        if (tgraphData.InspectorBtnStates)
                        {
                            counter++;
                            PortsUtils.LoadAssets(tgraph, false);
                            break;
                        }

                        foundGraph = tgraph;
                    }
                }

                if (counter == 0)
                {
                    if (foundGraph == null)
                    {
                        VGraphUtil.CreateVGraph();
                        var gg = PortsUtils.GetVGprahsScriptableObjects();
                        var refind = gg.Find(x => AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() == x.sceneGuid);
                        refind.graphState.InspectorBtnStates = true;
                        PortsUtils.LoadAssets(refind, false);
                    }
                    else
                    {
                        foundGraph.graphState.InspectorBtnStates = true;
                        PortsUtils.LoadAssets(foundGraph, false);
                    }
                }
            }

            SetToolbar();

            if (listV != null)
                listV.Rebuild();
        }
        private void OnDisable()
        {
            //This for Summary 
            if (!PortsUtils.PlayMode)
                this.rootVisualElement.UnregisterCallback<MouseMoveEvent>(VUITemplate.RegSummaryCallBack);

            if (graphView != null)
                rootVisualElement.Remove(graphView);

            PortsUtils.VGraph = null;
        }
        public void SetGraphsWindow(VViews graphViewInstance)
        {
            if (graphView != null)
            {
                graphView.graphViewChanged -= OnGraphChange;
                rootVisualElement.Remove(graphView);
                rootVisualElement.MarkDirtyRepaint();
            }

            if (graphViewInstance == null)
            {
                graphViewInstance = new VViews()
                {
                    name = "VelvieGraph"
                };
            }

            rootVisualElement.Add(graphViewInstance);
            graphView = graphViewInstance;
            graphView.StretchToParentSize();
            graphView.MarkDirtyRepaint();
            graphView.graphViewChanged += OnGraphChange;
        }
        private void CleanToolbars()
        {
            if (activeVToolbar != null)
            {
                activeVToolbar.ResetButtonList();

                for (int i = activeVToolbar.Toolbars.Count; i-- > 0;)
                {
                    if (activeVToolbar.Toolbars[i] != null)
                    {
                        activeVToolbar.Toolbars[i].RemoveFromHierarchy();
                        activeVToolbar.Toolbars.RemoveAt(i);
                    }
                }
            }
        }
        public void SetToolbar()
        {
            if (inspectorIsActive)
                HideInspector();

            CleanToolbars();

            VToolbars vt = new VToolbars();
            activeVToolbar = vt;
            vt.vgraph = this;
            vt.InsertToolBar();
            activeVToolbar.Toolbars.ForEach(x => rootVisualElement.Add(x));

            //Enable VGraphContainer Entity window
            rootVisualElement.schedule.Execute(VGraphEntityWindow).ExecuteLater(0);

            //Enable MiniMap
            if (PortsUtils.activeVGraphAssets != null && PortsUtils.activeVGraphAssets.graphState.miniMapActiveState)
                rootVisualElement.schedule.Execute(EnableMinimap);

            //populate all VBlocks Scripts in VNodes folder
            foreach (var components in VBlockUtils.vblockComponents)
            {
                if (components != null && components.headerValue != string.Empty)
                {
                    vt.InsertVNodeComponentMenu(components.headerValue, components.name, components.vcolor);
                }
            }

        }

        private GraphViewChange OnGraphChange(GraphViewChange change)
        {
            if (change.movedElements != null)
            {
                foreach (GraphElement e in change.movedElements)
                {
                    if (e.GetType() != typeof(VNodes))
                        continue;

                    var vp = e.userData as VPortsInstance;
                    vp.vnodeProperty.nodePosition = e.GetPosition();
                    e.parent.MarkDirtyRepaint();
                    EditorUtility.SetDirty(PortsUtils.activeVGraphAssets);
                }
            }

            return change;
        }
        public void AddGroup()
        {
            if (graphView == null || PortsUtils.activeVGraphAssets == null)
                return;

            var group = new Group();
            group.style.flexGrow = new StyleFloat(1);
            group.Add(new ResizableElement());

            group.pickingMode = PickingMode.Position;
            group.title = "GROUP";
            group.name = "";
            graphView.AddElement(group);
            group.autoUpdateGeometry = true;
            group.IsResizable();
            group.IsRenamable();

            PortsUtils.activeVGraphAssets.graphState.inGroup.Add(group);
            var t = graphView.GetCenterNode(this);
            graphView.schedule.Execute(() => group.SetPosition(new Rect(t.x, t.y, 300f, 300f))).ExecuteLater(1);
            graphView.MarkDirtyRepaint();
            PortsUtils.SetActiveAssetDirty();
        }

        public void EnableMinimap()
        {
            if (!PortsUtils.activeVGraphAssets.graphState.miniMapActiveState)
            {
                if (graphView == null)
                    return;

                PortsUtils.activeVGraphAssets.graphState.miniMapActiveState = true;

                if (miniMap != null && graphView.Contains(miniMap))
                {
                    miniMap.zoomFactorTextChanged = null;
                    graphView.Remove(miniMap);
                }

                miniMap = new MiniMap();
                miniMap.zoomFactorTextChanged += x => miniMap.MarkDirtyRepaint();
                graphView.Add(miniMap);
                miniMap.BringToFront();

                var centerPos = graphView.GetCenterNode(this);
                graphView.schedule.Execute(() => miniMap.SetPosition(new Rect(centerPos.x * 1.5f, centerPos.y, 100, 100)));
                PortsUtils.SetActiveAssetDirty();
            }
            else
            {
                PortsUtils.activeVGraphAssets.graphState.miniMapActiveState = false;

                if (miniMap != null && graphView.Children().Contains(miniMap))
                {
                    miniMap.zoomFactorTextChanged = null;
                    graphView.Remove(miniMap);
                }
            }
        }
        private VisualElement VBlockPopupWindow = null;
        //Create VBlock inspector window
        public void VBlockWindow()
        {
            if (VBlockPopupWindow != null && parentInspectorBox.Contains(VBlockPopupWindow))
                parentInspectorBox.Remove(VBlockPopupWindow);

            VBlockPopupWindow = new VBlockWindow(this);
            parentInspectorBox.Add(VBlockPopupWindow);
            parentInspectorBox.MarkDirtyRepaint();
            PortsUtils.SetActiveAssetDirty();
        }

        public void VGraphEntityWindow()
        {
            if (graphEntity_ScrollView == null)
            {
                graphEntity_ScrollView = new ScrollView();
                graphEntity_ScrollView.pickingMode = PickingMode.Ignore;
                graphEntity_ScrollView.style.marginTop = 10;
                graphEntity_ScrollView.mode = ScrollViewMode.Horizontal;
                graphEntity_ScrollView.horizontalScrollerVisibility = ScrollerVisibility.Auto;
                graphEntity_ScrollView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                graphEntity_ScrollView.style.height = new StyleLength(new Length(80, LengthUnit.Percent));
            }

            if (graphEntity_ScrollView_Box != null)
            {
                if (graphEntity_ScrollView_Box.childCount > 0)
                {
                    foreach (var child in graphEntity_ScrollView_Box.Children().ToList())
                    {
                        if (child == null)
                            continue;

                        child.RemoveFromHierarchy();
                    }
                }
                graphEntity_ScrollView.Remove(graphEntity_ScrollView_Box);
            }

            if (!activeVToolbar.entityToolbar.Contains(graphEntity_ScrollView))
                activeVToolbar.entityToolbar.Add(graphEntity_ScrollView);

            var box = new Box();
            box.pickingMode = PickingMode.Ignore;
            box.name = "entityBox";
            box.style.flexDirection = FlexDirection.Row;
            VEditorFunc.SetUIDynamicSize(box, 100, false);
            graphEntity_ScrollView.Add(box);
            graphEntity_ScrollView_Box = box;

            var vgraphcontainers = PortsUtils.GetVGprahsScriptableObjects();
            var getSort = vgraphcontainers.FindAll(x => AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() == x.sceneGuid);
            var sortList = new List<(Label btn, int idx)>(getSort.Count);
            activeVToolbar.ResetButtonList();

            if (getSort.Count > 0)
            {
                foreach (var t in getSort)
                    activeVToolbar.InsertToEntityToolbar(box, t);
            }

            foreach (var btn in activeVToolbar.buttons)
            {
                int parsedStr = 0;
                int.TryParse(btn.name, out parsedStr);
                sortList.Add((btn, parsedStr));
            }

            sortList.Sort((x, y) => x.idx.CompareTo(y.idx));

            for (int i = 0; i < sortList.Count; i++)
            {
                if (i != 0)
                    sortList[i].btn.PlaceInFront(sortList[i - 1].btn);
            }

            VGraphsContainer VGraphContain = null;

            if (PortsUtils.activeVGraphAssets != null)
            {
                foreach (var btn in activeVToolbar.buttons)
                {
                    var t = btn.userData as GraphStates;
                    if (PortsUtils.activeVGraphAssets.govcoreid.ToString().Equals(t.graphStateId))
                    {
                        activeVToolbar.ButtonSelectedState(btn, true);
                        VGraphContain = PortsUtils.activeVGraphAssets;
                    }
                    else
                    {
                        activeVToolbar.ButtonSelectedState(btn, false);
                    }
                }
            }

            PortsUtils.SetActiveAssetDirty();
        }
        public void InspectorWindow(Box box)
        {
            inspectorIsActive = true;

            if (parentInspectorBox == null)
            {
                parentInspectorBox = new VisualElement();
                parentInspectorBox.style.flexDirection = FlexDirection.Column;
                parentInspectorBox.style.alignSelf = Align.FlexStart;
                parentInspectorBox.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
                parentInspectorBox.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                parentInspectorBox.name = "parentInspectorBox";
                rootVisualElement.Add(parentInspectorBox);
                parentInspectorBox.Add(new ResizableElement());
            }

            if (inspectorWindow != null)
            {
                if (dummyTargetBox != null && inspectorWindow.Contains(dummyTargetBox))
                {
                    inspectorWindow.Remove(dummyTargetBox);
                    dummyTargetBox = null;
                }

                if (activeBoxPopup != null)
                {
                    if (inspectorWindow.Contains(activeBoxPopup))
                    {
                        inspectorWindow.Remove(activeBoxPopup);
                        activeBoxPopup = null;
                    }
                }

                if (parentInspectorBox.Contains(inspectorWindow))
                    parentInspectorBox.Remove(inspectorWindow);
            }

            var t = new VGetDefaultInspector();
            inspectorWindow = t.SetPopUpContainerWindow();
            parentInspectorBox.Add(inspectorWindow);
            activeBoxPopup = box;
            inspectorWindow.Add(box);
            VBlockWindow();
        }
        //Add VBlock to the inspector's scrollview
        public ListView listV { get; set; } = null;
        public void AddVBlockLabel(string titleCon, VColor col, string componentName, VNodes playModeNode = null)
        {
            if (PortsUtils.activeVGraphAssets != null && PortsUtils.activeVNode != null)
            {
                if (listV != null)
                {
                    AddRemoveListv(true);
                }

                if (PortsUtils.activeVObject == null)
                    PortsUtils.FindVGraphObject();

                VCoreUtil vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();

                int index = 0;
                bool indexChild = false;
                int? selIndex = null;
                VBlockComponent vbcom = null;

                if (!String.IsNullOrEmpty(componentName))
                {
                    if (listV.selectedItem != null)
                    {
                        vcoreObj = VBlockUtils.AddVBlockComponent(componentName, listV.selectedIndex);
                        selIndex = listV.selectedIndex;
                    }
                    else
                    {
                        vcoreObj = VBlockUtils.AddVBlockComponent(componentName);
                    }
                }

                for (int i = 0; i < vcoreObj.vBlockCores.Count; i++)
                {
                    if (vcoreObj.vBlockCores[i].vnodeId != PortsUtils.activeVNode.VNodeId)
                        continue;

                    index = i;
                    indexChild = true;
                    vbcom = vcoreObj.vBlockCores[i];
                    break;
                }
                //NOTE: Reorderable ListView will automatically change the index in the data source(original List)
                const int defHeight = 30;
                const int defWidth = 230;
                const int itemHeight = 35;

                //Rearrange IFs margins
                Func<VBlockLabel> makeItem = () =>
                {
                    var vb = new VBlockLabel(titleCon, col, defHeight, defWidth, 0, true);
                    return vb;
                };

                Action<VisualElement, int> bindItem = (e, i) =>
                {
                    var vb = e as VBlockLabel;
                    vb.SetRegisters();
                    vb.style.marginLeft = vbcom.vblocks[i].attachedComponent.leftMargin;
                    vb.style.opacity = vbcom.vblocks[i].attachedComponent.oppacity;

                    if (vbcom.vblocks[i].attachedComponent.leftMargin > 0 && !vbcom.vblocks[i].attachedComponent.isEndIf && !vbcom.vblocks[i].attachedComponent.isIf && !vbcom.vblocks[i].attachedComponent.isWhile)
                    {
                        vb.SetSymbols(true);
                    }
                    else if (vbcom.vblocks[i].attachedComponent.leftMargin == 0 && !vbcom.vblocks[i].attachedComponent.isEndIf && !vbcom.vblocks[i].attachedComponent.isIf && !vbcom.vblocks[i].attachedComponent.isWhile)
                    {
                        //vb.SetSymbols(false);
                        Component ascom = vbcom.vblocks[i].attachedComponent.component;
                        var attr = (VTagAttribute)Attribute.GetCustomAttribute(ascom.GetType(), typeof(VTagAttribute), false);

                        if (attr != null && !String.IsNullOrEmpty(attr.symbol))
                        {
                            vb.SetSymbols(false, customSymbol: attr.symbol);
                        }
                    }
                    else if (vbcom.vblocks[i].attachedComponent.isEndIf || vbcom.vblocks[i].attachedComponent.isIf || vbcom.vblocks[i].attachedComponent.isWhile)
                    {
                        vb.SetSymbols(null);
                    }

                    var asvctype = vbcom.vblocks[i].attachedComponent.component as VBlockCore;
                    vb.VBlockId = vbcom.vblocks[i].guid;

                    if (asvctype != null && !String.IsNullOrEmpty(asvctype.OnVSummary()))
                        vb.VBlockContent.text = vbcom.vblocks[i].name + "\n" + "<i><size=8>ERROR!</size></i>";
                    else
                        vb.VBlockContent.text = vbcom.vblocks[i].name;

                    vb.VBlockToggle = vbcom.vblocks[i].enable;
                    vb.SetIndicators(vb.VBlockToggle);

                    VColorAttr.GetColor(vb.VBlockContent, vbcom.vblocks[i].vcolor);
                    var ints = i + 1;
                    vb.VBlockLineNumber.text = ints.ToString();
                };

                Action<VisualElement, int> unbindItem = (e, i) =>
                {
                    var vb = e as VBlockLabel;
                    vb.UnregisterCallB();
                    vb.style.marginLeft = 0;
                };

                if (indexChild)
                {
                    listV = new ListView(vbcom.vblocks, itemHeight, makeItem, bindItem);
                    listV.unbindItem = unbindItem;
                    ListProperties(listV);

                    if (!PortsUtils.PlayMode)
                    {
                        listV.itemIndexChanged += (x, y) =>
                        {
                            PrevSelected();
                        };

                        listV.itemsRemoved += (x) =>
                        {
                            RefreshListV();
                        };
                    }
                }
                else
                {
                    listV = new ListView(new List<int>(0), itemHeight, makeItem, bindItem);
                    ListProperties(listV);
                }

                AddRemoveListv(false);

                void AddRemoveListv(bool remove)
                {
                    if (VBlockPopupWindow != null)
                    {
                        foreach (var box in VBlockPopupWindow.Children())
                        {
                            if (box.name == "blockyBoxBox")
                            {
                                if (!remove)
                                {
                                    if (!box.Contains(listV) && listV != null)
                                        box.Add(listV);
                                }
                                else
                                {
                                    if (box.Contains(listV) && listV != null)
                                        box.Remove(listV);
                                }

                                break;
                            }
                        }
                    }
                }

                RefreshListV();

                if (!PortsUtils.PlayMode)
                {
                    listV.schedule.Execute(() =>
                    {
                        if (selIndex.HasValue)
                        {
                            listV.SetSelection(selIndex.Value + 1);
                        }
                        else
                        {
                            listV.SetSelection(listV.itemsSource.Count - 1);
                        }

                        listV.ScrollToItem(listV.selectedIndex);
                        PortsUtils.VGraph?.ShowSelectedVblockSerializedFields();

                    }).ExecuteLater(0);
                }
            }
        }
        public void PrevSelected()
        {
            if (listV != null && listV.itemsSource.Count > 1)
            {
                var prevSelected = listV.selectedIndex;
                RefreshListV();
                listV.SetSelection(prevSelected);
            }
        }
        private void ReArrangeIfEndIfs(VBlockComponent vbcom)
        {
            int counter = 0;

            foreach (var core in vbcom.vblocks)
            {
                if (core == null)
                    continue;

                if (core.attachedComponent.component is If)
                {
                    core.attachedComponent.isIf = true;
                    core.attachedComponent.oppacity = 1f;
                    counter++;
                }
                else if (core.attachedComponent.component is End)
                {
                    core.attachedComponent.isEndIf = true;
                    core.attachedComponent.oppacity = 1f;

                    if (counter > 0)
                    {
                        counter--;
                    }
                }
                else if (core.attachedComponent.component is While)
                {
                    core.attachedComponent.isWhile = true;
                    core.attachedComponent.oppacity = 1f;
                    counter++;
                }
                else
                {
                    core.attachedComponent.oppacity = 1f;
                }

                if (counter > 0)
                {
                    if (!core.attachedComponent.isIf && !core.attachedComponent.isEndIf && !core.attachedComponent.isWhile)
                    {
                        core.attachedComponent.leftMargin = 10 * counter;
                        core.attachedComponent.oppacity = 0.8f;
                    }
                    else if (core.attachedComponent.isIf || core.attachedComponent.isEndIf || core.attachedComponent.isWhile)
                    {
                        if (core.attachedComponent.isIf || core.attachedComponent.isWhile)
                        {
                            if (counter - 1 > -1)
                            {
                                core.attachedComponent.leftMargin = 10 * (counter - 1);
                            }
                            else
                            {
                                core.attachedComponent.leftMargin = 0;
                            }
                        }
                        else
                        {
                            core.attachedComponent.leftMargin = 10 * counter;
                        }
                    }
                }
                else if (counter == 0)
                {
                    core.attachedComponent.leftMargin = 0;
                }
            }
            PortsUtils.SetActiveAssetDirty();
        }
        private void ListProperties(ListView listV)
        {
            listV.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            listV.showBorder = true;
            listV.selectionType = SelectionType.Multiple;
            listV.reorderMode = ListViewReorderMode.Animated;
            listV.reorderable = true;
            listV.horizontalScrollingEnabled = false;
            listV.style.alignContent = Align.Center;
            listV.style.flexGrow = new StyleFloat(1);
            VEditorFunc.SetUIDynamicSize(listV, 100, true);
            VEditorFunc.SetUIDynamicSize(listV, 100, false);
        }
        public void ShowSelectedVblockSerializedFields(bool refresh = false)
        {
            if (listV != null && PortsUtils.activeVGraphAssets != null && PortsUtils.activeVNode != null && listV.selectedItem != null)
            {
                if (inspectorWindow == null)
                    return;

                if (!refresh)
                {
                    var e = listV.selectedItem as VelvieBlockComponent;
                    var comp = e.attachedComponent.component;
                    PortsUtils.ActiveInspector = comp;
                    dummyTargetBox = DrawVInspectorWindow(comp);
                    PortsUtils.ActiveInspectorContainer = dummyTargetBox;
                    inspectorWindow.Add(dummyTargetBox);
                }
                else
                {
                    if (PortsUtils.ActiveInspector != null)
                    {
                        PortsUtils.ActiveInspectorContainer?.RemoveFromHierarchy();
                        dummyTargetBox = DrawVInspectorWindow(PortsUtils.ActiveInspector);
                        PortsUtils.ActiveInspectorContainer = dummyTargetBox;
                        inspectorWindow.Add(dummyTargetBox);
                    }
                }
            }
        }
        public void RemoveActiveNodeFromView()
        {
            if (inspectorWindow == null)
                return;

            foreach (var activeNode in inspectorWindow.Children())
            {
                if (activeNode.name == "drawInspectorWindow")
                {
                    activeNode.RemoveFromHierarchy();
                    inspectorWindow.MarkDirtyRepaint();
                    break;
                }
            }
        }
        public void RefreshListV()
        {
            if (listV == null)
                return;

            //Rearrange IFs margins
            VCoreUtil vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();
            VBlockComponent vbcom = null;

            for (int i = 0; i < vcoreObj.vBlockCores.Count; i++)
            {
                if (vcoreObj.vBlockCores[i] == null || PortsUtils.activeVNode == null || vcoreObj.vBlockCores[i].vnodeId != PortsUtils.activeVNode.VNodeId)
                    continue;

                vbcom = vcoreObj.vBlockCores[i];
                break;
            }

            if (vbcom != null)
                ReArrangeIfEndIfs(vbcom);

            listV.Rebuild();
            PortsUtils.SetActiveAssetDirty();
        }
        private ScrollView inspectorScrolllView;
        private Box DrawVInspectorWindow(Component component)
        {
            foreach (var scr in inspectorWindow.Children())
            {
                if (scr.name == "drawInspectorWindow")
                {
                    if (inspectorScrolllView != null)
                        scr.Remove(inspectorScrolllView);

                    inspectorWindow.Remove(scr);
                    break;
                }
            }

            var box = new Box();
            box.name = "drawInspectorWindow";

            var scrollV = new ScrollView();
            scrollV.pickingMode = PickingMode.Ignore;
            inspectorScrolllView = scrollV;
            scrollV.verticalScrollerVisibility = ScrollerVisibility.Auto;
            scrollV.horizontalScrollerVisibility = ScrollerVisibility.Auto;
            VEditorFunc.SetUIDynamicSize(box, 100, true);
            VEditorFunc.SetUIDynamicSize(box, 100, false);
            box.Add(scrollV);
            scrollV.Add(GenerateInspectorsBox(component));
            return box;
        }
        protected VisualElement GenerateInspectorsBox(Component component)
        {
            var box = new VisualElement();

            if (component != null)
            {
                box.style.backgroundColor = Color.grey;
                box.name = "serializedInspector";

                //Getting the name of the Class/Script
                //TODO: VBlock will have unique identifiers from one to another.
                //Like VBLock category such as: Flow, multimedia, etc

                string name = component.name;
                name = component.GetType().ToString();

                // Create and add the normal (mode) IMGUI inspector.
                //var getEditorGui = new VGetDefaultInspector();
                //var NormalInspector = new IMGUIContainer(() => { getEditorGui.DoDrawCustomIMGUIInspector(component); });
                InspectorElement uieNormalInspector = new InspectorElement();
                uieNormalInspector.Bind(new SerializedObject(component));
                box.Add(uieNormalInspector);
            }
            return box;
        }

        public void HideInspector()
        {
            if (inspectorIsActive)
            {
                if (parentInspectorBox != null && rootVisualElement.Contains(parentInspectorBox))
                {
                    if (dummyTargetBox != null && inspectorWindow.Contains(dummyTargetBox))
                    {
                        inspectorWindow.Remove(dummyTargetBox);
                        dummyTargetBox = null;
                    }

                    if (activeBoxPopup != null)
                    {
                        if (inspectorWindow.Contains(activeBoxPopup))
                        {
                            inspectorWindow.Remove(activeBoxPopup);
                            activeBoxPopup = null;
                        }
                    }

                    if (VBlockPopupWindow != null && parentInspectorBox.Contains(VBlockPopupWindow))
                    {
                        parentInspectorBox.Remove(VBlockPopupWindow);
                    }

                    if (parentInspectorBox.Contains(inspectorWindow))
                        parentInspectorBox.Remove(inspectorWindow);

                    rootVisualElement.Remove(parentInspectorBox);
                    parentInspectorBox = null;
                    inspectorIsActive = false;
                    PortsUtils.activeVNode = null;
                }
            }
            else
            {
                inspectorIsActive = true;

                if (PortsUtils.activeVNode != null)
                    InspectorWindow(PortsUtils.activeVNode.DisplayPopupWindow("dummy"));
                else
                    InspectorWindow(new Box());
            }
            PortsUtils.SetActiveAssetDirty();
        }
    }
}