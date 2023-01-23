using System.Collections.Generic;
using UnityEngine;
using VelvieR;
using System;
using UnityEditor;

namespace VIEditor
{
    public static class VGraphUtil
    {
        private static List<GameObject> vGraphGameObjects = new List<GameObject>();
        public static GameObject activeVgraphGameObject { get; set; }

        //Create VGraph object into the scene
        public static void CreateVGraph()
        {
            if (PortsUtils.VGraph != null)
            {
                //Vgraph object
                GameObject vg = new GameObject();
                vg.name = GetNames();
                vg.AddComponent<VCoreUtil>();
                vg.AddComponent<VInputBuffer>();
                VEditorFunc.AssignAndAddTag(vg);

                vGraphGameObjects.Add(vg);
                var com = vg.GetComponent<VCoreUtil>();
                var vid = com.vcoreid;
                PortsUtils.CreateSaveAsset(vg, vid, vg.name, com);
                var varAsset = PortsUtils.GetVariableScriptableObjects();

                if(varAsset == null || varAsset.Count == 0)
                {
                    PortsUtils.CreateVariableAsset();
                }
                
                var getcom = vg.GetComponent<VInputBuffer>();
                
                if(getcom != null)
                {
                    getcom.hideFlags = HideFlags.HideInInspector;
                }
            }
        }
        private static void CreateSceneUtil()
        {
            //Scenee object reference check. Always!
            var findObj = Resources.FindObjectsOfTypeAll<SceneRefUtil>();

            //unhidden
            //findObj[0].gameObject.hideFlags = HideFlags.None;
            if(findObj == null || findObj.Length == 0)
            {
                GameObject sceneUtil = new GameObject();
                sceneUtil.name = "VelvieManager-19851954-hp-nf";
                sceneUtil.AddComponent<SceneRefUtil>();
                //sceneUtil.hideFlags = HideFlags.HideInHierarchy;

                #if UNITY_EDITOR
                Debug.LogWarning(sceneUtil.name + " VLog : Created and is hidden in hiearchy. Used by VelvieR for sceneObject references!");
                #endif
            }
        }
        public static string GetNames(string strName = "VGraph")
        {
            var t = string.Empty;

            var graphObjs = PortsUtils.GetVGprahsScriptableObjects();
            int cnt = (graphObjs.Count - 1) + 1;

            if (graphObjs.Count > 0)
            {
                t = strName + cnt;
            }
            else
            {
                t = strName;
            }

            return t;
        }

        //Remove VGraph gameObject in the scene
        public static void RemoveVgraph(GameObject vg)
        {
            if (vg != null)
                vGraphGameObjects.Remove(vg);
        }

        //Get any VCoreUtil. Not important!
        public static VCoreUtil FindVCoreUtil()
        {
            VCoreUtil t = null;

            foreach (var e in vGraphGameObjects)
            {
                if (e != null)
                {
                    t = e.GetComponent<VCoreUtil>();
                    break;
                }
            }
            return t;
        }
    }
}