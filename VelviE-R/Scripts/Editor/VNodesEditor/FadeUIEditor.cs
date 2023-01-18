using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(FadeUI))]
    public class FadeUIEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as FadeUI;

            root.Add(DrawUI(t));
            root.Add(DrawFadeFrom(t));
            root.Add(DrawFadeTo(t));
            root.Add(DrawDuration(t));
            root.Add(DrawLoopCount(t));
            root.Add(DrawLoopType(t));
            root.Add(DrawEase(t));
            root.Add(DrawWait(t));
            root.Add(DrawEnableOnStart(t));
            root.Add(DrawDisableOnFinished(t));


            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawUI(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("UI object : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(RectTransform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.rect;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.rect = objField.value as RectTransform;
            });

            return rootBox;
        }
        private VisualElement DrawFadeFrom(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("Fade from : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.fadeFrom;
            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.fadeFrom = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawFadeTo(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("Fade to : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.fadeTo;
            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.fadeTo = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawDuration(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;
            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.duration = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawLoopCount(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("Loop count : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.loopCount;
            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.loopCount = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawWait(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("WaituntilFinished : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.waitUntilFinished = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawDisableOnFinished(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("DisableOnComplete : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.disableOnComplete;

            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.disableOnComplete = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawEnableOnStart(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("EnableOnStart : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.enableOnStart;

            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.enableOnStart = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawLoopType(FadeUI t)
        {
            var rootBox = VUITemplate.GetTemplate("PingPong : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.pingPong;

            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.pingPong = objField.value;
                }
            });

            return rootBox;
        }
        private VisualElement DrawEase(FadeUI t)
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
    }
}