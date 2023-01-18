using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(PortraitEffects))]
    public class PortraitEffectEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as PortraitEffects;

            root.Add(DrawEffect(t));
            root.Add(DrawObj(t));
            root.Add(DrawPower(t));
            root.Add(DrawLoopCount(t));
            root.Add(DrawDuration(t));
            root.Add(DrawWait(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }

        public VisualElement DrawEffect(PortraitEffects t)
        {
            var rootBox = VUITemplate.GetTemplate("Type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.effects.ToString();
            objField.choices = Enum.GetNames(typeof(VStageEffects)).ToList();

            objField.RegisterValueChangedCallback((x) =>
            {
                foreach(var astype in Enum.GetValues(typeof(VStageEffects)))
                {
                    var asori = (VStageEffects)astype;

                    if(x.newValue == asori.ToString())
                    {
                        t.effects = asori;
                        break;
                    }
                }
            });

            return rootBox;
        }
        public VisualElement DrawObj(PortraitEffects t)
        {
            var rootBox = VUITemplate.GetTemplate("Vstage : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();

            objField.objectType = typeof(VStageUtil);
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.vstage;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.vstage = objField.value as VStageUtil;
            });

            return rootBox;
        }
        public VisualElement DrawPower(PortraitEffects t)
        {
            var rootBox = VUITemplate.GetTemplate("Power : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.power;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.power = x.newValue;
            });

            return rootBox;
        }
        public VisualElement DrawDuration(PortraitEffects t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.duration = x.newValue;
            });

            return rootBox;
        }
        public VisualElement DrawLoopCount(PortraitEffects t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop count : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loopCount;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.loopCount = x.newValue;
            });

            return rootBox;
        }
        public VisualElement DrawWait(PortraitEffects t)
        {
            var rootBox = VUITemplate.GetTemplate("WaitUntilFinished : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            objField.RegisterValueChangedCallback((x) =>
            {
                t.waitUntilFinished = x.newValue;
            });

            return rootBox;
        }
    }
}