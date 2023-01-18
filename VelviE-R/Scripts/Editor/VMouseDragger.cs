using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using VIEditor;
public class VMouseDragger : PointerManipulator
{
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

    private Vector2 targetStartPosition { get; set; }

    private Vector3 pointerStartPosition { get; set; }

    private bool enabled { get; set; }

    private VisualElement root { get; }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        targetStartPosition = target.transform.position;
        pointerStartPosition = evt.position;
        target.CapturePointer(evt.pointerId);
        enabled = true;
     }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            Vector3 pointerDelta = evt.position - pointerStartPosition;

            target.transform.position = new Vector2(
                Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
        }
    }

    private void PointerUpHandler(PointerUpEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
        }
    }

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        if (enabled)
        {
            UQueryBuilder<VisualElement> allvblocks =
                PortsUtils.VGraph.ParentVBlockBox.Query<VisualElement>("VBlockLabel");

            UQueryBuilder<VisualElement> overlappingSlots =
                allvblocks.Where(OverlapsTarget);

            VisualElement closestOverlappingSlot =
                FindClosestSlot(overlappingSlots);

            Vector3 closestPos = Vector3.zero;
            
            if (closestOverlappingSlot != null)
            {
                closestPos = RootSpaceOfSlot(closestOverlappingSlot);
                closestPos = new Vector2(closestPos.x - 5, closestPos.y - 5);
            }
            target.transform.position =
                closestOverlappingSlot != null ?
                closestPos :
                targetStartPosition;

            enabled = false;
        }
    }

    private bool OverlapsTarget(VisualElement vblock)
    {
        return target.worldBound.Overlaps(vblock.worldBound);
    }

    private VisualElement FindClosestSlot(UQueryBuilder<VisualElement> VBlocks)
    {
        List<VisualElement> slotsList = VBlocks.ToList();
        float bestDistanceSq = float.MaxValue;
        VisualElement closest = null;
        foreach (VisualElement slot in slotsList)
        {
            Vector3 displacement =
                RootSpaceOfSlot(slot) - target.transform.position;
            float distanceSq = displacement.sqrMagnitude;
            if (distanceSq < bestDistanceSq)
            {
                bestDistanceSq = distanceSq;
                closest = slot;
            }
        }
        return closest;
    }

    private Vector3 RootSpaceOfSlot(VisualElement VBlock)
    {
        Vector2 slotWorldSpace = VBlock.parent.LocalToWorld(VBlock.layout.position);
        return root.WorldToLocal(slotWorldSpace);
    }

}
