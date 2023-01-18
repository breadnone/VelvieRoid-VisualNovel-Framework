
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.IO;
using VIEditor;
public class VProcessBuild : IPreprocessBuildWithReport
{
      public int callbackOrder { get { return 0; } }
      public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report) 
      {
          var vcontainers = PortsUtils.GetVGprahsScriptableObjects();
          foreach(var t in vcontainers)
          {
              //VVariableUtils.ReSerializeOnEditorPlayMode(t.govcoreid);
          }
      }
}
#endif