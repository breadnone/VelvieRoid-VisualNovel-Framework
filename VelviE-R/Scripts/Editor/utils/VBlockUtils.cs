using System.Collections.Generic;
using UnityEngine;
using VelvieR;
using UnityEditor;
using System;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace VIEditor
{
    /////NOTE : All VBlock operations are here!
    public static class VBlockUtils
    {
        //cache all velvieblock here, don't serialize it!
        public static List<VelvieBlockComponent> vblockComponents = new List<VelvieBlockComponent>();
        //Add to vblockComponents List
        public static int activeListVIndex { get; set; }
        public static void PoolVBlockComponent(VelvieBlockComponent vcom)
        {
            if (vcom != null && !vblockComponents.Contains(vcom))
            {
                vblockComponents.Add(vcom);
            }
        }
        private static VelvieBlockComponent GetVBlockComponent(string strComponent)
        {
            VelvieBlockComponent vb = null;
            if (vblockComponents.Count > 0)
            {
                foreach (var type in vblockComponents)
                {
                    if (!type.name.Equals(strComponent))
                        continue;

                    vb = type;
                    break;
                }
            }
            return vb;
        }
        //Add vblock component to VGraphContainer gameObject in hierarchy
        public static VCoreUtil AddVBlockComponent(string strComponent, int? index = null)
        {
            if (PortsUtils.activeVGraphAssets != null && PortsUtils.activeVNode != null && !String.IsNullOrEmpty(strComponent))
            {
                var getvcomp = GetVBlockComponent(strComponent);

                if(PortsUtils.activeVObject == null)
                    PortsUtils.activeVObject = PortsUtils.FindVGraphObject();
                    
                PortsUtils.activeVObject.AddComponent(getvcomp.monoComponent);
                var vcoreUtil = PortsUtils.activeVObject.GetComponent<VCoreUtil>();
                var typeComponent = PortsUtils.activeVObject.GetComponent(getvcomp.monoComponent);

                string nodeNames = null;

                foreach (var nname in PortsUtils.activeVNode.contentContainer.Children())
                {
                    if (nname.name == "nodeName")
                    {
                        var asoritype = nname as TextField;
                        nodeNames = asoritype.value;
                    }
                }

                var newVb = new VelvieBlockComponent
                {
                    guid = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(0, int.MaxValue),
                    name = getvcomp.name,
                    monoComponent = getvcomp.monoComponent,
                    vnodeId = PortsUtils.activeVNode.VNodeId,
                    onEnterDelay = getvcomp.onEnterDelay,
                    onExitDelay = getvcomp.onExitDelay,
                    headerValue = getvcomp.headerValue,
                    summaryValue = getvcomp.summaryValue,
                    vcolor = getvcomp.vcolor,
                    vnodeName = nodeNames
                };

                List<Component> allcomps;
                PortsUtils.activeVObject.GetComponents(typeof(Component), allcomps = new List<Component>());
                var typo = allcomps[allcomps.Count - 1].GetType();

                Attribute[] attrs = Attribute.GetCustomAttributes(typo, true);
                VTagAttribute atr = null;

                foreach (var attt in attrs)
                {
                    if (attt is VTagAttribute vatr)
                    {
                        atr = vatr;
                        newVb.componentId = vatr.componentId;
                        break;
                    }
                }

                if (PortsUtils.activeVNode.IsGameStarted == EnableState.GameStarted)
                    newVb.isInGameStartedBlock = true;

                Component lastComponentAdded = null;
                int counter = 1;
                ReAddLast();

                void ReAddLast()
                {
                    if (allcomps[allcomps.Count - counter] is VBlockCore && allcomps.Count > 1)
                    {
                        lastComponentAdded = allcomps[allcomps.Count - counter];
                    }
                }

                allcomps[allcomps.Count - counter].hideFlags = HideFlags.HideInInspector;

                if (lastComponentAdded == null)
                {
                    counter++;

                    if (allcomps.Count - counter != -1)
                        ReAddLast();
                }

                //Last component added to a gameObject guaranteed to be the latest added. This may error prone for certain situations
                newVb.attachedComponent = new AttachedComponent
                {
                    component = allcomps[allcomps.Count - 1],
                    componentId = atr.componentId
                };

                var exist = vcoreUtil.vBlockCores.Exists(x => x.vnodeId == PortsUtils.activeVNode.VNodeId);

                if (exist)
                {
                    foreach (var et in vcoreUtil.vBlockCores)
                    {
                        if (et.vnodeId != PortsUtils.activeVNode.VNodeId)
                            continue;

                        if (!index.HasValue)
                            et.vblocks.Add(newVb);
                        else
                            et.vblocks.Insert(index.Value + 1, newVb);

                        break;
                    }
                }
                else
                {
                    var newVComponent = new VBlockComponent();
                    newVComponent.vnodeId = PortsUtils.activeVNode.VNodeId;

                    if (!index.HasValue)
                        newVComponent.vblocks.Add(newVb);
                    else
                        newVComponent.vblocks.Insert(index.Value + 1, newVb);

                    vcoreUtil.vBlockCores.Add(newVComponent);
                }

                if (vcoreUtil.HideAllComponents)
                {
                    vcoreUtil.HideAllVCoreComponents(vcoreUtil.HideAllComponents);
                }

                EditorUtility.SetDirty(PortsUtils.activeVObject);
                return vcoreUtil;
            }
            return null;
        }
        public static void MoveBackIndex()
        {
            var vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();

            foreach (var items in PortsUtils.VGraph.listV.selectedItems)
            {
                var astype = items as VelvieBlockComponent;
                for (int i = vcoreObj.vBlockCores.Count; i-- > 0;)
                {
                    if (vcoreObj.vBlockCores[i].vblocks.Contains(astype))
                    {
                        vcoreObj.vBlockCores[i].vblocks.Remove(astype);
                    }
                }
            }

            PortsUtils.VGraph.PrevSelected();
            PortsUtils.VGraph.listV.ScrollToItem(PortsUtils.VGraph.listV.selectedIndex);
        }
        public static void MoveForwardIndex()
        {
            if (PortsUtils.VGraph.listV.selectedItem == null || PortsUtils.VGraph.listV.itemsSource.Count == 0 || PortsUtils.PlayMode)
                return;

            var vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();
            bool success = false;
            int index = PortsUtils.VGraph.listV.selectedIndex;

            var astype = PortsUtils.VGraph.listV.selectedItem as VelvieBlockComponent;
            for (int i = 0; i < vcoreObj.vBlockCores.Count; i++)
            {
                if (vcoreObj.vBlockCores[i].vblocks.Contains(astype))
                {
                    if (index + 1 <= vcoreObj.vBlockCores[i].vblocks.Count - 1)
                    {
                        Swap(vcoreObj.vBlockCores[i].vblocks, index, PortsUtils.VGraph.listV.selectedIndex + 1);

                        PortsUtils.VGraph.listV.SetSelection(index + 1);

                        success = true;
                        break;
                    }
                }
            }
            if (success)
            {
                PortsUtils.VGraph.PrevSelected();
                PortsUtils.VGraph.listV.ScrollToItem(PortsUtils.VGraph.listV.selectedIndex);
            }
        }
        public static void MoveBackwardIndex()
        {
            if (PortsUtils.VGraph.listV.selectedItem == null || PortsUtils.VGraph.listV.itemsSource.Count == 0 || PortsUtils.PlayMode)
                return;

            var vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();
            bool success = false;
            int index = PortsUtils.VGraph.listV.selectedIndex;
            var astype = PortsUtils.VGraph.listV.selectedItem as VelvieBlockComponent;

            for (int i = 0; i < vcoreObj.vBlockCores.Count; i++)
            {
                if (vcoreObj.vBlockCores[i].vblocks.Contains(astype))
                {
                    if (index - 1 >= 0)
                    {
                        Swap(vcoreObj.vBlockCores[i].vblocks, index, PortsUtils.VGraph.listV.selectedIndex - 1);

                        PortsUtils.VGraph.listV.SetSelection(index - 1);

                        success = true;
                        break;
                    }
                }
            }
            if (success)
            {
                PortsUtils.VGraph.RefreshListV();
                //PortsUtils.VGraph.listV.ClearSelection();
            }
        }
        public static void SendToBack()
        {
            if (PortsUtils.VGraph.listV.selectedItem == null)
                return;

            var vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();
            bool success = false;
            int index = PortsUtils.VGraph.listV.selectedIndex;
            var astype = PortsUtils.VGraph.listV.selectedItem as VelvieBlockComponent;

            for (int i = 0; i < vcoreObj.vBlockCores.Count; i++)
            {
                if (vcoreObj.vBlockCores[i].vblocks.Contains(astype))
                {
                    Swap(vcoreObj.vBlockCores[i].vblocks, index, 0);
                    PortsUtils.VGraph.listV.SetSelection(0);
                    success = true;
                    break;
                }
            }
            if (success)
            {
                PortsUtils.VGraph.PrevSelected();
                PortsUtils.VGraph.listV.ScrollToItem(PortsUtils.VGraph.listV.selectedIndex);
            }
        }
        public static void SendToFront()
        {
            if (PortsUtils.VGraph.listV.selectedItem == null || PortsUtils.activeVObject == null || PortsUtils.VGraph.listV.itemsSource.Count == 0)
                return;

            var vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();
            bool success = false;
            int index = PortsUtils.VGraph.listV.selectedIndex;
            var astype = PortsUtils.VGraph.listV.selectedItem as VelvieBlockComponent;
            int lastIndex = 0;

            for (int i = 0; i < vcoreObj.vBlockCores.Count; i++)
            {
                if (vcoreObj.vBlockCores[i].vblocks.Contains(astype))
                {
                    lastIndex = vcoreObj.vBlockCores[i].vblocks.Count - 1;
                    Swap(vcoreObj.vBlockCores[i].vblocks, index, lastIndex);
                    PortsUtils.VGraph.listV.SetSelection(lastIndex);
                    success = true;
                    break;
                }
            }
            if (success)
            {
                PortsUtils.VGraph.PrevSelected();
                PortsUtils.VGraph.listV.ScrollToItem(PortsUtils.VGraph.listV.selectedIndex);
            }
        }
        public static void Find()
        {
            if(PortsUtils.PlayMode || PortsUtils.VGraph.listV.selectedItem == null)
            {
                return;
            }
            
            var tmp = PortsUtils.VGraph.listV.selectedItems.ToList().Count;
            var vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();


        }
        public static void Delete()
        {
            if(PortsUtils.PlayMode || PortsUtils.VGraph.listV.selectedItem == null)
            {
                return;
            }

            var tmp = PortsUtils.VGraph.listV.selectedItems.ToList().Count;
            var vcoreObj = PortsUtils.activeVObject.GetComponent<VCoreUtil>();

            if (tmp == 1)
            {
                var astype = PortsUtils.VGraph.listV.selectedItem as VelvieBlockComponent;

                if (astype == null)
                    return;

                List<Component> allcomps;
                vcoreObj.GetComponents(typeof(Component), allcomps = new List<Component>());

                foreach (var et in vcoreObj.vBlockCores)
                {
                    for (int j = et.vblocks.Count; j-- > 0;)
                    {
                        if (et.vblocks[j] == astype && astype.attachedComponent.component is Call calle)
                        {
                            //TODO: This is not fixed 
                            VPortsInstance thisvport = GetVPorts(calle, false);
                            VPortsInstance thatvport = GetVPorts(calle, true);

                            if (thisvport != null && thatvport != null)
                            {
                                DisconnectPorts(calle, thisvport, thatvport, true);
                            }

                            ReCheck(astype.vnodeName);
                            UnityEngine.Object.DestroyImmediate(astype.attachedComponent.component);
                            et.vblocks.Remove(astype);
                        }
                        //Non Call block removal
                        else if (et.vblocks[j] == astype && astype.attachedComponent.component is not Call)
                        {
                            UnityEngine.Object.DestroyImmediate(astype.attachedComponent.component);
                            et.vblocks.Remove(astype);
                        }
                    }
                }

                void ReCheck(string str)
                {
                    for (int i = vcoreObj.vBlockCores.Count; i-- > 0;)
                    {
                        var vcom = vcoreObj.vBlockCores[i];

                        for (int j = 0; j < vcom.vblocks.Count; j++)
                        {
                            if (vcom.vblocks[j].attachedComponent.component is Call vcall && !String.IsNullOrEmpty(str) && vcall.vnode == str)
                            {
                                VPortsInstance thisvport = GetVPorts(vcall, false);
                                VPortsInstance thatvport = GetVPorts(vcall, true);

                                if (thisvport != null && thatvport != null)
                                {
                                    DisconnectPorts(vcall, thisvport, thatvport, true);
                                }

                                vcall.VGraph = null;
                                vcall.vnode = string.Empty;
                                vcall.triggerer = null;

                                if (PortsUtils.activeVObject != vcall.gameObject)
                                    EditorUtility.SetDirty(vcall.gameObject);
                                //WARNING: THis may introduce bugs. UNTESTED!
                                //vcall.NewTriggerId();
                            }
                        }
                    }
                }
            }
            else if (tmp > 1)
            {
                List<string> deletedNodes = new List<string>();
                GameObject tmpObj = null;

                foreach (var items in PortsUtils.VGraph.listV.selectedItems)
                {
                    var astype = items as VelvieBlockComponent;

                    if (astype == null)
                        continue;

                    for (int i = vcoreObj.vBlockCores.Count; i-- > 0;)
                    {
                        for (int j = vcoreObj.vBlockCores[i].vblocks.Count; j-- > 0;)
                        {
                            if (vcoreObj.vBlockCores[i].vblocks[j] == astype && astype.attachedComponent.component is Call calle)
                            {
                                VPortsInstance thisvport = GetVPorts(calle, false);
                                VPortsInstance thatvport = GetVPorts(calle, true);

                                tmpObj = calle.gameObject;

                                if (thisvport != null && thatvport != null)
                                {
                                    DisconnectPorts(calle, thisvport, thatvport, true);
                                }

                                if (!String.IsNullOrEmpty(calle.vnode))
                                    deletedNodes.Add(calle.vnode);

                                if (calle.triggerer != null)
                                    ReCheckMultiple(calle.triggerer.nodeName);

                                UnityEngine.Object.DestroyImmediate(astype.attachedComponent.component);
                                vcoreObj.vBlockCores[i].vblocks.Remove(astype);

                                if (tmpObj != null && PortsUtils.activeVObject != tmpObj)
                                    EditorUtility.SetDirty(tmpObj);
                            }
                            //Non Call block removal
                            else if (vcoreObj.vBlockCores[i].vblocks[j] == astype && astype.attachedComponent.component is not Call)
                            {
                                UnityEngine.Object.DestroyImmediate(astype.attachedComponent.component);
                                vcoreObj.vBlockCores[i].vblocks.Remove(astype);
                            }
                        }
                    }
                }

                //TODO: THIS MUST BE FULLLY I SAY FULLLLLY COMPLETELY TESTED! consider this somewhat broken
                //If it works, then it works... Not gonna revisit this in a loooonggg timeeee...
                void ReCheckMultiple(string str)
                {
                    for (int i = vcoreObj.vBlockCores.Count; i-- > 0;)
                    {
                        var vcom = vcoreObj.vBlockCores[i];

                        for (int j = 0; j < vcom.vblocks.Count; j++)
                        {
                            if (vcom.vblocks[j].attachedComponent.component is Call vcall && !String.IsNullOrEmpty(str) && vcall.vnode == str)
                            {
                                VPortsInstance thisvport = GetVPorts(vcall, false);
                                VPortsInstance thatvport = GetVPorts(vcall, true);

                                if (PortsUtils.activeVObject != vcall.gameObject)
                                    EditorUtility.SetDirty(vcall.gameObject);

                                if (thisvport != null && thatvport != null)
                                {
                                    DisconnectPorts(vcall, thisvport, thatvport, true);
                                }

                                vcall.VGraph = null;
                                vcall.vnode = string.Empty;
                                vcall.triggerer = null;
                            }
                        }
                    }
                }
            }

            PortsUtils.VGraph.listV.ClearSelection();
            PortsUtils.VGraph.listV.Rebuild();
            PortsUtils.VGraph.RemoveActiveNodeFromView();
            PortsUtils.SetActiveAssetDirty();
        }

        public static VPortsInstance GetVPorts(Call t, bool thisNode = false)
        {
            if (!thisNode)
            {
                return PortsUtils.activeVNode.userData as VPortsInstance;
            }
            else
            {
                foreach (var vblock in PortsUtils.VGraph.graphView.nodes)
                {
                    var blocks = vblock as VNodes;

                    foreach (var content in blocks.contentContainer.Children())
                    {
                        var asoritype = content as TextField;

                        if (asoritype != null && asoritype.name == "nodeName" && asoritype.value == t.vnode)
                        {
                            return blocks.userData as VPortsInstance;
                        }
                    }
                }
            }

            return null;
        }
        public static void DisconnectPorts(Call t, VPortsInstance thisvport, VPortsInstance thatvport, bool removeFromConnected = false)
        {
            if (thisvport != null && thatvport != null)
            {
                VNodes thisNode = null;
                VNodes thatNode = null;

                foreach (var tnode in PortsUtils.VGraph.graphView.nodes)
                {
                    var astype = tnode as VNodes;

                    if (astype == null)
                        continue;

                    var usrdata = astype.userData as VPortsInstance;

                    if (usrdata == thisvport)
                    {
                        thisNode = astype;

                        if (thatNode != null)
                            break;
                    }
                    else if (usrdata == thatvport)
                    {
                        thatNode = astype;

                        if (thisNode != null)
                            break;
                    }
                }

                if (thisNode != null && thatNode != null)
                {
                    var thisedge = thisNode.outputPort.connections;

                    if (thisedge.Count() > 0)
                    {
                        foreach (var edg in thisedge.ToList())
                        {
                            if (edg == null)
                                continue;

                            if (edg.input == thatNode.inputPort && edg.output == thisNode.outputPort)
                            {
                                thisNode.outputPort.Disconnect(edg);
                                thatNode.inputPort.Disconnect(edg);

                                //RemoveConnectedFromList(thisvport, thatvport.vportInstanceGuid);
                                PortsUtils.VGraph.graphView.RemoveElement(edg);
                                PortsUtils.VGraph.graphView.MarkDirtyRepaint();

                                //TODO: STILL BROKEN
                                if (thisNode.outputPort.connections.Count() == 0 && thisNode.inputPort.connections.Count() == 0)
                                {
                                    thisNode.titleContainer.style.backgroundColor = Color.red;
                                }
                                if (thatNode.outputPort.connections.Count() == 0 && thatNode.inputPort.connections.Count() == 0)
                                {
                                    thatNode.titleContainer.style.backgroundColor = Color.red;
                                }
                                break;
                            }
                        }
                    }

                    //TODO: This may introduce bugs... change this later
                    if (removeFromConnected)
                    {
                        var findvp = thisvport.connectedTo.Find(x => x == thatvport.vportInstanceGuid);

                        if (findvp != null)
                            thisvport.connectedTo.Remove(findvp);
                    }
                }
            }
        }

        public static IList<T> Swap<T>(IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
            return list;
        }
    }
}