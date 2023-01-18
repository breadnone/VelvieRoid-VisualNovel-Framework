using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using UnityEngine.Audio;
namespace VIEditor
{
    [CustomEditor(typeof(ChangeOutput))]

    public class ChangeOutputEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as ChangeOutput;
            dummy = new VisualElement();

            root.Add(DrawAu(t));
            root.Add(DrawMixer(t));
            root.Add(DrawPlay(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawAu(ChangeOutput t)
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
        private VisualElement DrawMixer(ChangeOutput t)
        {
            var rootBox = VUITemplate.GetTemplate("Mixer : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(AudioMixerGroup);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.mixerGroup;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.mixerGroup = objField.value as AudioMixerGroup;
            });

            return rootBox;
        }
        private VisualElement DrawPlay(ChangeOutput t)
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