using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(Pitch))]

    public class PitchEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as Pitch;
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
        private VisualElement DrawAu(Pitch t)
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
        private VisualElement DrawVal(Pitch t)
        {
            var rootBox = VUITemplate.GetTemplate("Pitch level : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Slider();
            objField.highValue = 3f;
            objField.lowValue = -3f;
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
        private VisualElement DrawWait(Pitch t)
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
        private VisualElement DrawLerp(Pitch t)
        {
            var rootBox = VUITemplate.GetTemplate("Lerp : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.lerp;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.lerp = objField.value;
                    dummy.SetEnabled(t.lerp);
                });
            }

            return rootBox;
        }
        private VisualElement DrawDuration(Pitch t)
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