using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VelvieR;
using TMPro;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;

namespace VIEditor
{
    public static class VVariableUtils
    {
        public static Action ReSerialize { get; set; }
        public static VJsonAdapter vjson { get; set; }
        public static VariableDrawerWindow ActiveVariableWindow { get; set; }
        public static bool variableWindowIsActive { get; set; }

        public static void CreateEmptyVariable(ListView list)
        {
            if(list == null)
                return;
                
            //Default is int type
            var initType = new VInteger{name = GetVarNames("Var"), guid = PortsUtils.variable.GlobalCounter++};
            initType.IsPublic = false;
            initType.sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString();
            PortsUtils.variable.ivar.Add(initType);
            PortsUtils.SetActiveAssetDirty();
            list.Rebuild();
        }
        public static string GetVarNames(string inName)
        {
            int counta = 0;
            var vname = inName;
            ReCheck();

            void ReCheck()
            {
                if(counta != 0)
                {
                    if(Char.IsDigit(vname[vname.Length -1]))
                    {
                        var len = vname.Length - 1;
                        vname = vname[0..len];
                    }
                    
                    vname += counta;
                }

                if(PortsUtils.variable.ivar.Exists(x => x.Name == vname))
                {
                    counta++;
                    ReCheck();
                    return;
                }
            }

            return vname;
        }
    }
}