using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace VIEditor
{
    [CustomPropertyDrawer(typeof(DefaultInspectorToUIElementTest))]
    public class VBlock : PropertyDrawer
    {
        public VBlock(string scriptName, string titleName)
        {
            scpName = scriptName;
            ttlname = titleName;
        }

        private string scpName = string.Empty;
        private string ttlname = string.Empty;
        private Box box = new Box();
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            UnityEngine.Random.InitState(property.displayName.GetHashCode());
            container.style.backgroundColor = UnityEngine.Random.ColorHSV();

            //Get the class
            DefaultInspectorToUIElementTest t = new DefaultInspectorToUIElementTest();

            SerializedObject serializedObject = new UnityEditor.SerializedObject(t);

            var iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                do
                {
                    var propertyField = new PropertyField(iterator.Copy()) { name = ttlname + iterator.propertyPath };

                    if (serializedObject.targetObject != null)
                        box.Add(propertyField);
                }
                while (iterator.NextVisible(false));
            }

            return container;
        }
    }
}