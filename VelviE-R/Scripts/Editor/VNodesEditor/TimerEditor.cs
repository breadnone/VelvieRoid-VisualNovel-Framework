using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using TMPro;

namespace VIEditor
{
    [CustomEditor(typeof(Timer))]
    public class TimerEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            var t = target as Timer;

            root.Add(DrawText(t));
            root.Add(DrawStart(t));
            root.Add(DrawEnd(t));
            root.Add(DrawFormat(t));
            root.Add(DrawWait(t));

            return root;
        }

        private VisualElement DrawText(Timer t)
        {
            var rootBox = VUITemplate.GetTemplate("Text component : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(TMP_Text);
            objField.objectType = typeof(TMP_Text);
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.text;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.text = x.newValue as TMP_Text;
                });
            }

            return rootBox;
        }
        private VisualElement DrawFormat(Timer t)
        {
            var rootBox = VUITemplate.GetTemplate("Format : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.format;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.format = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawStart(Timer t)
        {
            var rootBox = VUITemplate.GetTemplate("Start : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.start;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.start = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawEnd(Timer t)
        {
            var rootBox = VUITemplate.GetTemplate("End : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.end;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.end = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawWait(Timer t)
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
                    t.waitUntilFinished = x.newValue;
                });
            }

            return rootBox;
        }
    }
}