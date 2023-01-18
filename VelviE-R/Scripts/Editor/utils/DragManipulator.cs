using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VIEditor;
public class DragManipulator : PointerManipulator
{
    public DragManipulator(VisualElement target)
    {
        this.target = target;
        root = target.parent;
        enableClick = false;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }
    public bool enableClick {get;set;}
    public bool lockXaxis {get;set;}
    public bool lockYaxis {get;set;}
    private Vector2 targetStartPosition { get; set; }

    private Vector3 pointerStartPosition { get; set; }

    private bool enabled { get; set; }

    private VisualElement root { get; }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        if(evt.button == 1 || !enableClick)
            return;

        targetStartPosition = target.transform.position;
        pointerStartPosition = evt.position;
        target.CapturePointer(evt.pointerId);
        enabled = true;
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        if(evt.button == 1 || !enableClick)
            return;

        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            Vector3 pointerDelta = evt.position - pointerStartPosition;

            target.transform.position = new Vector2(
                Mathf.Clamp(targetStartPosition.x + pointerDelta.x, -target.parent.worldBound.width, target.parent.panel.visualTree.worldBound.width),
                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, -target.parent.panel.visualTree.worldBound.height, target.parent.panel.visualTree.worldBound.height));
        }
    }

    private void PointerUpHandler(PointerUpEvent evt)
    {
        if(evt.button == 1 || !enableClick)
            return;

        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            targetStartPosition = target.transform.position;
            pointerStartPosition = evt.position;

            var e = evt.target as VisualElement;
            var ev = e.userData as Label;

            target.ReleasePointer(evt.pointerId);
        }
    }

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        if (enabled)
        {
            enabled = false;
        }
    }
}
