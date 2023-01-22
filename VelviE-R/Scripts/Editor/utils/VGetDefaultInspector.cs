using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using Object = UnityEngine.Object;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
//using UnityEditor.UIElements;
namespace VIEditor
{
    public class VGetDefaultInspector
    {
        private bool ShouldRethrowException(Exception exception)
        {
            while (exception is TargetInvocationException && exception.InnerException != null)
                exception = exception.InnerException;

            return exception is ExitGUIException;
        }
        public bool DoDrawCustomIMGUIInspector(Object target)
        {
            var editor = GetActiveEditor(target);
            if (editor == null)
                editor = Editor.CreateEditor(target);

            try
            {
                if(editor == null || target == null)
                    return false;
            }
            catch(Exception)
            {
                
            }

            EditorGUIUtility.wideMode = true;
            GUIStyle editorWrapper = (editor.UseDefaultMargins() ? EditorStyles.inspectorDefaultMargins : GUIStyle.none);
            EditorGUILayout.BeginVertical(editorWrapper);
            {
                GUI.changed = false;

                try
                {
                    editor.OnInspectorGUI();
                }
                catch (Exception)
                {
                    /*
                    if (ShouldRethrowException(e))
                    {
                        throw;
                    }

                    Debug.LogException(e);
                    */
                }
            }
            EditorGUILayout.EndVertical();
            return true;
        }
        private Editor GetActiveEditor(Object target)
        {
            var activeEditors = ActiveEditorTracker.sharedTracker.activeEditors;
            if (activeEditors == null || activeEditors.Count() == 0)
                return null;

            var editor = activeEditors.FirstOrDefault((e) => e.target == target);
            return editor;
        }
        //Set properties of popup window
        public UnityEngine.UIElements.VisualElement SetPopUpContainerWindow()
        {
            var inspectorWindow = new UnityEngine.UIElements.VisualElement();
            inspectorWindow.style.backgroundColor = Color.grey;
            inspectorWindow.style.flexDirection = FlexDirection.Column;
            var lbl = new Label{text = "Property Tab"};
            lbl.style.backgroundColor = Color.magenta;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            lbl.style.height = new StyleLength(new Length(10, LengthUnit.Percent));
            lbl.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            inspectorWindow.Add(lbl);
            //inspectorWindow.style.width = 390;
            inspectorWindow.style.position = Position.Relative;
            inspectorWindow.style.alignSelf = Align.FlexStart; //This basically just saying: Attach to Left - dynmamic size
            inspectorWindow.style.flexGrow = new StyleFloat(1);
            inspectorWindow.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            inspectorWindow.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            //inspectorWindow.text = "Property Tab";
            return inspectorWindow;
        }
    }
}