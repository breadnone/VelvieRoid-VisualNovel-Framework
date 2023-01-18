using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using TMPro;

namespace VIEditor
{
    [CustomEditor(typeof(SetText))]
    public class SetTextEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetText;

            root.Add(DrawTMP(t));
            root.Add(DrawTextArea(t));
            root.Add(DrawPauses(t));
            root.Add(DrawTypeWrite(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawTMP(SetText t)
        {
            var rootBox = VUITemplate.GetTemplate("TextmeshPro : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(TMP_Text);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.text;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.text = objField.value as TMP_Text;
            });

            return rootBox;
        }
        private VisualElement DrawTextArea(SetText t)
        {
            var rootBox = VUITemplate.GetTemplate("Words : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.style.flexDirection = FlexDirection.Column;
            objField.style.overflow = Overflow.Visible;
            objField.style.whiteSpace = WhiteSpace.Normal;
            objField.style.unityOverflowClipBox = OverflowClipBox.ContentBox;
            objField.style.height = 100;
            objField.multiline = true;
            objField.value = t.content;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.content = objField.value;
            });

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

            objField.RegisterValueChangedCallback((x) =>
            {
                t.isTypewriter = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawWait(SetText t)
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
        private VisualElement DrawPauses(SetText t)
        {
            var rootBox = VUITemplate.GetTemplate("PausesBetweenWords : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.pauseBetweenWords;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.pauseBetweenWords = objField.value;
            });

            return rootBox;
        }
    }
}