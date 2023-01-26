using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(PanStereo))]

    public class PanStereoEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as PanStereo;
            dummy = new VisualElement();

            root.Add(DrawAu(t));
            root.Add(DrawVal(t));
            root.Add(dummy);
            dummy.Add(DrawDuration(t));
            dummy.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawAu(PanStereo t)
        {
            var rootBox = VUITemplate.GetTemplate("AudioSource : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(AudioSource);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.audioSource;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.audioSource = objField.value as AudioSource;
                });
            }

            return rootBox;
        }
        private VisualElement DrawVal(PanStereo t)
        {
            var rootBox = VUITemplate.GetTemplate("Value : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Slider();
            objField.highValue = 1f;
            objField.lowValue = -1f;
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
        private VisualElement DrawWait(PanStereo t)
        {
            var rootBox = VUITemplate.GetTemplate("WaitUntilFinished : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.waitUntilFinished = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawDuration(PanStereo t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.duration = objField.value;
                });
            }

            return rootBox;
        }
    }
}