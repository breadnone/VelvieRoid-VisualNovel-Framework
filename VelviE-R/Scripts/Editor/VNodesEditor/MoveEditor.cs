using System.Collections.Generic;
using UnityEngine;
using VelvieR;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(Move))]
    public class MoveEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as Move;
            root.Add(DrawObject(t));
            root.Add(DrawVector(t));
            root.Add(DrawDuration(t));
            root.Add(DrawLoopCount(t));
            root.Add(DrawTarget(t));
            root.Add(DrawLoopType(t));
            root.Add(DrawEase(t));
            root.Add(DrawEnableOnStart(t));
            root.Add(DrawDisableOnFinished(t));
            root.Add(DrawLocal(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private VisualElement DrawObject(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("Object to move : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetobject;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.targetobject = objField.value as GameObject;
                });
            }

            return rootBox;
        }
        private VisualElement DrawVector(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("To : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.to;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.to = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawDuration(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.duration = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawEase(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("Ease : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.ease.ToString();

            if (!PortsUtils.PlayMode)
            {
                objField.choices = Enum.GetNames(typeof(LeanTweenType)).ToList();
                objField.RegisterCallback<ChangeEvent<string>>((x) =>
                {
                    foreach (var asEnum in Enum.GetValues(typeof(LeanTweenType)))
                    {
                        var asetype = (LeanTweenType)asEnum;

                        if (x.newValue == asetype.ToString())
                        {
                            t.ease = asetype;
                        }
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawLoopCount(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop count : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loopCount;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.loopCount = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawLoopType(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop type : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loopType.ToString();
            var list = new List<string> { "Clamp", "PingPong" };
            objField.choices = list;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterCallback<ChangeEvent<string>>((x) =>
                {
                    foreach (var asEnum in Enum.GetValues(typeof(LeanTweenType)))
                    {
                        var asetype = (LeanTweenType)asEnum;

                        if (x.newValue.Contains(asetype.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            t.ease = asetype;
                            break;
                        }
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawWait(Move t)
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
                    t.waitUntilFinished = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawEnableOnStart(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("EnableOnStart : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.enableOnStart;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.enableOnStart = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawDisableOnFinished(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("DisableOnComplete : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.disableOnComplete;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.disableOnComplete = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawLocal(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("IsLocal : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.isLocal;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.isLocal = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawTarget(Move t)
        {
            var rootBox = VUITemplate.GetTemplate("Move to target : ");
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
    }
}