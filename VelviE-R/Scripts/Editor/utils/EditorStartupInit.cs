using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VIEditor;
using VelvieR;
using System.Reflection;
using System;
using System.Linq;
using VTasks;
using UnityEditor.SceneManagement;

///VERY FIRST LOADED!
///Mostly for saves, uielements states and Components in Scripts/VNodes 

[InitializeOnLoad]
static class EditorStartupInit
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void InitNodeTracking()
    {
        EditorApplication.playModeStateChanged -= LogPlayModeState;
        EditorApplication.playModeStateChanged += LogPlayModeState;
        PortsUtils.waitLoading = false;
        
    }
    static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state.ToString().Equals("EnteredPlayMode"))
        {
            PortsUtils.LastPlayedVContainer = PortsUtils.activeVGraphAssets;
            PortsUtils.PlayMode = true;
        }
        else if (state.ToString().Equals("ExitingPlayMode"))
        {
            VTokenManager.CancelAllVTokens();
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            PortsUtils.PlayMode = false;
            PortsUtils.LoadAssets(PortsUtils.LastPlayedVContainer, false);

            if (PortsUtils.VGraph != null)
            {
                PortsUtils.VGraph.Refresh();

                if (PortsUtils.VGraph.inspectorIsActive)
                {
                    PortsUtils.VGraph.HideInspector();

                    if (PortsUtils.activeVNode != null)
                    {
                        PortsUtils.VGraph.graphView.ClearSelection();
                        PortsUtils.activeVNode.OnSelected();
                    }
                }
                else
                {
                    PortsUtils.VGraph.graphView?.ClearSelection();
                    PortsUtils.activeVNode = null;
                }
            }
        }

        if (EditorWindow.HasOpenInstances<VGraphs>())
        {
            VBlockManager.VgraphsOpenInstance = true;
        }
        else
        {
            VBlockManager.VgraphsOpenInstance = false;
        }
    }
    static EditorStartupInit()
    {
        VBlockUtils.vblockComponents = new List<VelvieBlockComponent>();
        var assets = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/VelviE-R/Scripts/VNodes" });

        foreach (var guid in assets)
        {
            var e = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));

            if (e != null)
            {
                //Init without having to instantiate the class! Awesome!                
                //var obj = FormatterServices.GetUninitializedObject(getMonoClass);
                var getMonoClass = e.GetClass();

                var monoType = new VelvieBlockComponent
                {
                    guid = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(0, int.MaxValue),
                    name = getMonoClass.Name,
                    monoComponent = getMonoClass
                };

                //Getting VTag attribute
                Attribute[] attrs = Attribute.GetCustomAttributes(getMonoClass, true);

                if (attrs.Length > 0)
                {
                    if (attrs[0] is VTagAttribute vtag)
                    {
                        monoType.headerValue = vtag.vheaderattr;
                        monoType.summaryValue = vtag.vtooltip;
                        monoType.vcolor = vtag.vcolor;
                        vtag.name = monoType.name;
                    }
                }

                //Get member/method attributes
                MemberInfo[] members = getMonoClass.GetMembers();

                if (members.Length > 0)
                {
                    foreach (var attr in members)
                    {
                        System.Object[] myAttributes = attr.GetCustomAttributes(true);

                        foreach (var at in myAttributes)
                        {
                            if (at is VTagDelay vtag)
                            {
                                if (vtag.delayType == VDelay.OnEnter)
                                {
                                    monoType.onEnterDelay = vtag.delayTime;
                                }
                                else if (vtag.delayType == VDelay.OnExit)
                                {
                                    monoType.onExitDelay = vtag.delayTime;
                                }
                                else if (vtag.delayType == VDelay.None)
                                {
                                    monoType.onEnterDelay = 0;
                                    monoType.onExitDelay = 0;
                                }

                            }
                        }
                    }
                }

                VBlockUtils.PoolVBlockComponent(monoType);
            }
        }
    }
}

[InitializeOnLoadAttribute]
//Detect renamed AND deletion of VGraph object
//TODO : All these are too slow! Fix this later
static class HierarchyMonitor
{
    // register an event handler when the class is initialized
    static HierarchyMonitor()
    {
        if (VBlockManager.ReSerialize == null)
        {
            //VBlockManager.ReSerialize = VVariableUtils.ReSerializeOnEditorPlayMode;
        }

        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    static void OnHierarchyChanged()
    {
        var all = Resources.FindObjectsOfTypeAll<VCoreUtil>();
        var vgraphs = PortsUtils.GetVGprahsScriptableObjects();

        if(all.Length > 0)
        {
            bool nulls = PortsUtils.Vcores.Exists(x => x == null);

            if(nulls || PortsUtils.Vcores.Count == 0)
            {
                PortsUtils.Vcores.Clear();
                PortsUtils.Vcores = all.ToList();
            }
            else
            {
                for(int i = 0; i < all.Length; i++)
                {
                    if(!PortsUtils.Vcores.Contains(all[i]))
                    {
                        PortsUtils.Vcores.Add(all[i]);
                    }
                }
            }
        }

        //check VStages
        var stages = Resources.FindObjectsOfTypeAll<VStageComponent>();

        if (stages.Length > 0)
        {
            for (int i = 0; i < stages.Length; i++)
            {
                stages[i].ReValidate();
            }
        }
        ////////////////////

        //check VelvieDialogue
        var dialogue = Resources.FindObjectsOfTypeAll<VelvieDialogue>();

        if (dialogue.Length > 0)
        {
            for (int i = 0; i < dialogue.Length; i++)
            {
                if(dialogue[i] == null)
                    continue;

                dialogue[i].ReValidate();
                
                for(int j = 0; j < all.Length; j++)
                {
                    if(all[j] == null)
                        continue;

                    if(!all[j].vdialogues.Contains(dialogue[i]))
                        all[j].vdialogues.Add(dialogue[i]);
                }
            }
        }
        //////////////////////

        bool wasRemoved = false;
        bool wasRenamed = false;
        GameObject renamedGameobject = null;
        CheckNames();

        void CheckNames()
        {
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] == null)
                    continue;

                var t = all[i];

                for (int j = 0; j < vgraphs.Count; j++)
                {
                    if (t.vcoreid != vgraphs[j].govcoreid)
                        continue;

                    //Detects renamed vgraph object in the hierarchy
                    if (t.name != vgraphs[j].vgraphGOname)
                    {
                        vgraphs[j].vgraphGOname = t.name;
                        renamedGameobject = t.gameObject;
                        t.vcorename = t.name;
                        wasRenamed = true;
                        return;
                    }
                }
            }
        }

        //Detects removed/deleted Vgraph object in the hierarchy
        if (!wasRenamed)
        {
            if (vgraphs.Count != all.Length)
            {
                bool runOnce = false;
                //it was deleted. Immediately remove VGraphContainer .asset
                for (int i = 0; i < vgraphs.Count; i++)
                {
                    if (vgraphs[i] == null)
                        continue;

                    bool thisGraph = false;

                    for (int j = 0; j < all.Length; j++)
                    {
                        if (all[j] == null)
                            continue;

                        if (all[j].vcoreid == vgraphs[i].govcoreid)
                            thisGraph = true;

                        if (!runOnce)
                        {
                            //Always check if new VelvieDialogue is in the scene's hierarchy
                            runOnce = true;
                        }
                    }

                    if (!thisGraph)
                    {
                        wasRemoved = true;
                        

                        if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() == vgraphs[i].sceneGuid)
                        {
                            Debug.Log("ASSET DELETED!");
                            AssetDatabase.DeleteAsset(vgraphs[i].path);
                        }
                    }
                }
            }
        }

        if (wasRemoved || wasRenamed)
        {
            if (vgraphs.Count > 0)
                PortsUtils.VGraph?.VGraphEntityWindow();

            if (wasRenamed)
            {
                EditorUtility.SetDirty(renamedGameobject);
                EditorUtility.SetDirty(vgraphs.Find(x => x.vgraphGOname == renamedGameobject.name));
            }

        }

        //refresh binds
        for(int i = 0; i < PortsUtils.RefreshBinds.Count; i++)
        {
            if(PortsUtils.RefreshBinds[i] != null && PortsUtils.RefreshBinds[i].editor != null)
                PortsUtils.RefreshBinds[i].act?.Invoke();
        }

        //TODO guard clause this please!
        if(!PortsUtils.PlayMode)
        VUITemplate.RePolAll();
    }
}

