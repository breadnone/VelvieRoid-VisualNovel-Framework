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
            lblSeconds.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lblSeconds.style.marginLeft = 10;
            lblSeconds.text = "Frames : ";

            var visCon = new VisualElement();
            visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var objSeconds = new IntegerField();
            visCon.Add(objSeconds);

            objSeconds.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            objSeconds.value = t.Frames;

            if(!PortsUtils.PlayMode)
            {
                objSeconds.RegisterValueChangedCallback((x)=>
                {
                    t.Frames = objSeconds.value;
                });
            }

            box.Add(lblSeconds);
            box.Add(visCon);
            root.Add(box);
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, ()=> t.OnVSummary());
            return root;
        }
    }
}