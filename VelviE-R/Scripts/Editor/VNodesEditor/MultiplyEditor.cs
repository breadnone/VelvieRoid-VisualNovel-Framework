using System.Collections.Generic;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(Multiply))]
    public class MultiplyEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            dummySlot = new VisualElement();
            var t = target as Multiply;
            t.mathBasics = MathBasic.Multiply;
            root.Add(DrawToggle(t));
            root.Add(DrawVars(t));            
            root.Add(dummySlot);

            if (t.isLocal)
            {
                DrawLocalVars(t);
            }
            else
            {
                DrawAnyType(t);
            }

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        
        private VisualElement DrawToggle(Multiply t)
        {
            var rootBox = VUITemplate.GetTemplate("Compare type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.choices = new List<string>{"Value", "Local"};

            if(t.isLocal)
            objField.value = "Local";
            else
            objField.value = "Value";

            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    RemoveChild();

                    if(x.newValue == "Value")
                    {
                        t.isLocal = false;
                        DrawAnyType(t);
                    }
                    else
                    {
                        t.isLocal = true;
                        t.localVariable = null;
                        DrawLocalVars(t);
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawVars(Multiply t)
        {
            var varTemplate = VUITemplate.VariableTemplate();

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
                        t.variable = null;
                        RemoveChild();
                        PortsUtils.SetActiveAssetDirty();
                    }
                    else
                    {
                        t.variable = PortsUtils.variable.ivar.Find(x => x.Name == evt.newValue);
                        DrawAnyType(t);
                        PortsUtils.SetActiveAssetDirty();
                    }
                }
            });

            return varTemplate.root;
        }
        private void DrawLocalVars(Multiply t)
        {
            var varTemplate = VUITemplate.VariableTemplate("Local : ");

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
                        DrawAnyType(t);
                        PortsUtils.SetActiveAssetDirty();
                    }
                }
            });

            dummySlot.Add(varTemplate.root);
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
        private void DrawAnyType(Multiply t)
        {
            if (t.variable != null)
                RemoveChild();

            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = "Value : ";

            var vis = new VisualElement();

            if (t.variable is VBoolean && t.anyType.type == VTypes.Boolean)
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

            else if (t.variable is VVector2)
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
            else if (t.variable is VVector3)
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
            else if (t.variable is VVector4)
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
            else if (t.variable is VInteger)
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

            else if (t.variable is VDouble)
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
            else if (t.variable is VFloat)
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
            else if (t.variable is VString)
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
            dummySlot.Add(subroot);
        }
    }
}