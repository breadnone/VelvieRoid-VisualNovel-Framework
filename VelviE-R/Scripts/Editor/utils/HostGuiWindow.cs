using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VIEditor
{
    public class HostGuiWindow : EditorWindow
    {
        [MenuItem("VelviE-R/Velvie-R")]
        public static void OpenHostWindow()
        {
            var window = GetWindow<HostGuiWindow>();
            //var window = ScriptableObject.CreateInstance<VGraphs>();
            window.titleContent = new GUIContent("Velvie-R");
            window.titleContent.tooltip = "Velvie Main Window";

            var windowGraph = GetWindow<VGraphs>(typeof(HostGuiWindow));
            //var window = ScriptableObject.CreateInstance<VGraphs>();
            windowGraph.titleContent = new GUIContent("VRGraph");
            windowGraph.titleContent.tooltip = "Flowchart Graph";

            var windowCharacter = GetWindow<VCharacter>(typeof(HostGuiWindow));
            //var window = ScriptableObject.CreateInstance<VCharacter>();
            windowCharacter.titleContent = new GUIContent("VCharacter");
            windowCharacter.titleContent.tooltip = "Character Creation";

            var thumbnailAnimation = GetWindow<VAnimatableThumbnail>(typeof(HostGuiWindow));
            //var window = ScriptableObject.CreateInstance<VCharacter>();
            thumbnailAnimation.titleContent = new GUIContent("VAnimatable Thumbnail");
            thumbnailAnimation.titleContent.tooltip = "Frame by frame animation for character's portraits";
        }
    }
}
