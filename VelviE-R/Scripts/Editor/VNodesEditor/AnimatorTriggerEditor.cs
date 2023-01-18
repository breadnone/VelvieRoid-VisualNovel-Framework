using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VIEditor
{
    [CustomEditor(typeof(AnimatorTrigger))]
    public class AnimatorTriggerEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as AnimatorTrigger;
            root.Add(DrawObj(t));
            root.Add(DrawParam(t));
            
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        public VisualElement DrawObj(AnimatorTrigger t)
        {
            var rootBox = VUITemplate.GetTemplate("Animator : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();

            objField.objectType = typeof(Animator);
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.animator;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.animator = objField.value as Animator;
            });

            return rootBox;
        }
        public VisualElement DrawParam(AnimatorTrigger t)
        {
            var rootBox = VUITemplate.GetTemplate("Trigger : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.parameter;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.parameter = x.newValue;
            });

            return rootBox;
        }
    }
}