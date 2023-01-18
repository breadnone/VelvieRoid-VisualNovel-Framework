using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using VelvieR;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VIEditor
{
    [CustomEditor(typeof(VelvieR.Rotate))]
    public class RotateEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as VelvieR.Rotate;
            root.Add(DrawObject(t));
            root.Add(DrawVector3Type(t));
            root.Add(DrawAngle(t));
            root.Add(DrawLocal(t));
            //root.Add(DrawVector(t));
            root.Add(DrawDuration(t));
            root.Add(DrawLoopCount(t));
            root.Add(DrawTarget(t));
            root.Add(DrawLoopType(t));
            root.Add(DrawEase(t));
            root.Add(DrawEnableOnStart(t));
            root.Add(DrawDisableOnFinished(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private VisualElement DrawObject(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Object to rotate : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(GameObject);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetobject;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.targetobject = objField.value as GameObject;
            });

            return rootBox;
        }
        private VisualElement DrawVector(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("To : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.to;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.to = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawDuration(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.duration = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawAngle(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Angle : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.angle;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.angle = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawEase(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Ease : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.ease.ToString();
            objField.choices = Enum.GetNames(typeof(LeanTweenType)).ToList();
            objField.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    foreach(var asEnum in Enum.GetValues(typeof(LeanTweenType)))
                    {
                        var asetype = (LeanTweenType)asEnum;

                        if(x.newValue == asetype.ToString())
                        {
                            t.ease = asetype;
                        }
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawLoopCount(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop count : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loopCount;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.loopCount = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawLoopType(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.loopType.ToString();
            var list = new List<string>{"Clamp", "PingPong"};
            objField.choices = list;

            objField.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    foreach(var asEnum in Enum.GetValues(typeof(LeanTweenType)))
                    {
                        var asetype = (LeanTweenType)asEnum;

                        if(x.newValue.Contains(asetype.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            t.ease = asetype;
                            break;
                        }
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawWait(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("WaitUntilFinished : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.waitUntilFinished = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawLocal(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("IsLocal : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.isLocal;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.isLocal = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawEnableOnStart(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("EnableOnStart : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.enableOnStart;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.enableOnStart = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawDisableOnFinished(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("DisableOnComplete : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.disableOnComplete;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.disableOnComplete = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawTarget(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Rotate to target : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.target;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.target = objField.value as Transform;
            });

            return rootBox;
        }
        private VisualElement DrawVector3Type(VelvieR.Rotate t)
        {
            var rootBox = VUITemplate.GetTemplate("Vector3 type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.rotateType.ToString();
            var list = Enum.GetNames(typeof(RotateType)).ToList();
            objField.choices = list;

            objField.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    foreach(var asEnum in Enum.GetValues(typeof(LeanTweenType)))
                    {
                        var asetype = (LeanTweenType)asEnum;

                        if(x.newValue.Contains(asetype.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            t.ease = asetype;
                            break;
                        }
                    }
                }
            });

            return rootBox;
        }
    }
}