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
        private Box DrawSlider(SetButtonInteractable t)
        {
            var rootBox = VUITemplate.GetTemplate("Button : ");
            var objField = new ObjectField();
            objField.objectType = typeof(UnityEngine.UI.Button);
            objField.allowSceneObjects = true;

            var fld = rootBox.userData as VisualElement;
            objField.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            fld.Add(objField);

            objField.value = t.button;

            if(!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.button = objField.value as UnityEngine.UI.Button;
                });
            }

            return rootBox;
        }
        private Box DrawInteractable(SetButtonInteractable t)
        {
            var rootBox = VUITemplate.GetTemplate("Interactable : ");
            var field = rootBox.userData as VisualElement;

            var objField = new Toggle();
            objField.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            field.Add(objField);
            objField.value = t.interactable;

            if(!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.interactable = objField.value;
                });
            }

            return rootBox;
        }
    }
}