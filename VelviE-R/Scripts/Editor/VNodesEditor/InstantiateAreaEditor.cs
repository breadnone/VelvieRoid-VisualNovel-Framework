using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(InstantiateArea))]
    public class InstantiateAreaEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as InstantiateArea;
            root.Add(DrawObject(t));
            root.Add(DrawTarget(t));
            root.Add(DrawAmount(t));
            root.Add(DrawRandomRotation(t));
            root.Add(DrawName(t));
            root.Add(DrawPrefix(t));
            root.Add(DrawManager(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private VisualElement DrawObject(InstantiateArea t)
        {
            var rootBox = VUITemplate.GetTemplate("Target to duplicate : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetObject;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.targetObject = objField.value as GameObject;
            });

            return rootBox;
        }
        private VisualElement DrawTarget(InstantiateArea t)
        {
            var rootBox = VUITemplate.GetTemplate("Cube object : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.cubeObject;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.cubeObject = objField.value as GameObject;
            });

            return rootBox;
        }
        private VisualElement DrawAmount(InstantiateArea t)
        {
            var rootBox = VUITemplate.GetTemplate("Amount : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.amount;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.amount = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawName(InstantiateArea t)
        {
            var rootBox = VUITemplate.GetTemplate("Custom name : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.customName;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.customName = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawPrefix(InstantiateArea t)
        {
            var rootBox = VUITemplate.GetTemplate("Prefix : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.prefixes;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.prefixes = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawManager(InstantiateArea t)
        {
            var rootBox = VUITemplate.GetTemplate("Add to manager : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.addToGameObjectManager;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.addToGameObjectManager = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawRandomRotation(InstantiateArea t)
        {
            var rootBox = VUITemplate.GetTemplate("Random rotation : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.randomRotation;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.randomRotation = objField.value;
            });

            return rootBox;
        }
    }
}