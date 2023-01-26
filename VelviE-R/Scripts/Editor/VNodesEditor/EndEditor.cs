using UnityEditor;
using VelvieR;
using UnityEngine.UIElements;

// NOTE to future maintainer, have fun refactoring this -____-
//I'm the creator, will NEVER revisit this forbidden hell ever again! 

namespace VIEditor
{
    [CustomEditor(typeof(End))]
    public class EndEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {

            VisualElement root = new VisualElement();
            var t = target as End;

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

    }
}