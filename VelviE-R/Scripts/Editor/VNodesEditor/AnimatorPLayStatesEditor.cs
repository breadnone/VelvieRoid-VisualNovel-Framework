using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using System.Collections.Generic;

namespace VIEditor
{
    [CustomEditor(typeof(AnimatorPlayStates))]
    public class AnimatorPLayStatesEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as AnimatorPlayStates;

            root.Add(DrawStop(t));
            root.Add(DrawAnimatorObj(t));
            root.Add(DrawLayer(t));
            root.Add(DrawState(t));
            root.Add(DrawWeight(t));
            root.Add(DrawInterpolate(t));
            root.Add(DrawDuration(t));
            root.Add(DrawResetSpeed(t));
            root.Add(DrawFixedTime(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawAnimatorObj(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("Animator : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Animator);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.animator;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.animator = objField.value as Animator;
                });
            }
            return rootBox;
        }
        private VisualElement DrawLayer(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("Layer : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.layerName;
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.layerName = objField.value;

                });
            }
            return rootBox;
        }
        private VisualElement DrawState(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("State : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.stateName;
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.stateName = objField.value;

                });
            }
            return rootBox;
        }
        private VisualElement DrawFixedTime(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("PlayInFixedTime : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.playInFixedTime;
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.playInFixedTime = objField.value;
                });
            }
            return rootBox;
        }
        private VisualElement DrawResetSpeed(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("Reset speed : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.resetDefaultSpeed;
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.resetDefaultSpeed = objField.value;
                });
            }
            return rootBox;
        }
        private VisualElement DrawStop(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("Stop : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.choices = new List<string> { "Stop", "Play" };
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.stop)
                objField.value = "Stop";
            else
                objField.value = "Play";
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (x.newValue == "Stop")
                        t.stop = true;
                    else
                        t.stop = false;
                });
            }
            return rootBox;
        }
        private VisualElement DrawWeight(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("Weight : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Slider();
            objField.style.width = 160;
            objField.lowValue = 0f;
            objField.highValue = 1f;
            field.Add(objField);

            objField.value = t.weight;

            var objIndic = new FloatField();
            objIndic.style.width = 40;
            objIndic.value = t.weight;

            rootBox.Add(objIndic);
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.weight = x.newValue;
                    objIndic.value = x.newValue;
                });
            }
            return rootBox;
        }
        private VisualElement DrawInterpolate(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("Interpolate : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.choices = new List<string> { "Interpolate", "Disable" };
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.interpolate)
                objField.value = "Interpolate";
            else
                objField.value = "Disable";
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (x.newValue == "Interpolate")
                        t.interpolate = true;
                    else
                        t.interpolate = false;
                });
            }
            return rootBox;
        }
        private VisualElement DrawDuration(AnimatorPlayStates t)
        {
            var rootBox = VUITemplate.GetTemplate("InterpolateSpeed : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.interpolateDuration;
            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.interpolateDuration = objField.value;
                });
            }
            return rootBox;
        }
    }
}