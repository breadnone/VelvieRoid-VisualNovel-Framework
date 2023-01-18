using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(Volume))]

    public class VolumeEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as Volume;
            dummy = new VisualElement();

            root.Add(DrawAu(t));
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
        private VisualElement DrawAu(Volume t)
        {
            var rootBox = VUITemplate.GetTemplate("AudioSource : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(AudioSource);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.audioSource;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.audioSource = objField.value as AudioSource;
            });

            return rootBox;
        }
        private VisualElement DrawVal(Volume t)
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
        private VisualElement DrawWait(Volume t)
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
        private VisualElement DrawLerp(Volume t)
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
        private VisualElement DrawDuration(Volume t)
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