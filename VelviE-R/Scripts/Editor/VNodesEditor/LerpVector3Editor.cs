using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using VelvieR;
using TMPro;

namespace VIEditor
{
    [CustomEditor(typeof(LerpVector3))]
    public class LerpVector3Editor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as LerpVector3;
            root.Add(DrawVars(t));
            root.Add(DrawTo(t));
            root.Add(DrawDuration(t));
            root.Add(DrawText(t));
            root.Add(DrawFormat(t));
            root.Add(DrawWait(t));

            return root;
        }
        private VisualElement DrawVars(LerpVector3 t)
        {
            var varTemplate = VUITemplate.VariableTemplate(type: VTypes.Vector3);

            if (t.variable == null)
            {
                varTemplate.child.value = "<None>";
            }
            else
            {
                if (PortsUtils.variable.ivar.Exists(x => x.Name == t.variable.Name))
                {
                    varTemplate.child.value = t.variable.Name;
                }
                else
                {
                    varTemplate.child.value = "<None>";
                }
            }

            var tp  = varTemplate.root;

            if (PortsUtils.variable.ivar.Count > 0)
            {
                varTemplate.child.choices.Clear();

                var varlist = new List<string>();
                PortsUtils.variable.ivar.ForEach((x) => 
                { 
                    if(x.GetVtype() == VTypes.Vector3)
                        varlist.Add(x.Name); 
                });

                varlist.Add("<None>");
                varTemplate.child.choices = varlist;
            }

            varTemplate.child.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                if (!PortsUtils.PlayMode && PortsUtils.variable.ivar.Count > 0)
                {
                    if (evt.newValue == "<None>")
                    {
                        t.variable = null;
                        PortsUtils.SetActiveAssetDirty();
                    }
                    else
                    {
                        t.variable = PortsUtils.variable.ivar.Find(x => x.Name == evt.newValue);
                        PortsUtils.SetActiveAssetDirty();
                    }
                }
            });

            return varTemplate.root;
        }
        private VisualElement DrawText(LerpVector3 t)
        {
            var rootBox = VUITemplate.GetTemplate("Output text : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(TMP_Text);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.outputText;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.outputText = objField.value as TMP_Text;
            });

            return rootBox;
        }
        private VisualElement DrawTo(LerpVector3 t)
        {
            var rootBox = VUITemplate.GetTemplate("To : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.to;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.to = x.newValue;
            });

            return rootBox;
        }
        private VisualElement DrawDuration(LerpVector3 t)
        {
            var rootBox = VUITemplate.GetTemplate("Duration : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.duration;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.duration = x.newValue;
            });

            return rootBox;
        }
        private VisualElement DrawFormat(LerpVector3 t)
        {
            var rootBox = VUITemplate.GetTemplate("Format : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.format;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.format = x.newValue;
            });

            return rootBox;
        }
        private VisualElement DrawWait(LerpVector3 t)
        {
            var rootBox = VUITemplate.GetTemplate("WaitUntilFinished : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.waitUntilFinished = x.newValue;
            });

            return rootBox;
        }
    }
}