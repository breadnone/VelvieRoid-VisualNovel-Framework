
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VelvieR;

namespace VIEditor
{
    public class VGraphTracking
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnRuntimeMethodLoad()
        {
            
            VBlockManager.vnodeTracking = null;
            var obj = new VGraphTracking();
            VBlockManager.vnodeTracking = obj.UpdateListVTrack;
        }
        private VNodes activeSelectedVnode;
        private int activeIndex;
        private VGraphsContainer currentLoadedAsset;
        private List<VGraphsContainer> vcontainers = new List<VGraphsContainer>();
        public List<VNodes> vnodes = new List<VNodes>();
        public void UpdateListVTrack(string vnodeid, string guid, int velvieBlockIndex, bool selected)
        {
            if (VBlockManager.VgraphsOpenInstance)
            {
                if(vcontainers.Count == 0)
                    vcontainers = PortsUtils.GetVGprahsScriptableObjects();

                if (vcontainers.Count == 0)
                    return;

                if(vnodes.Count == 0)
                {
                    vnodes = PortsUtils.VGraph.graphView.nodes.ToList().ConvertAll(x => (VNodes)x);
                }

                Reiterate();

                void Reiterate()
                {
                    for (int i = 0; i < vcontainers.Count; i++)
                    {
                        for (int j = 0; j < vcontainers[i].vports.Count; j++)
                        {
                            var vnode = vcontainers[i].vports[j];
                            if (vnode.vport.vParentNodeId != vnodeid)
                                continue;

                            PortsUtils.activeVGraphAssets = vcontainers[i];
                            if(currentLoadedAsset != vcontainers[i])
                            {
                                currentLoadedAsset = vcontainers[i];
                                PortsUtils.LoadAssets(vcontainers[i], false);
                            }
                            return;
                        }
                    }
                }

                if (selected)
                {
                    //TODO: This can be utterly slow on a large Graphs! Might broke things too.! 
                    if (PortsUtils.activeVGraphAssets != null && PortsUtils.VGraph.graphView != null)
                    {
                        foreach (var vnode in vnodes)
                        {
                            var vn = vnode as VNodes;
                            var vu = vn.userData as VPortsInstance;

                            if (vn.VNodeId == vnodeid)
                            {
                                activeSelectedVnode = vn;
                                vn.OnSelected();
                                PortsUtils.VGraph.listV.ScrollToItem(velvieBlockIndex);
                                PortsUtils.VGraph.listV.SetSelection(velvieBlockIndex);
                                PortsUtils.VGraph.ShowSelectedVblockSerializedFields();
                                activeIndex = velvieBlockIndex;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if(activeSelectedVnode != null)
                    {
                        PortsUtils.VGraph.listV.ClearSelection();
                        activeSelectedVnode = null;
                    }
                }
            }
        }
    }
}
#endif