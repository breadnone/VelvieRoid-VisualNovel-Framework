using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(SetPrefVariable))]
    public class SetPrefVariableEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetPrefVariable;
            dummySlot = new VisualElement();
            
            root.Add(DrawKey(t));
            root.Add(DrawVars(t));
            root.Add(dummySlot);
            dummySlot.Add(DrawVals(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root; 
        }
        private VisualElement DrawVars(SetPrefVariable t)
        {
            if(t.localVariable == null)
                RemoveChild();

            var varTemplate = VUITemplate.VariableTemplate();

            if (t.localVariable == null)
            {
                varTemplate.child.value = "<None>";
            }
            else
            {
                if (PortsUtils.variable.ivar.Exists(x => x.Name == t.localVariable.Name))
                {
                    varTemplate.child.value = t.localVariable.Name;
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
                    RemoveChild();

                    if (evt.newValue == "<None>")
                    {
                        t.localVariable = null;
                        PortsUtils.SetActiveAssetDirty();
                    }
                    else
                    {
                        t.localVariable = PortsUtils.variable.ivar.Find(x => x.Name == evt.newValue);
                        dummySlot.Add(DrawVals(t));
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
        private VisualElement DrawVals(SetPrefVariable t)
        {
            if (t.localVariable != null)
                RemoveChild();

            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = "Value : ";

            var vis = new VisualElement();

            if (t.localVariable is VBoolean && t.anyType.type == VTypes.Boolean)
            {
                t.anyType.type = VTypes.Boolean;
                var vas = new Toggle();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.boolVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.boolVal = vas.value;
                });
            }

            else if (t.localVariable is VVector2)
            {
                t.anyType.type = VTypes.Vector2;
                var vas = new Vector2Field();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.vec2Val;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.vec2Val = vas.value;
                });
            }
            else if (t.localVariable is VVector3)
            {
                t.anyType.type = VTypes.Vector3;
                var vas = new Vector3Field();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.vec3Val;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.vec3Val = vas.value;
                });
            }
            else if (t.localVariable is VVector4)
            {
                t.anyType.type = VTypes.Vector4;
                var vas = new Vector4Field();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.vec4Val;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.vec4Val = vas.value;
                });
            }
            else if (t.localVariable is VInteger)
            {
                t.anyType.type = VTypes.Integer;
                var vas = new IntegerField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.intVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.intVal = x.newValue;
                });
            }

            else if (t.localVariable is VDouble)
            {
                t.anyType.type = VTypes.Double;
                var vas = new DoubleField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.doubleVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.doubleVal = x.newValue;
                });
            }
            else if (t.localVariable is VFloat)
            {
                t.anyType.type = VTypes.Float;
                var vas = new FloatField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.floatVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.floatVal = x.newValue;
                });
            }
            else if (t.localVariable is VString)
            {
                t.anyType.type = VTypes.String;
                var vas = new TextField();
                vas.style.width = 220;
                vis.Add(vas);

                if (t.anyType != null)
                {
                    vas.value = t.anyType.strVal;                    
                }

                vas.RegisterValueChangedCallback((x) =>
                {
                    t.anyType.strVal = x.newValue;
                });
            }

            subroot.Add(txtLabel);
            subroot.Add(vis);
            return subroot;
        }
        private VisualElement DrawKey(SetPrefVariable t)
        {
            var rootBox = VUITemplate.GetTemplate("Key : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.key;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.key = objField.value;
            });

            return rootBox;
        }
    }
}