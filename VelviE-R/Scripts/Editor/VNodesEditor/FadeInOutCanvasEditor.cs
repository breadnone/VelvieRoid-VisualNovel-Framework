using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(FadeInOutCanvas))]
    public class FadeInOutCanvasEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as FadeInOutCanvas;

            root.Add(DrawCanvas(t));
            root.Add(DrawFadeBool(t));
            root.Add(DrawDuration(t));
            root.Add(DrawEnableOnStartBool(t));
            root.Add(DrawDisableOnFinishedBool(t));
            root.Add(DrawWaitBool(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        public VisualElement DrawCanvas(FadeInOutCanvas t)
        {
            var rootBox = VUITemplate.GetTemplate("Canvas group : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(CanvasGroup);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.canvas;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.canvas = objField.value as CanvasGroup;
                });
            }

            return rootBox;
        }

        private VisualElement DrawFadeBool(FadeInOutCanvas t)
        {
            var rootBox = VUITemplate.GetTemplate("Fade type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.choices = new List<string> { "Fade In", "Fade Out" };

            if (t.fadeIn)
                objField.value = "Fade In";
            else
                objField.value = "Fade Out";

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterCallback<ChangeEvent<string>>((x) =>
                {
                    if (x.newValue == "Fade In")
                    {
                        t.fadeIn = true;
                    }
                    else
                    {
                        t.fadeIn = false;
                    }
                });
            }

            return rootBox;
        }

        private VisualElement DrawWaitBool(FadeInOutCanvas t)
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
                    t.fadeIn = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawDisableOnFinishedBool(FadeInOutCanvas t)
        {
            var rootBox = VUITemplate.GetTemplate("DisableOnComplete : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            objField.value = t.disableOnComplete;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.disableOnComplete = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawEnableOnStartBool(FadeInOutCanvas t)
        {
            var rootBox = VUITemplate.GetTemplate("EnableOnStart : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            objField.value = t.enableOnStart;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.enableOnStart = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawDuration(FadeInOutCanvas t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            objField.value = t.duration;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.duration = x.newValue;
                });
            }

            return rootBox;
        }
    }
}