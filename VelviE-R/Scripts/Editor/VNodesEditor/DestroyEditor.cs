using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(Destroy))]
    public class DestroyEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as Destroy;

            root.Add(DrawObject(t));
            root.Add(DrawDelay(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawObject(Destroy t)
        {
            var rootBox = VUITemplate.GetTemplate("Object to destroy : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetObject;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.targetObject = objField.value as GameObject;
                });
            }
            
            return rootBox;
        }
        private VisualElement DrawDelay(Destroy t)
        {
            var rootBox = VUITemplate.GetTemplate("Delay in seconds : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.destroyAfter;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.destroyAfter = objField.value;
                });
            }

            return rootBox;
        }
    }
}