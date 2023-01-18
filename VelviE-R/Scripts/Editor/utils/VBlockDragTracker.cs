using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace VIEditor
{
    public enum VBlockSelection
    {
        Single, //Default
        Sequential, //Ctrl
        Multiple //Shift
    }
    public static class VBlockDragTracker
    {
        public static VBlockSelection selectionType = VBlockSelection.Single;
        public static List<VisualElement> activeVBlockLabel = new List<VisualElement>();
        public static VisualElement activeDraggedVBlockLabel { get; set; }
        public static VisualElement targetVBlockLabel { get; set; }
        public static void CacheVBlock(VisualElement lbl)
        {
            activeVBlockLabel.Add(lbl);
        }
        public static (string titleCon, Color col, int defHeight, int defWidth, int blockCounter) DefaultDummy()
        {
            return ("Moving", Color.grey, 30, 300, 0);
        }
    }
}