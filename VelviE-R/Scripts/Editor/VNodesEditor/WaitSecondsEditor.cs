using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(WaitSeconds))]
    public class WaitSecondsEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var t = target as WaitSeconds;
            
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var box = new Box();
            box.style.flexDirection = FlexDirection.Row;

            var lblSeconds = new Label();
            lblSeconds.style.width = 120;
            lblSeconds.text = "Seconds : ";

            var objSeconds = new FloatField();
            objSeconds.style.width = 220;
            objSeconds.value = t.Seconds;

            objSeconds.RegisterValueChangedCallback((x)=>
            {
                t.Seconds = objSeconds.value;
            });

            box.Add(lblSeconds);
            box.Add(objSeconds);
            root.Add(box);

            var boxTwo = new Box();
            boxTwo.style.flexDirection = FlexDirection.Row;

            var lblScale = new Label();
            lblScale.style.width = 120;
            lblScale.text = "Unscaled time : ";

            var objScale = new Toggle();
            objScale.style.width = 220;
            objScale.value = t.UnscaledTime;

            objScale.RegisterValueChangedCallback((x)=>
            {
                t.UnscaledTime = objScale.value;
            });

            boxTwo.Add(lblScale);
            boxTwo.Add(objScale);
            root.Add(boxTwo);

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, ()=> t.OnVSummary());
            return root;
        }
    }
}