using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(Instantiate))]
    public class InstantiateEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as Instantiate;
            root.Add(DrawObject(t));
            root.Add(DrawVector(t));
            root.Add(DrawTarget(t));
            root.Add(DrawAmount(t));
            root.Add(DrawName(t));
            root.Add(DrawPrefix(t));
            root.Add(DrawManager(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private VisualElement DrawObject(Instantiate t)
        {
            var rootBox = VUITemplate.GetTemplate("Target to duplicate : ");
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
        private VisualElement DrawVector(Instantiate t)
        {
            var rootBox = VUITemplate.GetTemplate("Position : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.position;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.position = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawTarget(Instantiate t)
        {
            var rootBox = VUITemplate.GetTemplate("Copy transform : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.target;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.target = objField.value as Transform;
                });
            }

            return rootBox;
        }
        private VisualElement DrawAmount(Instantiate t)
        {
            var rootBox = VUITemplate.GetTemplate("Amount : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.amount;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.amount = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawName(Instantiate t)
        {
            var rootBox = VUITemplate.GetTemplate("Custom name : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.customName;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.customName = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawPrefix(Instantiate t)
        {
            var rootBox = VUITemplate.GetTemplate("Prefix : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.prefixes;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.prefixes = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawManager(Instantiate t)
        {
            var rootBox = VUITemplate.GetTemplate("Add to manager : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.addToGameObjectManager;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.addToGameObjectManager = objField.value;
                });
            }

            return rootBox;
        }
    }
}