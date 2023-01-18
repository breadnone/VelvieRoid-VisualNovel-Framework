using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using System;
using System.Linq;
using VelvieR;
using UnityEngine.UIElements;

namespace VIEditor
{
    [CustomEditor(typeof(MoveSpline))]
    public class MoveSplineEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as MoveSpline;
            root.Add(DrawObject(t));
            root.Add(DrawVector(t));
            root.Add(DrawDuration(t));
            root.Add(DrawEase(t));
            root.Add(DrawDisableOnFinished(t));
            root.Add(DrawLocal(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private VisualElement DrawObject(MoveSpline t)
        {
            var rootBox = VUITemplate.GetTemplate("Object to move : ");
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
        private VisualElement DrawVector(MoveSpline t)
        {
            var rootBox = VUITemplate.GetTemplate("Vector points : ");
            var field = VUITemplate.GetField(rootBox);

            Func<Vector3Field> makeItem = () => new Vector3Field();

            Action<VisualElement, int> bindItem = (e, i) => 
            {
                var astype = e as Vector3Field;                
                astype.value = t.splinePoint[i];
                int idx = i;

                astype.RegisterValueChangedCallback((x)=>
                {
                    if(!PortsUtils.PlayMode)
                    t.splinePoint[i] = x.newValue;
                });
            };

            var btnAdd = new Button();
            btnAdd.style.width = 30;
            btnAdd.style.height = 20;
            btnAdd.text = "+";

            var btnRem = new Button();
            btnRem.style.width = 30;
            btnRem.style.height = 20;
            btnRem.text = "-";

            const int itemHeight = 20;
            var objField = new ListView(t.splinePoint, itemHeight, makeItem, bindItem);
            objField.reorderable = false;
            objField.selectionType = SelectionType.Single;
            objField.showBorder = true;
            objField.style.width = field.style.width;
            field.style.flexDirection = FlexDirection.Column;
            field.Add(objField);

            btnAdd.clicked += ()=>
            {
                t.splinePoint.Add(Vector3.zero);
                objField.Rebuild();
            };
            btnRem.clicked += ()=>
            {
                if(objField.selectedItem != null)
                {
                    t.splinePoint.RemoveAt(objField.selectedIndex);
                }
                else
                {
                    if(t.splinePoint.Count > 0)
                    t.splinePoint.RemoveAt(t.splinePoint.Count - 1);
                }
                objField.Rebuild();
            };
            var visEl = new VisualElement();
            visEl.style.flexDirection = FlexDirection.Row;
            field.Add(visEl);

            visEl.Add(btnAdd);
            visEl.Add(btnRem);

            return rootBox;
        }
        private VisualElement DrawDuration(MoveSpline t)
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
        private VisualElement DrawEase(MoveSpline t)
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
        private VisualElement DrawWait(MoveSpline t)
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
        private VisualElement DrawEnableOnStart(MoveSpline t)
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
        private VisualElement DrawDisableOnFinished(MoveSpline t)
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
        private VisualElement DrawLocal(MoveSpline t)
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
    }
}