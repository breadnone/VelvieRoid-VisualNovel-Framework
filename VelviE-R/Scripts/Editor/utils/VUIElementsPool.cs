using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;
using System;
using Object = UnityEngine.Object;

namespace VIEditor
{
    public static class VUIElementsPool
    {
        private static List<EditorWindow> AllEditorWindows = new List<EditorWindow>();
        private static List<VisualElement> AllUIElements = new List<VisualElement>();

        public static void RefreshAll()
        {
            foreach(var t in AllEditorWindows)
            {
                if(t != null)
                    t.Repaint();
            }

            foreach(var e in AllUIElements)
            {
                if(e != null)
                {
                    var visChild = e.Children();
                    e.MarkDirtyRepaint();

                    foreach(var y in visChild)
                    {
                        if(y != null)
                        {
                            y.MarkDirtyRepaint();
                        }
                    }
                }          
            }
        }

        public static void PoolVisualElement(VisualElement vis)
        {
            AllUIElements.Add(vis);
        }
        public static void PoolEditorWindow(EditorWindow editor)
        {
            AllEditorWindows.Add(editor);
        }
    }
}