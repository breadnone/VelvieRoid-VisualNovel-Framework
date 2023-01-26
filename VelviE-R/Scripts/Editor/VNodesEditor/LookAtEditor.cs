using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(LookAt))]
    public class LookAtEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as LookAt;
            root.Add(DrawEnable(t));
            root.Add(DrawTargetObject(t));
            root.Add(DrawObjectFollowing(t));
            root.Add(DrawLookAtType(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawEnable(LookAt t)
        {
            var rootBox = VUITemplate.GetTemplate("Enable : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.enable;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.enable = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawTargetObject(LookAt t)
        {
            var rootBox = VUITemplate.GetTemplate("Target to look : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetToFollow as Transform;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.targetToFollow = objField.value as Transform;
                });
            }

            return rootBox;
        }
        private VisualElement DrawObjectFollowing(LookAt t)
        {
            var rootBox = VUITemplate.GetTemplate("Object looking : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.objectFollowing;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.objectFollowing = objField.value as Transform;
                });
            }

            return rootBox;
        }
        private VisualElement DrawLookAtType(LookAt t)
        {
            var rootBox = VUITemplate.GetTemplate("Mode : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.vecType.ToString();

            if (!PortsUtils.PlayMode)
            {
                objField.choices = Enum.GetNames(typeof(RotateType)).ToList();
                objField.RegisterCallback<ChangeEvent<string>>((x) =>
                {
                    foreach (var asEnum in Enum.GetValues(typeof(RotateType)))
                    {
                        var asetype = (RotateType)asEnum;

                        if (x.newValue == asetype.ToString())
                        {
                            t.vecType = asetype;
                        }
                    }
                });
            }

            return rootBox;
        }
    }
}