using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using VIEditor;
using VelvieR;
using UnityEngine.Events;
using System.Reflection;

[CustomEditor(typeof(Call))]
public class CallEditor : Editor
{
    private ToolbarMenu txtfld;
    private ToolbarMenu objfld;
    private Call thisCall;
    private Label summary;
    public override VisualElement CreateInspectorGUI()
    {
        Box myInspector = new Box();
        var t = target as Call;
        Undo.RecordObject(t, "Call undo object");
        thisCall = t;

        if (String.IsNullOrEmpty(t.TriggerId))
            t.TriggerId = Guid.NewGuid().ToString() + UnityEngine.Random.Range(0, int.MaxValue);

        myInspector.style.flexDirection = FlexDirection.Column;
        myInspector.Add(DrawVGraph(t));
        myInspector.Add(DrawVNodeName(t));
        myInspector.Add(DrawContinueState(t));

        //Always add this at the end!
        VUITemplate.DrawSummary(myInspector, t, ()=> t.OnVSummary());
        return myInspector;
    }
    public void AskNewTriggerId()
    {
        if (thisCall != null)
            thisCall.TriggerId = Guid.NewGuid().ToString() + UnityEngine.Random.Range(0, int.MaxValue);
    }
    private Box DrawVGraph(Call t)
    {
        //Character's Sprites
        var boxGraph = VUITemplate.VGraphTemplate();

        if (t.VGraph != null)
        {
            boxGraph.child.value = t.VGraph.vcorename;
        }
        else
        {
            boxGraph.child.value = "<None>";
        }
        
        if(!PortsUtils.PlayMode)
        {
            boxGraph.child.RegisterValueChangedCallback((x) =>
            {
                var vgraphs = VEditorFunc.EditorGetVCoreUtils();

                if(vgraphs == null || vgraphs.Length == 0)
                    return;

                bool found = false;
                
                foreach(var g in vgraphs)
                {
                    if(g == null)
                        continue;

                    if(g.vcorename == x.newValue)
                    {
                        t.VGraph = g;
                        found = true;
                        break;
                    }
                }

                if(!found)
                {
                    boxGraph.child.value = "<None>";
                }
            });
        }

        return boxGraph.root;
    }
    private void RepoPulateGraphMenus(Call t)
    {
        objfld.menu.MenuItems().Clear();
        var loadasset = VEditorFunc.EditorGetVCoreUtils();

        if(PortsUtils.PlayMode)
            return;

        foreach (var loadAset in loadasset)
        {
            objfld.menu.AppendAction(loadAset.vcorename, a =>
            {
                objfld.text = loadAset.vcorename;
                t.VGraph = loadAset;
                RePoolNodeComboBox(t);
                WarningCheck(t);
            });
        }
    }
    //ContinueVBlockState
    private Box DrawVNodeName(Call t)
    {
        //Nodes      
        var boxVnodeName = new Box();
        boxVnodeName.style.marginTop = 5;
        boxVnodeName.style.flexDirection = FlexDirection.Row;

        Label strLbl = new Label();
        strLbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strLbl.text = "VNode : ";
        boxVnodeName.Add(strLbl);

        var objField = new ToolbarMenu();
        txtfld = objField;
        objField.style.marginLeft = 4;
        objField.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        if(!PortsUtils.PlayMode)
        {
        txtfld.RegisterCallback<MouseEnterEvent>((x) =>
        {
            RePoolNodeComboBox(t);
        });
        }

        //Draw combobox
        RePoolNodeComboBox(t);
        boxVnodeName.Add(objField);
        return boxVnodeName;
    }
    private void RePoolNodeComboBox(Call t)
    {
        if(PortsUtils.PlayMode)
            return;

        if (txtfld == null)
            return;

        if (txtfld.menu.MenuItems().Count > 0)
            txtfld.menu.MenuItems().Clear();

        if (t.VGraph == null)
        {
            // This VGraph if null
            foreach (var node in PortsUtils.VGraph.graphView.nodes)
            {
                var asorinode = node as VNodes;

                if (asorinode == null)
                    continue;

                string nodeNm = string.Empty;

                foreach (var ast in asorinode.contentContainer.Children())
                {
                    var txtdsc = ast as TextField;

                    if (txtdsc == null || txtdsc.name != "nodeName")
                        continue;

                    nodeNm = txtdsc.value;

                    txtfld.menu.AppendAction(nodeNm, a =>
                    {
                        if (!String.IsNullOrEmpty(txtfld.text))
                        {
                            txtfld.text = nodeNm;
                            t.vnode = nodeNm;
                            DrawConnectedLines(t);
                            WarningCheck(t);
                        }
                    });
                    break;
                }
                if (String.IsNullOrEmpty(t.vnode) || t.vnode == "<None>")
                {
                    txtfld.text = "<None>";
                }
                else
                {
                    txtfld.text = t.vnode;
                }
            }
        }
        else
        {
            var loadasset = PortsUtils.GetVGprahsScriptableObjects();
            VGraphsContainer gContainer = null;

            foreach (var vg in loadasset)
            {
                if (vg.govcoreid == t.VGraph.vcoreid)
                {
                    gContainer = vg;
                    break;
                }
            }

            // To jump to other VGraph!
            foreach (var vblock in gContainer.vports)
            {
                var tt = vblock.vport.vnodeProperty.nodeName;

                if (String.IsNullOrEmpty(tt))
                    continue;

                txtfld.menu.AppendAction(tt, a =>
                {
                    txtfld.text = tt;
                    t.vnode = tt;
                    DrawConnectedLines(t);
                    WarningCheck(t);
                });
            }

            if (String.IsNullOrEmpty(t.vnode) || t.vnode == "<None>")
            {
                txtfld.text = "<None>";
            }
            else
            {
                txtfld.text = t.vnode;
            }
        }

        if (String.IsNullOrEmpty(t.vnode) || t.vnode == "<None>")
        {
            txtfld.text = "<None>";
        }
        else
        {
            txtfld.text = t.vnode;
        }

        txtfld.MarkDirtyRepaint();
    }
    private void WarningCheck(Call t)
    {
        if (String.IsNullOrEmpty(t.vnode) || t.VGraph == null)
        {
            if (!String.IsNullOrEmpty(t.OnVSummary()))
            {
                if (summary != null)
                    summary.text = "<b>WARNING!</b>\n" + t.OnVSummary();
            }

            int currentIndex = PortsUtils.VGraph.listV.selectedIndex;
            PortsUtils.VGraph.RefreshListV();
            PortsUtils.VGraph.listV.SetSelection(currentIndex);
            PortsUtils.VGraph?.ShowSelectedVblockSerializedFields();
        }
        else
        {
            int currentIndex = PortsUtils.VGraph.listV.selectedIndex;
            PortsUtils.VGraph.RefreshListV();
            PortsUtils.VGraph.listV.SetSelection(currentIndex);
            PortsUtils.VGraph?.ShowSelectedVblockSerializedFields();
        }
    }
    private Box DrawContinueState(Call t)
    {
        //Character's Sprites
        var boxVnodeName = new Box();
        boxVnodeName.style.marginTop = 5;
        boxVnodeName.style.flexDirection = FlexDirection.Row;

        Label strLbl = new Label();
        strLbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strLbl.text = "State : ";
        boxVnodeName.Add(strLbl);

        var objField = new ToolbarMenu();
        objField.style.marginLeft = 4;
        objField.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        objField.text = t.ContinueState.ToString();
        boxVnodeName.Add(objField);

        foreach (var state in Enum.GetValues(typeof(ContinueVBlockState)))
        {
            var str = state.ToString();
            var nuType = (ContinueVBlockState)state;

            objField.menu.AppendAction(str, a =>
            {
                objField.text = str;
                t.ContinueState = nuType;
            });
        }

        return boxVnodeName;
    }
    public void DrawConnectedLines(Call t)
    {
        var nodes = PortsUtils.VGraph.graphView.nodes;
        VPortsInstance thisvport = VBlockUtils.GetVPorts(t, false);
        VPortsInstance thatvport = VBlockUtils.GetVPorts(t, true);

        if (thisvport != null && thatvport != null)
        {
            if (t.triggerer == null)
            {
                if (thatvport.vportInstanceGuid == thisvport.vportInstanceGuid)
                {
                    return;
                }

                TriggererCalls trig = new TriggererCalls
                {
                    thisNodeId = thisvport.vportInstanceGuid,
                    thatNodeId = thatvport.vportInstanceGuid,
                    trigId = t.TriggerId,
                    nodeName = t.vnode
                };

                t.triggerer = trig;
                PortsUtils.ConnectPorts(thisvport, thatvport, true);
            }
            else
            {
                if (thatvport.vportInstanceGuid != t.triggerer.thatNodeId)
                {
                    VPortsInstance vport = null;

                    foreach (var node in nodes)
                    {
                        var astypo = node as VNodes;
                        var asvp = astypo.userData as VPortsInstance;

                        if (asvp == null)
                            continue;

                        if (asvp.vportInstanceGuid == t.triggerer.thatNodeId)
                        {
                            vport = asvp;
                            break;
                        }
                    }

                    if (thatvport.vportInstanceGuid == t.triggerer.thatNodeId)
                    {
                        return;
                    }
                    else if (thatvport.vportInstanceGuid == t.triggerer.thisNodeId && vport != null)
                    {
                        VBlockUtils.DisconnectPorts(t, thisvport, vport, true);
                        t.vnode = string.Empty;
                        t.triggerer = null;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(t.triggerer.thatNodeId) && vport != null)
                        {
                            VBlockUtils.DisconnectPorts(t, thisvport, vport, true);
                        }

                        t.triggerer.thatNodeId = thatvport.vportInstanceGuid;
                        PortsUtils.ConnectPorts(thisvport, thatvport, true);
                    }
                }
            }

            if (t != null && t.gameObject != null)
                EditorUtility.SetDirty(t.gameObject);

            if (PortsUtils.activeVGraphAssets != null)
                EditorUtility.SetDirty(PortsUtils.activeVGraphAssets);
        }
    }

    private void PersistentInvoker(Call t)
    {
        var parentType = this.GetType();
        MethodInfo[] vmethods = parentType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var vmethod in vmethods)
        {
            var mName = vmethod.Name;

            if (vmethod != null && vmethod.Name == "RepopulateNodesGraphs")
            {
                try
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(t.RePoolNodes, 0);
                }
                catch (Exception)
                {
                    //skip
                }

                UnityAction act = Delegate.CreateDelegate(typeof(UnityAction), this, vmethod) as UnityAction;
                UnityEditor.Events.UnityEventTools.AddPersistentListener(t.RePoolNodes, act);
                EditorUtility.SetDirty(t.VGraph.gameObject);
                EditorUtility.SetDirty(PortsUtils.activeVGraphAssets);
                break;
            }
        }
    }
}
