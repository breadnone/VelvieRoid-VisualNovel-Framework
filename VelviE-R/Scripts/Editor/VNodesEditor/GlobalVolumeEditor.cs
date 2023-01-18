using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(GlobalVolume))]

    public class GlobalVolumeEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as GlobalVolume;
            dummy = new VisualElement();

            root.Add(DrawVal(t));
            root.Add(DrawLerp(t));

            root.Add(dummy);
            dummy.Add(DrawDuration(t));
            dummy.Add(DrawWait(t));

            dummy.SetEnabled(t.lerp);

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }

        private VisualElement DrawVal(GlobalVolume t)
        {
            var rootBox = VUITemplate.GetTemplate("Volume : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Slider();
            objField.highValue = 1f;
            objField.lowValue = 0f;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.value;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.value = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawWait(GlobalVolume t)
        {
            var rootBox = VUITemplate.GetTemplate("WaitUntilFinished : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.waitUntilFinished = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawLerp(GlobalVolume t)
        {
            var rootBox = VUITemplate.GetTemplate("Lerp : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.lerp;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.lerp = objField.value;
                dummy.SetEnabled(t.lerp);
            });

            return rootBox;
        }
        private VisualElement DrawDuration(GlobalVolume t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.duration = objField.value;
            });

            return rootBox;
        }
    }
}