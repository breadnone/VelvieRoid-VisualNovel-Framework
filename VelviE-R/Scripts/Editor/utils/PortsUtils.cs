using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VelvieR;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;

namespace VIEditor
{
    [System.Serializable]
    public class VEditorNotify
    {
        public Editor editor;
        public Action act;
    }
    /////NOTE : Most editor custom classes are in EditorClassUtils.cs
    public static class PortsUtils
    {
        public static VInventory ActiveInventory { get; set; }
        public static VCharacterContainer VCharaContainer { get; set; }
        public static Variables variable { get; set; }
        public static VInputBuffer ActiveInputHandle { get; set; }
        public static List<VEditorNotify> RefreshBinds = new List<VEditorNotify>();
        public static List<VCoreUtil> Vcores = new List<VCoreUtil>();
        public static VCharacter activeVCharacter { get; set; }
        public static VNodes activeVNode { get; set; }
        public static GameObject activeVObject { get; set; }
        public static List<VBlockLabel> vblocks = new List<VBlockLabel>();
        public static VGraphs VGraph { get; set; }
        public static string savePath { get; set; } = "Assets/VelviE-R/Resources/";
        public static VGraphsContainer LastPlayedVContainer { get; set; }
        public static bool waitLoading { get; set; }
        public static bool PlayMode { get; set; }
        public static Component ActiveInspector {get;set;}
        public static Box ActiveInspectorContainer {get;set;}
        public static VCharacterV[] GetCharacters()
        {
            var chars = VEditorFunc.EditorGetVCharacterUtils();
            VCharacterV[] vchars = new VCharacterV[chars.Length];

            if (chars != null && chars.Length > 0)
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    vchars[i] = chars[i].character;
                }

                return vchars;
            }

            return null;
        }

        public static void InsertToVPorts(VPortsInstance vp)
        {
            if (vp == null)
                return;

            var vpt = new VPorts
            {
                guid = Guid.NewGuid().ToString(),
                vport = vp
            };

            activeVGraphAssets.vports.Add(vpt);
        }
        public static void RemoveVPorts(VPortsInstance vp)
        {
            if (vp == null)
                return;

            var vcore = activeVObject.GetComponent<VCoreUtil>();
            var strId = string.Empty;

            for (int i = activeVGraphAssets.vports.Count; i-- > 0;)
            {
                var vcon = activeVGraphAssets.vports[i];

                if (vcon.vport == vp)
                {
                    for (int j = vcore.vBlockCores.Count; j-- > 0;)
                    {
                        var comp = vcore.vBlockCores[j];

                        if (comp.vnodeId == vp.vParentNodeId)
                        {
                            var foundComp = comp.vblocks.Find(x => x.vnodeId == vp.vParentNodeId);

                            if (foundComp != null)
                            {
                                if (foundComp.attachedComponent.component is Call calle)
                                {
                                    //TODO: If all went south, all due to how the ports and those connections.
                                    // Not going to revisit this hell. Ever!
                                    VPortsInstance thisvport = VBlockUtils.GetVPorts(calle, false);
                                    VPortsInstance thatvport = VBlockUtils.GetVPorts(calle, true);

                                    //deletedNode = foundComp.vnodeName;
                                    if (!String.IsNullOrEmpty(foundComp.vnodeName))
                                    {
                                        ReCheck(foundComp.vnodeName, thisvport);
                                    }

                                    if (thisvport != null && thatvport != null)
                                    {
                                        VBlockUtils.DisconnectPorts(calle, thisvport, thatvport, true);
                                    }
                                }

                                //TODO: BROKEN!:
                                void ReCheck(string str, VPortsInstance thatVport)
                                {
                                    for (int u = vcore.vBlockCores.Count; u-- > 0;)
                                    {
                                        var vcom = vcore.vBlockCores[u];

                                        for (int p = 0; p < vcom.vblocks.Count; p++)
                                        {
                                            if (vcom.vblocks[p].attachedComponent.component is Call vcalle && vcalle.vnode == str)
                                            {
                                                VPortsInstance thisvportSecond = VBlockUtils.GetVPorts(vcalle, false);

                                                if (thisvportSecond != null && thatVport != null)
                                                {
                                                    VBlockUtils.DisconnectPorts(vcalle, thisvportSecond, thatVport);

                                                    vcalle.VGraph = null;
                                                    vcalle.vnode = null;
                                                    vcalle.triggerer = null;

                                                    if (PortsUtils.activeVObject != vcalle.gameObject)
                                                        EditorUtility.SetDirty(vcalle.gameObject);
                                                }

                                                //WARNING: THis may introduce bugs. UNTESTED!
                                                //vcall.NewTriggerId();
                                            }
                                        }
                                    }
                                }

                                UnityEngine.Object.DestroyImmediate(foundComp.attachedComponent.component);
                                comp.vblocks.Remove(foundComp);
                                PortsUtils.VGraph.RemoveActiveNodeFromView();
                                //break;
                            }
                        }
                    }

                    activeVGraphAssets.vports.Remove(vcon);
                    break;
                }
            }
        }
        public static void ConnectPorts(VPortsInstance thisVPortInstance, VPortsInstance thatVPortInstance, bool forceDuplicates = false)
        {
            if (thisVPortInstance != null && thatVPortInstance != null && VGraph.graphView != null)
            {
                var inOut = GetInputOutputVNodes(thisVPortInstance, thatVPortInstance);
                Port outputPort = inOut.thisOutputPort;
                Port inputPort = inOut.thatinputPort;

                VNodes thisVnodes = inOut.thisVnode;
                VNodes thatVnodes = inOut.thatVnodes;

                if (thisVPortInstance == thatVPortInstance)
                    return;

                var exists = thisVPortInstance.connectedTo.Exists(x => x.Equals(thatVPortInstance.vportInstanceGuid));

                if (exists && !forceDuplicates)
                {
                    //stack them if any
                    //NOTE: If any graphical glitches of stacking when loading/saving/removing, then here where it comes from!
                    //thisVPortInstance.connectedTo.Add(thatVPortInstance.vportInstanceGuid);
                    //thisVPortInstance.connectedTo.Add(thatVPortInstance.vportInstanceGuid);
                    return;
                }
                else
                {
                    thisVnodes.expanded = true;
                    thatVnodes.expanded = true;
                    var edge = outputPort.ConnectTo(inputPort);
                    outputPort.Connect(edge);
                    VGraph.graphView.AddElement(edge);

                    //WORKAROUND
                    //Disable Mouse events on edges!
                    //TODO: Unsubscribe this when saving! -> PortUtils
                    edge.RegisterCallback<MouseDownEvent>(e => e.StopImmediatePropagation(), TrickleDown.TrickleDown);
                    thisVnodes.titleContainer.style.backgroundColor = Color.blue;
                    thatVnodes.titleContainer.style.backgroundColor = Color.blue;
                    thisVPortInstance.vnodeProperty.nodeColor = Color.blue;
                    thatVPortInstance.vnodeProperty.nodeColor = Color.blue;
                    thisVPortInstance.connectedTo.Add(thatVPortInstance.vportInstanceGuid);

                    //WorkAround! For broken edges 
                    Rect realPosThis = thisVnodes.GetPosition();
                    Rect thatPosition = thatVnodes.GetPosition();
                    Rect tmpPosThis = new Rect(realPosThis.x + 1f, realPosThis.y, realPosThis.width, realPosThis.height);
                    thisVnodes.SetPosition(tmpPosThis);
                    thatVnodes.schedule.Execute(() => { thisVnodes.SetPosition(realPosThis); }).ExecuteLater(0);

                    thisVPortInstance.vnodeProperty.nodePosition = new Rect(realPosThis);
                    thatVPortInstance.vnodeProperty.nodePosition = new Rect(thatPosition);

                    //Refresh ports                  
                    thisVnodes.RefreshPorts();
                    thatVnodes.RefreshPorts();
                    thisVnodes.RefreshExpandedState();
                    thatVnodes.RefreshExpandedState();

                }
            }
            SetActiveAssetDirty();
        }
        private static (VNodes thisVnode, VNodes thatVnodes, Port thisOutputPort, Port thatinputPort) GetInputOutputVNodes(VPortsInstance thisVPortInstance, VPortsInstance thatVPortInstance)
        {
            Port outputPort = null;
            Port inputPort = null;

            VNodes thisVnodes = null;
            VNodes thatVnodes = null;

            foreach (var getPorts in VGraph.graphView.nodes)
            {
                var vnn = getPorts as VNodes;

                if (vnn.VNodeId == thisVPortInstance.vParentNodeId)
                {
                    thisVnodes = vnn;
                    outputPort = vnn.outputPort;

                    if (inputPort != null)
                        break;
                }
                else if (vnn.VNodeId == thatVPortInstance.vParentNodeId)
                {
                    thatVnodes = vnn;
                    inputPort = vnn.inputPort;

                    if (outputPort != null)
                        break;
                }
            }
            return (thisVnodes, thatVnodes, outputPort, inputPort);
        }
        public static void DisconnectPorts(in VisualElement parent)
        {
            foreach (var edge in VGraph.graphView.edges)
            {
                var inNode = edge.input.node as VNodes;
                var outNode = edge.output.node as VNodes;

                if(inNode == null || outNode == null)
                    continue;
                    
                bool isInParent = parent.Contains(inNode);
                bool isOutParent = parent.Contains(outNode);

                if (!isInParent || !isOutParent)
                {
                    inNode.outputPort.Disconnect(edge);
                    outNode.inputPort.Disconnect(edge);
                    outNode.RefreshExpandedState();
                    edge.RemoveFromHierarchy();
                }
            }
            //Cache the ITransform here
            var trans = VGraph.graphView.viewTransform;
            LoadAssets(activeVGraphAssets, false);

            //Reloads back the cached ITransform of a viewport
            VGraph.graphView.viewTransform.position = trans.position;
            VGraph.graphView.viewTransform.scale = trans.scale;
            SetActiveAssetDirty();
        }

        public static void CreateVariableAsset()
        {
            if (!AssetDatabase.IsValidFolder("Assets/VelviE-R/Resources/VelvieData"))
                AssetDatabase.CreateFolder("Assets/VelviE-R/Resources", "VelvieData");

            string tmpPath = "Assets/VelviE-R/Resources/VelvieData/VDAT-sr-nh.asset";
            var varContainer = ScriptableObject.CreateInstance<Variables>();
            varContainer.sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString();

            //index check
            AssetDatabase.CreateAsset(varContainer, tmpPath);
            EditorUtility.SetDirty(varContainer);

            //Immediately sets to active graphView
            var assetObj = (Variables)AssetDatabase.LoadAssetAtPath(tmpPath, typeof(Variables));
            EditorUtility.SetDirty(assetObj);
            PortsUtils.variable = assetObj;
        }

        //SAVE HERE
        //Used on the very 1st creation of Vgraph gameobject in the hierarchy
        public static void CreateSaveAsset(GameObject obj, int vcoreId, string goName, VCoreUtil vcoreCom)
        {
            //spawn the VSettings SO if none
            vcoreCom.VIo = obj.GetComponent<VInputBuffer>();

            var graphContainer = ScriptableObject.CreateInstance<VGraphsContainer>();
            graphContainer.govcoreid = vcoreId;

            if (!AssetDatabase.IsValidFolder("Assets/VelviE-R/Resources"))
                AssetDatabase.CreateFolder("Assets/VelviE-R", "Resources");

            string tmpPath = savePath + "VelviE-R-VGraphAssets-" + vcoreId + ".asset";
            graphContainer.path = tmpPath;
            graphContainer.vgraphGOname = goName;

            //index check
            var allAssets = GetVGprahsScriptableObjects();
            int idx = allAssets.Count + 1;
            graphContainer.entityIndex = idx;
            graphContainer.sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString();

            AssetDatabase.CreateAsset(graphContainer, tmpPath);
            EditorUtility.SetDirty(obj);

            //Immediately sets to active graphView
            var assetObj = (VGraphsContainer)AssetDatabase.LoadAssetAtPath(tmpPath, typeof(VGraphsContainer));
            assetObj.sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString();

            EditorUtility.SetDirty(assetObj);
            AssetDatabase.SaveAssets();

            if (allAssets.Count > 1)
            {
                foreach (var assets in allAssets)
                {
                    if (assets.govcoreid != graphContainer.govcoreid)
                    {
                        assets.graphState.InspectorBtnStates = false;
                    }
                    else
                    {
                        assets.graphState.InspectorBtnStates = true;
                    }
                }
            }
            else if (allAssets.Count == 1)
            {
                allAssets[0].graphState.InspectorBtnStates = true;
            }

            //LoadAssets(graphContainer, true);
            LoadAssets(assetObj, true);
        }
        public static void LoadAssets(VGraphsContainer vgraph, bool firstInit) //e.g : VelviE-R-VGraphAssets-" + id + ".asset
        {
            if(!waitLoading)
            {
                waitLoading = true;
                
                if (vgraph == null)
                {
                    return;
                }

                SetActiveVgraph(vgraph);

                VViews nuView = new VViews();
                VGraph?.SetGraphsWindow(nuView);

                if (!firstInit)
                {
                    if (vgraph.vports.Count > 0)
                    {
                        for (int i = vgraph.vports.Count; i-- > 0;)
                        {
                            var vnodeinstance = vgraph.vports[i];

                            var vnode = new VNodes()
                            {
                                title = vnodeinstance.vport.vnodeProperty.nodeTitle,
                                VText = vnodeinstance.vport.vnodeProperty.nodeName,
                                VNodeId = vnodeinstance.vport.vParentNodeId,
                                IsGameStarted = vnodeinstance.vport.vnodeProperty.isGameStarted
                            };

                            vnode.SetPosition(new Rect(vnodeinstance.vport.vnodeProperty.nodePosition));

                            var inputPort = nuView.TogglePort(vnode, Direction.Input, Port.Capacity.Multi);
                            inputPort.allowMultiDrag = false;
                            inputPort.name = "inputPort";
                            inputPort.pickingMode = PickingMode.Ignore;

                            var outputPort = nuView.TogglePort(vnode, Direction.Output, Port.Capacity.Multi);
                            outputPort.allowMultiDrag = false;
                            outputPort.name = "outputPort";
                            outputPort.pickingMode = PickingMode.Ignore;

                            inputPort.portName = "in";
                            vnode.inputContainer.Add(inputPort);

                            outputPort.portName = "out";
                            vnode.outputContainer.Add(outputPort);

                            //self cached the ports
                            vnode.inputPort = inputPort;
                            vnode.outputPort = outputPort;
                            vnode.titleContainer.style.backgroundColor = Color.red;
                            vnode.focusable = true;

                            var dropEventType = new ToolbarMenu { text = "EventType", variant = ToolbarMenu.Variant.Popup };
                            dropEventType.menu.AppendAction("EventType", a => { nuView.SetGameStarted(vnode, EnableState.None); dropEventType.text = "EventType"; }, a => DropdownMenuAction.Status.Normal);
                            dropEventType.menu.AppendAction("GameStart", a => { nuView.SetGameStarted(vnode, EnableState.GameStarted); dropEventType.text = "GameStart"; }, a => DropdownMenuAction.Status.Normal);
                            dropEventType.menu.AppendAction("Scheduler", a => { nuView.SetGameStarted(vnode, EnableState.Scheduler); dropEventType.text = "Scheduler"; }, a => DropdownMenuAction.Status.Normal);
                            dropEventType.name = "eventType";

                            if (vnode.IsGameStarted == EnableState.GameStarted)
                                dropEventType.text = "GameStart";
                            else if(vnode.IsGameStarted == EnableState.None)
                                dropEventType.text = "EventType";
                            else if(vnode.IsGameStarted == EnableState.Scheduler)
                                dropEventType.text = "Scheduler";

                            vnode.titleContainer.Add(dropEventType);
                            vnode.style.height = 110;
                            vnode.style.width = 150;

                            //VNode description
                            var txtDescription = new TextField();
                            txtDescription.style.alignSelf = Align.Center;
                            txtDescription.multiline = true;
                            txtDescription.style.width = 142;
                            txtDescription.style.height = 20;
                            txtDescription.name = "nodeName";
                            txtDescription.value = vnodeinstance.vport.vnodeProperty.nodeName;
                            vnode.contentContainer.Add(txtDescription);

                            VPortsInstance vins = new VPortsInstance
                            {
                                vnodeProperty = new VNodeProperty
                                {
                                    nodeColor = new StyleColor(vnode.titleContainer.style.backgroundColor.value),
                                    nodeTitle = vnodeinstance.vport.vnodeProperty.nodeTitle,
                                    nodeId = vnodeinstance.vport.vnodeProperty.nodeId,
                                    nodeName = vnodeinstance.vport.vnodeProperty.nodeName,
                                    nodePosition = new Rect(vnodeinstance.vport.vnodeProperty.nodePosition.x, vnodeinstance.vport.vnodeProperty.nodePosition.y, vnodeinstance.vport.vnodeProperty.nodePosition.width, vnodeinstance.vport.vnodeProperty.nodePosition.height),
                                    isGameStarted = vnodeinstance.vport.vnodeProperty.isGameStarted
                                },

                                vParentNodeId = vnode.VNodeId,
                                vportInstanceGuid = vnodeinstance.vport.vportInstanceGuid
                            };

                            txtDescription.RegisterCallback<FocusOutEvent>((x) =>
                            {
                                if (!PortsUtils.PlayMode)
                                {
                                    var vpFound = PortsUtils.activeVGraphAssets.vports.Find(x => x.vport.vnodeProperty.nodeId == vins.vnodeProperty.nodeId);

                                    if (vpFound != null)
                                    {
                                        vpFound.vport.vnodeProperty.nodeName = VGraph.graphView.GetNonDuplicateName(txtDescription.name, txtDescription.value, vnode);
                                        txtDescription.value = vpFound.vport.vnodeProperty.nodeName;
                                    }
                                }
                            });

                            if (vnodeinstance.vport.connectedTo.Count > 0)
                            {
                                vins.connectedTo = new List<string>(vnodeinstance.vport.connectedTo);
                            }

                            vnode.userData = vins as VPortsInstance;
                            vnodeinstance.vport = vins;

                            var vpt = new VPorts();
                            vpt.guid = vnodeinstance.guid;
                            vpt.vport = vins;
                            vnodeinstance = vpt;
                            nuView.AddElement(vnode);
                        }

                        //connecting ports                
                        for (int i = 0; i < vgraph.vports.Count; i++)
                        {
                            var thisP = vgraph.vports[i];

                            foreach (var con in thisP.vport.connectedTo)
                            {
                                for (int j = 0; j < vgraph.vports.Count; j++)
                                {
                                    var thatP = vgraph.vports[j];

                                    if (con != thatP.vport.vportInstanceGuid)
                                        continue;

                                    var inOut = GetInputOutputVNodes(thisP.vport, thatP.vport);
                                    Port outputPort = inOut.thisOutputPort;
                                    Port inputPort = inOut.thatinputPort;

                                    VNodes thisVnodes = inOut.thisVnode;
                                    VNodes thatVnodes = inOut.thatVnodes;

                                    thisVnodes.titleContainer.style.backgroundColor = Color.blue;
                                    thatVnodes.titleContainer.style.backgroundColor = Color.blue;

                                    var edge = outputPort.ConnectTo(inputPort);
                                    outputPort.Connect(edge);

                                    edge.RegisterCallback<MouseDownEvent>(e => e.StopImmediatePropagation(), TrickleDown.TrickleDown);
                                    nuView.AddElement(edge);
                                }
                            }
                        }

                        //TODO : Group isn't working yet
                        /*
                        if(vgraph.graphState.inGroup.Count > 0)
                        {
                            foreach(var group in vgraph.graphState.inGroup)
                            {
                                nuView.AddElement(group);
                            }
                        }
                        */
                    }
                }

                VGraph.SetToolbar();
                SetActiveAssetDirty();
                VGraph.rootVisualElement.schedule.Execute(() => {waitLoading = false;}).ExecuteLater(1);
            }
        }
        public static void SetActiveAssetDirty()
        {
            if (activeVGraphAssets != null)
            {
                EditorUtility.SetDirty(activeVGraphAssets);
            }

            if (activeVObject != null)
                EditorUtility.SetDirty(activeVObject);

            if (variable != null)
            {
                EditorUtility.SetDirty(variable);
            }
        }
        public static List<VGraphsContainer> GetVGprahsScriptableObjects()
        {
            var t = new List<VGraphsContainer>();
            var assets = AssetDatabase.FindAssets("t:VGraphsContainer", new[] { "Assets/VelviE-R/Resources" });

            foreach (var guid in assets)
            {
                var e = AssetDatabase.LoadAssetAtPath<VGraphsContainer>(AssetDatabase.GUIDToAssetPath(guid));

                if (e != null)
                {
                    t.Add(e);
                }
            }
            return t;
        }
        public static List<Variables> GetVariableScriptableObjects()
        {
            var t = new List<Variables>();
            var assets = AssetDatabase.FindAssets("t:Variables", new[] { "Assets/VelviE-R/Resources/VelvieData" });

            foreach (var guid in assets)
            {
                var e = AssetDatabase.LoadAssetAtPath<Variables>(AssetDatabase.GUIDToAssetPath(guid));

                if (e != null)
                {
                    t.Add(e);
                }
            }
            return t;
        }

        public static GameObject FindVGraphObject()
        {
            GameObject vgo = null;
            var all = VEditorFunc.EditorGetVCoreUtils();
            bool mismatchGuid = false;

            foreach (var go in all)
            {
                if (go.name == activeVGraphAssets.vgraphGOname)
                {                    
                    if (go.vcoreid == activeVGraphAssets.govcoreid)
                    {
                        vgo = go.gameObject;
                        break;
                    }

                    mismatchGuid = true;                    
                }
            }

            if(mismatchGuid)
            {
                Debug.LogError("VGraph's GUID mismatch! Most probably due to re-generated meta files");
            }

            return vgo;
        }

        public static VGraphsContainer activeVGraphAssets { get; set; }
        public static void SetActiveVgraph(VGraphsContainer vgraph)
        {
            activeVGraphAssets = vgraph;
            activeVObject = FindVGraphObject();
        }
    }
}