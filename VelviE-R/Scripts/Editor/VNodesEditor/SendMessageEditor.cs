using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(SendMessage))]
    public class SendMessageEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SendMessage;

            root.Add(DrawObject(t));
            root.Add(DrawString(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root; 
        }
        private VisualElement DrawObject(SendMessage t)
        {
            var rootBox = VUITemplate.GetTemplate("Target object : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.gameobject;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.gameobject = objField.value as GameObject;
            });

            return rootBox;
        }
        private VisualElement DrawString(SendMessage t)
        {
            var rootBox = VUITemplate.GetTemplate("Method to execute : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.method;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.method = objField.value;
            });

            return rootBox;
        }
    }
}