using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using VIEditor;
using System;

[CustomEditor(typeof(JumpLabel))]
public class JumpLabelEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement myInspector = new VisualElement();
        myInspector.style.flexDirection = FlexDirection.Column;
        var t = target as JumpLabel;
        Undo.RecordObject(t, "JumpLabel undo object");

        //Jump String
        var boxSStrJump = new Box();
        boxSStrJump.style.marginTop = 5;
        boxSStrJump.style.marginBottom = 5;
        boxSStrJump.style.flexDirection = FlexDirection.Row;

        Label strStrJump = new Label();
        strStrJump.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
        strStrJump.style.marginLeft = 5;
        strStrJump.text = "Label : ";
        boxSStrJump.Add(strStrJump);

        var visCon = new VisualElement();
        visCon.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

        var auStrJump = new TextField();
        visCon.Add(auStrJump);

        auStrJump.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        boxSStrJump.Add(visCon);

        if (!String.IsNullOrEmpty(t.Label))
            auStrJump.value = t.Label;

            if(!PortsUtils.PlayMode)
            {
        auStrJump.RegisterCallback<FocusOutEvent>((x)=>
        {
                if (auStrJump.value != null)
                    t.Label = auStrJump.value;
                else
                    t.Label = null;
                
                WarningCheck(t);
        });
            }
        myInspector.Add(boxSStrJump);

        //Always add this at the end!
        VUITemplate.DrawSummary(myInspector, t, ()=> t.OnVSummary());
        return myInspector;
    }
    private void WarningCheck(JumpLabel t)
    {
        int currentIndex = PortsUtils.VGraph.listV.selectedIndex;
        PortsUtils.VGraph.RefreshListV();
        PortsUtils.VGraph.listV.SetSelection(currentIndex);
        PortsUtils.VGraph?.ShowSelectedVblockSerializedFields();        
    }
}
