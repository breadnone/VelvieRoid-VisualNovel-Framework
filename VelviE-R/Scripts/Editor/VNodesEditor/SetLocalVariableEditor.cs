using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace VIEditor
{
    [CustomEditor(typeof(SetLocalVariable))]
    public class SetLocalVariableEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            dummySlot = new VisualElement();
            var t = target as SetLocalVariable;

            root.Add(DrawVars(t));
            root.Add(dummySlot);

            if (t.Variable != null)
            {
                DrawVals(t);
            }
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawVars(SetLocalVariable t)
        {
            var varTemplate = VUITemplate.VariableTemplate();

            if (t.Variable == null)
            {
                varTemplate.child.value = "<None>";
            }
            else
            {
                if (PortsUtils.variable.ivar.Exists(x => x.Name == t.Variable.Name))
                {
                    varTemplate.child.value = t.Variable.Name;
                }
                else
                {
                    varTemplate.child.value = "<None>";
                }
            }

            if (PortsUtils.variable.ivar.Count > 0)
            {
                var varlist = new List<string>();
                PortsUtils.variable.ivar.ForEach((x) => { varlist.Add(x.Name); });

                varlist.Add("<None>");
                varTemplate.child.choices = varlist;
            }

            varTemplate.child.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                if (!PortsUtils.PlayMode && PortsUtils.variable.ivar.Count > 0)
                {
                    if (evt.newValue == "<None>")
                    {
                        t.Variable = null;
                        RemoveChild();
                        PortsUtils.SetActiveAssetDirty();
                    }
                    else
                    {
                        t.Variable = PortsUtils.variable.ivar.Find(x => x.Name == evt.newValue);
                        DrawVals(t);
                        PortsUtils.SetActiveAssetDirty();
                    }
                }
            });

            return varTemplate.root;
        }
        private void RemoveChild()
        {
            if (dummySlot.childCount > 0)
            {
                foreach (var child in dummySlot.Children().ToList())
                {
                    child.RemoveFromHierarchy();
                }
            }
        }
        private void DrawVals(SetLocalVariable t)
        {
            if (t.Variable != null)
                RemoveChild();

            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = "Value : ";

            var vis = new VisualElement();

            if (t.Variable is VBoolean && t.val.type == VTypes.Boolean)
            {
                t.val.type = VTypes.Boolean;
                var vas = new Toggle();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.boolVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.boolVal = vas.value;
                });
            }

            else if (t.Variable is VVector2)
            {
                t.val.type = VTypes.Vector2;
                var vas = new Vector2Field();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.vec2Val;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.vec2Val = vas.value;
                });
            }
            else if (t.Variable is VVector3)
            {
                t.val.type = VTypes.Vector3;
                var vas = new Vector3Field();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.vec3Val;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.vec3Val = vas.value;
                });
            }
            else if (t.Variable is VVector4)
            {
                t.val.type = VTypes.Vector4;
                var vas = new Vector4Field();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.vec4Val;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.vec4Val = vas.value;
                });
            }
            else if (t.Variable is VInteger)
            {
                t.val.type = VTypes.Integer;
                var vas = new IntegerField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.intVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.intVal = x.newValue;
                });
            }

            else if (t.Variable is VDouble)
            {
                t.val.type = VTypes.Double;
                var vas = new DoubleField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.doubleVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.doubleVal = x.newValue;
                });
            }
            else if (t.Variable is VFloat)
            {
                t.val.type = VTypes.Float;
                var vas = new FloatField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.floatVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.floatVal = x.newValue;
                });
            }
            else if (t.Variable is VString)
            {
                t.val.type = VTypes.String;
                var vas = new TextField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.val != null)
                {
                    vas.value = t.val.strVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.val.strVal = x.newValue;
                });
            }
            subroot.Add(txtLabel);
            subroot.Add(vis);
            dummySlot.Add(subroot);
        }
    }
}