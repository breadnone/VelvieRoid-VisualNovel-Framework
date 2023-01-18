using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;

namespace VIEditor
{
    [CustomEditor(typeof(WaitFrames))]
    public class WaitFramesEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var t = target as WaitFrames;
            
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var box = new Box();
            box.style.flexDirection = FlexDirection.Row;

            var lblSeconds = new Label();
            lblSeconds.style.width = 120;
            lblSeconds.text = "Frames : ";

            var objSeconds = new IntegerField();
            objSeconds.style.width = 220;
            objSeconds.value = t.Frames;

            objSeconds.RegisterValueChangedCallback((x)=>
            {
                t.Frames = objSeconds.value;
            });

            box.Add(lblSeconds);
            box.Add(objSeconds);
            root.Add(box);
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, ()=> t.OnVSummary());
            return root;
        }
    }
}