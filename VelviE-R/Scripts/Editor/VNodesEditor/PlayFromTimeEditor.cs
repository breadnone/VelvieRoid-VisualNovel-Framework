using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(PlayFromTime))]

    public class PlayFromTimeEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as PlayFromTime;
            dummy = new VisualElement();

            root.Add(DrawAu(t));
            root.Add(DrawStart(t));
            root.Add(DrawEnd(t));
            root.Add(DrawToggle(t));
            root.Add(DrawLoop(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawAu(PlayFromTime t)
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
        private VisualElement DrawStart(PlayFromTime t)
        {
            var rootBox = VUITemplate.GetTemplate("Start : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.start;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.start = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawEnd(PlayFromTime t)
        {
            var rootBox = VUITemplate.GetTemplate("End : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.start;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.start = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawToggle(PlayFromTime t)
        {
            var rootBox = VUITemplate.GetTemplate("UseLengthToStop : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.useLengthToStop;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.useLengthToStop = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawLoop(PlayFromTime t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loop;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.loop = objField.value;
            });

            return rootBox;
        }
    }
}