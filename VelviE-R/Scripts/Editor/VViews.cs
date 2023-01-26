using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using VelvieR;
using System.Collections.Generic;
using System.Linq;

namespace VIEditor
{
    public class VViews : GraphView
    {
        private readonly Vector2 defaultNodeSize = new Vector2(100f, 150f);
        private bool copied = false;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if(!PortsUtils.PlayMode)
            {
                if (evt.target is GraphView)
                {
                    evt.menu.AppendAction("Add VNode", (e) => AddVNode("VNode", PortsUtils.VGraph, Vector2.zero, true));
                    evt.menu.AppendAction("Center", (e) => { viewTransform.position = Vector3.zero; viewTransform.scale = Vector3.one; });
                    evt.menu.AppendAction("Clear Selection", (e) => ClearSelection());

                    evt.menu.AppendAction("Paste", (e) =>
                    {
                        if (copied)
                        {
                            nodeCache = new List<VNodes>();
                            AddCacheToList();

                            PasteVNodes("rightClickPaste", "unusedData");
                        }
                    });
                }
                else if (evt.target is VNodes vnode)
                {
                    if (PortsUtils.activeVGraphAssets != null && PortsUtils.activeVGraphAssets.graphState.inGroup.Count > 0)
                    {
                        foreach (var tgroup in PortsUtils.activeVGraphAssets.graphState.inGroup)
                        {
                            if (!tgroup.ContainsElement(vnode))
                                continue;

                            evt.menu.AppendAction("Ungroup", (e) =>
                            {
                                tgroup.RemoveElement(vnode);
                            });

                        }
                    }

                    evt.menu.AppendAction("Duplicate", (e) =>
                    {
                        if (selection.Count < 2 && selection.Count > 0)
                        {
                            nodeCache = new List<VNodes>();
                            nodeCache.Add(vnode);
                            PasteVNodes("rightClickPaste", "unusedData");
                        }
                        else if (selection.Count > 1)
                        {
                            nodeCache = new List<VNodes>();
                            AddCacheToList();
                            PasteVNodes("rightClickPaste", "unusedData");
                        }

                    });

                    evt.menu.AppendAction("Delete", (e) => { DeleteSelectedVNodes("singleDeletion"); });
                }
            }
        }
        public void AddCacheToList()
        {
            foreach (var t in selection)
            {
                nodeCache.Add(t as VNodes);
            }
        }
        public void PasteVNodes(string velvieOperation, string unusedData)
        {
            if (nodeCache.Count > 0)
            {
                var tmpList = new List<VNodes>();
                ClearSelection();

                foreach (var node in nodeCache)
                {
                    if (node == null)
                        continue;

                    VNodes newVnode = CreateVNode(node.title, PortsUtils.VGraph, Vector2.zero, false);
                    PortsUtils.VGraph.graphView.AddElement(newVnode);

                    var cusRect = node.GetPosition();
                    newVnode.SetPosition(new Rect(cusRect.x + 25, cusRect.y + 15, cusRect.width, cusRect.height));
                    tmpList.Add(newVnode);
                    this.AddToSelection(newVnode);
                }

                copied = false;
            }
        }

        //Constructor
        public VViews()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("VGrpahs"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ShortcutHandler(new Dictionary<Event, ShortcutDelegate>
            {
                {Event.KeyboardEvent("a"), FrameAll },
                {Event.KeyboardEvent("b"), FrameSelection }
            }));

            this.serializeGraphElements += VNodeCaches;
            this.unserializeAndPaste += PasteVNodes;
            this.canPasteSerializedData += AllowCopy;
            this.focusable = true;
            this.deleteSelection += DeleteSelectedVNodes;

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();
        }
        public List<VNodes> nodeCache = new List<VNodes>();
        public void DeleteSelectedVNodes(string operationName, AskUser askUser = AskUser.DontAskUser)
        {
            if (this.selection.Count > 0)
            {
                VisualElement getParent = null;
                foreach (var selection in selection.ToList())
                {
                    if (selection == null)
                        continue;

                    var t = selection as VNodes;

                    if (t == null)
                        continue;

                    var e = t.userData as VPortsInstance;
                    PortsUtils.RemoveVPorts(e);

                    if (getParent == null)
                        getParent = t.parent;

                    t.RemoveFromHierarchy();
                }

                //Finds orphan edges and remove them
                PortsUtils.DisconnectPorts(getParent);
                PortsUtils.SetActiveAssetDirty();
            }
        }

        public string VNodeCaches(IEnumerable<GraphElement> elements)
        {
            if (PortsUtils.VGraph.graphView.selection.Count > 0)
            {
                nodeCache = new List<VNodes>();
                foreach (var vnode in PortsUtils.VGraph.graphView.selection)
                {
                    var asnode = vnode as VNodes;
                    nodeCache.Add(asnode);
                }

                copied = true;
            }
            return "vnodeCopyOperation";
        }

        //Restricts the built-in copy functionality in favor of our own.
        public bool AllowCopy(string ser) { return true; }
        public Port TogglePort(VNodes node, Direction toggleDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, toggleDirection, capacity, typeof(float));
        }
        //Add new VNode
        public void AddVNode(string nodeName, VGraphs vg, Vector2 spawnPos, bool rePos)
        {
            var vnode = CreateVNode(nodeName, vg, spawnPos, rePos);
            AddElement(vnode);
            vnode.MarkDirtyRepaint();

            //Set Position
            schedule.Execute(() =>
            {
                if (!rePos)
                {
                    vnode.SetPosition(new Rect(spawnPos, defaultNodeSize));
                }
                else
                {
                    var vp = vnode.userData as VPortsInstance;
                    var centerPos = new Rect(GetCenterNode(vg), defaultNodeSize);
                    vnode.SetPosition(centerPos);
                    vp.vnodeProperty.nodePosition = new Rect(centerPos);
                }
            });

            MarkDirtyRepaint();
        }

        private VNodes CreateVNode(string nodeName, VGraphs vg, Vector2 spawnPos, bool rePos)
        {
            var vnode = new VNodes()
            {
                title = nodeName,
                VText = nodeName,
                VNodeId = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(0, int.MaxValue),
                IsGameStarted = EnableState.None
            };

            var inputPort = TogglePort(vnode, Direction.Input, Port.Capacity.Multi);
            inputPort.allowMultiDrag = false;
            inputPort.name = "inputPort";
            inputPort.pickingMode = PickingMode.Ignore;

            var outputPort = TogglePort(vnode, Direction.Output, Port.Capacity.Multi);
            outputPort.allowMultiDrag = false;
            outputPort.name = "outputPort";
            outputPort.pickingMode = PickingMode.Ignore;

            inputPort.portName = "in";
            vnode.inputContainer.Add(inputPort);

            outputPort.portName = "out";
            vnode.outputContainer.Add(outputPort);

            //self caches the ports
            vnode.inputPort = inputPort;
            vnode.outputPort = outputPort;
            vnode.titleContainer.style.backgroundColor = Color.red;
            vnode.focusable = true;

            var dropEventType = new ToolbarMenu { text = "EventType", variant = ToolbarMenu.Variant.Popup };
            dropEventType.menu.AppendAction("EventType", a => { SetGameStarted(vnode, EnableState.None); dropEventType.text = "EventType"; }, a => DropdownMenuAction.Status.Normal);
            dropEventType.menu.AppendAction("GameStart", a => { SetGameStarted(vnode, EnableState.GameStarted); dropEventType.text = "GameStart"; }, a => DropdownMenuAction.Status.Normal);
            dropEventType.menu.AppendAction("Scheduler", a => { SetGameStarted(vnode, EnableState.Scheduler); dropEventType.text = "Scheduler"; }, a => DropdownMenuAction.Status.Normal);
            dropEventType.name = "eventType";

            if (vnode.IsGameStarted == EnableState.GameStarted)
                dropEventType.text = "GameStart";
            else if (vnode.IsGameStarted == EnableState.None)
                dropEventType.text = "EventType";
            else if (vnode.IsGameStarted == EnableState.Scheduler)
                dropEventType.text = "Scheduler";

            vnode.titleContainer.Add(dropEventType);
            vnode.style.height = 110;
            vnode.style.width = 150;

            //VNode description
            var txtDescription = new TextField();
            txtDescription.style.alignSelf = Align.Center;
            txtDescription.style.width = 145;
            txtDescription.style.height = 20;
            txtDescription.name = "nodeName";
            txtDescription.value = GetNonDuplicateName(txtDescription.name, vnode: vnode);

            vnode.contentContainer.Add(txtDescription);
            activeVnode = vnode;
            string tmpGuid = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(0, int.MaxValue);

            VPortsInstance vportinstance = new VPortsInstance
            {
                vnodeProperty = new VNodeProperty
                {
                    nodeColor = vnode.titleContainer.style.backgroundColor,
                    nodeTitle = vnode.title,
                    nodeId = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(0, int.MaxValue),
                    nodeName = txtDescription.value,
                    isGameStarted = vnode.IsGameStarted
                    //Note: We left nodePoition unassigned here for later
                },

                vParentNodeId = vnode.VNodeId,
                vportInstanceGuid = tmpGuid
            };

            vnode.userData = vportinstance as VPortsInstance; //Node.userData is very handy to pass data around! 
            PortsUtils.InsertToVPorts(vportinstance);
            if (!PortsUtils.PlayMode)
            {
                txtDescription.RegisterCallback<FocusOutEvent>((x) =>
                {
                    /*
                    var vpFound = PortsUtils.activeVGraphAssets.vports.Find(x => x.vport.vnodeProperty.nodeId == vportinstance.vnodeProperty.nodeId);

                    if (vpFound != null)
                    {
                        var getname = GetNonDuplicateName(txtDescription.name, txtDescription.value);
                        vpFound.vport.vnodeProperty.nodeName = getname;
                        txtDescription.value = getname;
                        vportinstance.vnodeProperty.nodeName = getname;
                    }

                    
                    */
                                if (!PortsUtils.PlayMode)
                                {
                                    var vpFound = PortsUtils.activeVGraphAssets.vports.Find(x => x.vport.vnodeProperty.nodeId == vportinstance.vnodeProperty.nodeId);

                                    if (vpFound != null && PortsUtils.VGraph != null)
                                    {
                                        vpFound.vport.vnodeProperty.nodeName = PortsUtils.VGraph.graphView.GetNonDuplicateName(txtDescription.name, txtDescription.value, vnode);
                                        txtDescription.value = vpFound.vport.vnodeProperty.nodeName;
                                    }
                                }

                                PortsUtils.VGraph.RefreshListV();
                });
            }

            ports.ForEach(x =>
            {
                if (x.node != vnode)
                    x.node.selected = false;
            });

            vnode.RefreshExpandedState();
            vnode.RefreshPorts();
            PortsUtils.SetActiveAssetDirty();
            return vnode;
        }
        public string GetNonDuplicateName(string txtDescription, string defName = "Node", VNodes vnode = null)
        {
            string s = string.Empty;
            string defValue = defName;
            var allNodes = this.nodes;
            int counta = 0;

            ReIterateNodesDescription();

            void ReIterateNodesDescription()
            {
                foreach (var nodeDesc in allNodes)
                {
                    var asVn = nodeDesc as VNodes;

                    if (vnode != null)
                    {
                        //var asvport = (VPortsInstance)asVn.userData;
                        if (asVn == vnode)
                        {
                            continue;
                        }
                    }

                    if (asVn != null)
                    {
                        foreach (var intt in asVn.Children())
                        {
                            if (intt.name.Length == txtDescription.Length && intt.name == txtDescription)
                            {
                                var asbb = intt as TextField;

                                if (asbb.value == defValue && counta == 0)
                                {
                                    counta++;
                                    ReIterateNodesDescription();
                                    return;
                                }
                                else if (asbb.value == defValue + counta && counta > 0)
                                {
                                    counta++;
                                    ReIterateNodesDescription();
                                    return;
                                }
                            }
                        }
                    }
                }

                if (counta == 0)
                {
                    s = defName;
                }
                else
                {
                    s = defName + counta;
                }
            }
            return s;
        }
        private void AddOptionPort(VNodes vnnode)
        {
            var generatePort = TogglePort(vnnode, Direction.Output);
            var outputPortCount = vnnode.outputContainer.Query("connector").ToList().Count;
            generatePort.portName = "Choice" + outputPortCount;
            vnnode.outputContainer.Add(generatePort);
        }
        public void SetGameStarted(VNodes vnode, EnableState state)
        {
            var vcoreUtil = PortsUtils.activeVObject.GetComponent<VCoreUtil>();

            var vp = vnode.userData as VPortsInstance;
            vp.vnodeProperty.isGameStarted = state;
            vnode.IsGameStarted = state;

            foreach (var vc in vcoreUtil.vBlockCores)
            {
                if (!vnode.VNodeId.Equals(vc.vnodeId))
                    continue;

                if (vc.vblocks.Count == 0)
                    continue;

                foreach (var vcb in vc.vblocks)
                {
                    if (state == EnableState.GameStarted)
                        vcb.isInGameStartedBlock = true;
                    else
                        vcb.isInGameStartedBlock = false;
                }
            }
        }
        public Vector2 GetCenterNode(VGraphs vg)
        {
            var vviewRect = vg.graphView.contentContainer.contentRect;
            var graphView = vg.graphView.viewTransform.position;
            var graphViewScale = vg.graphView.scale;
            var t = new Vector2((vviewRect.width / 2f) - graphView.x, (vviewRect.height / 2f) - graphView.y);
            return t / graphViewScale;
        }
        public VNodes activeVnode { get; set; }
    }
}