using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;
using VIEditor;
using System;

public class VConfirmation : PopupField<TextField>
{
    public VConfirmation()
    {
        //okButton = act;

        this.pickingMode = PickingMode.Position;
        this.style.height = 300;
        this.style.width = 300;
        this.style.position = Position.Absolute;
        //this.text = confirmText;
        
        var box = new Box();
        box.StretchToParentSize();
        box.style.flexDirection = FlexDirection.Row;
        box.style.alignItems = Align.Center;

        var btnOk = new Button(okButton);
        btnOk.style.width = 100;
        btnOk.style.height = 70;
        box.Add(btnOk);

        var btnCancel = new Button(RemoveFromRoot);
        btnCancel.style.width = 100;
        btnCancel.style.height = 70;
        box.Add(btnCancel);

        this.Add(box);
        PortsUtils.VGraph.graphView.Add(this);
    }

    public Action okButton {get;set;} = null;

    private void RemoveFromRoot()
    {
        PortsUtils.VGraph.graphView.Remove(this);
        PortsUtils.VGraph.graphView.MarkDirtyRepaint();
    }
}
