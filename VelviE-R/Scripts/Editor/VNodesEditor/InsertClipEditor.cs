using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(InsertClip))]

    public class InsertClipEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as InsertClip;

            root.Add(DrawAu(t));
            root.Add(DrawClip(t));
            root.Add(DrawPlay(t));
            
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawAu(InsertClip t)
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
        private VisualElement DrawClip(InsertClip t)
        {
            var rootBox = VUITemplate.GetTemplate("Audio clip : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            
            objField.objectType = typeof(AudioClip);
            objField.allowSceneObjects = false;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.auclip;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.auclip = objField.value as AudioClip;
            });

            return rootBox;
        }
        private VisualElement DrawPlay(InsertClip t)
        {
            var rootBox = VUITemplate.GetTemplate("Play : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.play;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.play = objField.value;
            });

            return rootBox;
        }
    }
}