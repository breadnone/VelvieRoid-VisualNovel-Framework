using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(SetSlider))]
    public class SetSliderEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetSlider;

            root.Add(DrawSlider(t));
            root.Add(DrawValue(t));
            root.Add(DrawLerp(t));
            root.Add(DrawDuration(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawSlider(SetSlider t)
        {
            var rootBox = VUITemplate.GetTemplate("Slider : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(UnityEngine.UI.Slider);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.slider;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.slider = objField.value as UnityEngine.UI.Slider;
                });
            }

            return rootBox;
        }
        private VisualElement DrawTypeWrite(SetText t)
        {
            var rootBox = VUITemplate.GetTemplate("Typewriter : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.isTypewriter;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.isTypewriter = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawWait(SetSlider t)
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
        private VisualElement DrawLerp(SetSlider t)
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
                });
            }

            return rootBox;
        }
        private VisualElement DrawDuration(SetSlider t)
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
        private VisualElement DrawValue(SetSlider t)
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
    }
}