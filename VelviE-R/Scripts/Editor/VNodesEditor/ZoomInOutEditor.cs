using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(ZoomInOutCamera))]
    public class ZoomInOutEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as ZoomInOutCamera;
            root.Add(DrawCamera(t));
            root.Add(DrawValue(t));
            root.Add(DrawAnim(t));
            root.Add(DrawDuration(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawValue(ZoomInOutCamera t)
        {
            var rootBox = VUITemplate.GetTemplate("Zoom value : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.to;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.to = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawDuration(ZoomInOutCamera t)
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
        private VisualElement DrawAnim(ZoomInOutCamera t)
        {
            var rootBox = VUITemplate.GetTemplate("Animate : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.animate;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.animate = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawWait(ZoomInOutCamera t)
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
        private VisualElement DrawCamera(ZoomInOutCamera t)
        {
            var rootBox = VUITemplate.GetTemplate("Camera : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Camera);
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.cam;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.cam = objField.value as Camera;
            });

            return rootBox;
        }
    }
}