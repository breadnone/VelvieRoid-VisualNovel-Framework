using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(SetTimeScale))]
    public class SetTimeScaleEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as SetTimeScale;
            root.Add(DrawReset(t));
            root.Add(DrawValue(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawValue(SetTimeScale t)
        {
            var rootBox = VUITemplate.GetTemplate("Value : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.value;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.value = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawReset(SetTimeScale t)
        {
            var rootBox = VUITemplate.GetTemplate("Reset : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.resetToDefault;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.resetToDefault = objField.value;
                });
            }

            return rootBox;
        }
    }
}