using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    public class SetButtonInteractableEditor : Editor
    {
        [CustomEditor(typeof(SetButtonInteractable))]
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetButtonInteractable;

            root.Add(DrawSlider(t));
            root.Add(DrawInteractable(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawSlider(SetButtonInteractable t)
        {
            var rootBox = VUITemplate.GetTemplate("Button : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(UnityEngine.UI.Button);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.button;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.button = objField.value as UnityEngine.UI.Button;
            });

            return rootBox;
        }
        private VisualElement DrawInteractable(SetButtonInteractable t)
        {
            var rootBox = VUITemplate.GetTemplate("Interactable : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.interactable;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.interactable = objField.value;
            });

            return rootBox;
        }
    }
}