using UnityEngine;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;
using System.Linq;

// NOTE to future maintainer, have fun refactoring this -____-
//I'm the creator, will NEVER revisit this forbidden hell ever again! 

namespace VIEditor
{
    [CustomEditor(typeof(If))]
    public class IfEditor : Editor
    {
        private VisualElement firstSlot;
        private VisualElement secondSlot;
        public override VisualElement CreateInspectorGUI()
        {
            firstSlot = new VisualElement();
            secondSlot = new VisualElement();
            VisualElement root = new VisualElement();
            var t = target as If;
            Undo.RecordObject(t, "If undo object");

            root.Add(DrawOperator(t));
            root.Add(DrawVars(t));
            root.Add(firstSlot);
            root.Add(secondSlot);

            DrawVariableDrawer(t);

            if (t.LocalorValue == "LocalVariable")
            {
                tmenu.text = "LocalVariable";
                DrawLocalComparer(t);
                t.LocalorValue = "LocalVariable";
            }
            else
            {
                tmenu.text = "Value";
                DrawValue(t);
                t.LocalorValue = "Value";
            }
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private VisualElement DrawVars(If t)
        {
            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            txtLabel.text = "Variable : ";

            ToolbarMenu vis = new ToolbarMenu();
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            if (t.Variable == null)
            {
                vis.text = "<None>";
            }
            else
            {
                vis.text = t.Variable.Name;
            }

            if (PortsUtils.variable.ivar.Count > 0)
            {
                vis.menu.AppendAction("<None>", (x) =>
                {
                    t.ECondition = EnumCondition.None;
                    vis.text = "<None>";
                    t.Variable = null;
                    t.LocalVariable = null;
                    DrawValue(t);

                    if (!String.IsNullOrEmpty(t.LocalorValue))
                    {
                        if (t.LocalorValue == "Value")
                            DrawValue(t);
                        else
                            DrawLocalComparer(t);
                    }

                    PortsUtils.SetActiveAssetDirty();
                });

                foreach (var vars in PortsUtils.variable.ivar)
                {
                    vis.menu.AppendAction(vars.Name, (x) =>
                    {
                        t.ECondition = EnumCondition.None;
                        vis.text = vars.Name;
                        t.Variable = vars;
                        t.LocalVariable = null;
                        DrawVariableDrawer(t);

                        if (!String.IsNullOrEmpty(t.LocalorValue))
                        {
                            if (t.LocalorValue == "Value")
                                DrawValue(t);
                            else
                                DrawLocalComparer(t);
                        }

                        PortsUtils.SetActiveAssetDirty();
                    });
                }
            }

            subroot.Add(txtLabel);
            subroot.Add(vis);
            return subroot;
        }
        private void DrawVariableDrawer(If t, bool isDummy = false)
        {
            if (firstSlot != null && firstSlot.childCount > 0)
            {
                foreach (var items in firstSlot.Children().ToList())
                {
                    if (items != null)
                        items.RemoveFromHierarchy();
                }
            }

            firstSlot.style.marginBottom = 5;
            firstSlot.style.marginTop = 5;
            firstSlot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            txtLabel.text = "Condition : ";
            firstSlot.Add(txtLabel);

            if (!isDummy)
            {
                if (t.Variable != null && (t.Variable.GetVtype() == VTypes.String))
                {
                    firstSlot.Add(StringActions(t));
                }
                else if (t.Variable != null && (t.Variable.GetVtype() == VTypes.Float || t.Variable.GetVtype() == VTypes.Double || t.Variable.GetVtype() == VTypes.Integer))
                {
                    firstSlot.Add(ValueActions(t));
                }
                else if (t.Variable != null && t.Variable.GetVtype() == VTypes.Boolean)
                {
                    firstSlot.Add(BooleanActions(t));
                }
                else if (t.Variable != null && (t.Variable.GetVtype() == VTypes.Vector2 || t.Variable.GetVtype() == VTypes.Vector3 || t.Variable.GetVtype() == VTypes.Vector4))
                {
                    firstSlot.Add(VectorActions(t));
                }
                else if (t.Variable != null && t.Variable.GetVtype() == VTypes.GameObject)
                {
                    firstSlot.Add(GameObjectAction(t));
                }
                else if (t.Variable != null && t.Variable.GetVtype() == VTypes.Transform)
                {
                    firstSlot.Add(TransformActions(t));
                }
                else if (t.Variable != null && t.Variable.GetVtype() == VTypes.VList)
                {
                    //Contains, checks for value & name in the list. Returns the first
                    //Exists, checks if the variable existed in the list. Returns the first
                    firstSlot.Add(VListActions(t));
                }
                else
                {
                    var tt = NoneAction(t);
                    tt.name = "<None>";
                    firstSlot.Add(tt);
                }
            }
            else
            {
                ToolbarMenu tb = new ToolbarMenu();
                tb.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                tb.SetEnabled(false);
                firstSlot.Add(tb);
            }
        }

        private ToolbarMenu BooleanActions(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            dropd.text = t.ECondition.ToString();

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction("True", (x) =>
                {
                    t.ECondition = EnumCondition.True;
                    dropd.text = t.ECondition.ToString();
                });
                dropd.menu.AppendAction("False", (x) =>
                {
                    t.ECondition = EnumCondition.False;
                    dropd.text = t.ECondition.ToString();
                });
            }

            return dropd;
        }
        private ToolbarMenu VListActions(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            dropd.text = t.ECondition.ToString();

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction("VListContains", (x) =>
                {
                    t.ECondition = EnumCondition.VListContains;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });

                dropd.menu.AppendAction("VListExist", (x) =>
                {
                    t.ECondition = EnumCondition.VListContains;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            return dropd;
        }

        private ToolbarMenu ValueActions(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            if (t.ECondition == EnumCondition.BiggerThan)
                dropd.text = ">";
            else if (t.ECondition == EnumCondition.BiggerThanEqual)
                dropd.text = ">=";
            else if (t.ECondition == EnumCondition.SmallerThan)
                dropd.text = "<";
            else if (t.ECondition == EnumCondition.SmallerThanEqual)
                dropd.text = "<=";
            else
                dropd.text = t.ECondition.ToString();

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction(">", (x) =>
                {
                    dropd.text = ">";
                    t.ECondition = EnumCondition.BiggerThan;
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction(">=", (x) =>
                {
                    dropd.text = ">=";
                    t.ECondition = EnumCondition.BiggerThanEqual;
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("<", (x) =>
                {
                    dropd.text = "<";
                    t.ECondition = EnumCondition.SmallerThan;
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("<=", (x) =>
                {
                    t.ECondition = EnumCondition.SmallerThanEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("Equal", (x) =>
                {
                    t.ECondition = EnumCondition.Equal;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("NotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.NotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            return dropd;
        }
        private ToolbarMenu StringActions(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            dropd.text = t.ECondition.ToString();

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction("Equal", (x) =>
                {
                    t.ECondition = EnumCondition.Equal;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("NotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.NotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("EqualCaseInsensitive", (x) =>
                {
                    t.ECondition = EnumCondition.EqualCaseInsensitive;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("StartsWith", (x) =>
                {
                    t.ECondition = EnumCondition.StartsWith;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("EndsWith", (x) =>
                {
                    t.ECondition = EnumCondition.EndsWith;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("Length", (x) =>
                {
                    t.ECondition = EnumCondition.Length;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            return dropd;
        }
        private ToolbarMenu VectorActions(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            dropd.text = t.ECondition.ToString();

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction("DistanceEqual", (x) =>
                {
                    t.ECondition = EnumCondition.DistanceEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("DistanceNotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.DistanceNotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            return dropd;
        }
        private ToolbarMenu GameObjectAction(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction("Equal", (x) =>
                {
                    t.ECondition = EnumCondition.Equal;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("NotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.NotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("NameEqual", (x) =>
                {
                    t.ECondition = EnumCondition.NameEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("NameNotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.NameNotEqual;
                    dropd.text = "NameNotEqual";
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("InstanceIdEqual", (x) =>
                {
                    t.ECondition = EnumCondition.InstanceIdEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("InstanceIdNotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.InstanceIdNotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("PositionEqual", (x) =>
                {
                    t.ECondition = EnumCondition.PositionEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("PositionNotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.PositionNotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            return dropd;
        }

        private ToolbarMenu TransformActions(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            dropd.text = t.ECondition.ToString();

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction("LocalPositionNotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.LocalPositionNotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("LocalPositionEqual", (x) =>
                {
                    t.ECondition = EnumCondition.LocalPositionEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("ScaleEqual", (x) =>
                {
                    t.ECondition = EnumCondition.ScaleEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
                dropd.menu.AppendAction("ScaleNotEqual", (x) =>
                {
                    t.ECondition = EnumCondition.ScaleNotEqual;
                    dropd.text = t.ECondition.ToString();
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            return dropd;
        }
        private ToolbarMenu NoneAction(If t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            dropd.text = t.ECondition.ToString();

            if (!PortsUtils.PlayMode)
            {
                dropd.menu.AppendAction("<None>", (x) =>
                {
                    dropd.text = "<None>";
                    PortsUtils.SetActiveAssetDirty();
                });
            }

            return dropd;
        }
        private ToolbarMenu tmenu;
        private VisualElement DrawOperator(If t)
        {
            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            txtLabel.text = "Comparer type : ";

            ToolbarMenu vis = new ToolbarMenu();
            tmenu = vis;
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            if (String.IsNullOrEmpty(t.LocalorValue))
            {
                vis.text = "<None>";
            }
            else
            {
                vis.text = t.LocalorValue;
            }

            if (!PortsUtils.PlayMode)
            {
                vis.menu.AppendAction("LocalVariable", (x) =>
                {
                    vis.text = "LocalVariable";
                    t.LocalVariable = null;
                    t.ECondition = EnumCondition.None;
                    t.LocalorValue = "LocalVariable";
                    DrawLocalComparer(t);

                    PortsUtils.SetActiveAssetDirty();
                });
                vis.menu.AppendAction("Value", (x) =>
                {
                    vis.text = "Value";
                    t.LocalVariable = null;
                    t.ECondition = EnumCondition.None;
                    t.LocalorValue = "Value";
                    DrawValue(t);

                    PortsUtils.SetActiveAssetDirty();
                });
            }

            subroot.Add(txtLabel);
            subroot.Add(vis);
            return subroot;
        }

        private void DrawLocalComparer(If t)
        {
            if (secondSlot != null && secondSlot.childCount > 0)
            {
                foreach (var items in secondSlot.Children().ToList())
                {
                    if (items != null)
                        items.RemoveFromHierarchy();
                }
            }

            secondSlot.style.marginBottom = 5;
            secondSlot.style.marginTop = 5;
            secondSlot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            txtLabel.text = "Variable comparer : ";

            ToolbarMenu vis = new ToolbarMenu();
            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            if (t.LocalVariable == null)
            {
                vis.text = "<None>";
            }
            else
            {
                if (t.Variable.GetVtype() == t.LocalVariable.GetVtype())
                {
                    vis.text = t.LocalVariable.Name;
                }
            }

            if (PortsUtils.variable.ivar.Count > 0)
            {
                if (!PortsUtils.PlayMode)
                {
                    vis.menu.AppendAction("<None>", (x) =>
                    {
                        vis.text = "<None>";
                        t.LocalVariable = null;
                        PortsUtils.SetActiveAssetDirty();
                    });

                    foreach (var vars in PortsUtils.variable.ivar)
                    {
                        if (vars != null && t.Variable != null && vars.GetVtype() == t.Variable.GetVtype())
                        {
                            vis.menu.AppendAction(vars.Name, (x) =>
                            {
                                vis.text = vars.Name;
                                t.LocalVariable = vars;
                                PortsUtils.SetActiveAssetDirty();
                            });
                        }
                    }
                }
            }

            secondSlot.Add(txtLabel);
            secondSlot.Add(vis);
        }
        private void DrawValue(If t)
        {
            if (secondSlot != null && secondSlot.childCount > 0)
            {
                foreach (var items in secondSlot.Children().ToList())
                {
                    if (items != null)
                        items.RemoveFromHierarchy();
                }
            }

            secondSlot.style.marginBottom = 5;
            secondSlot.style.marginTop = 5;
            secondSlot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            txtLabel.text = "Value comparer : ";

            VisualElement vis = new VisualElement();

            if (t.Variable != null && (t.Variable.GetVtype() == VTypes.String))
            {
                if (t.Variable.GetVtype() == VTypes.String)
                {
                    vis = new TextField();
                    var tmp = vis as TextField;

                    t.LocalVariable = new VString();
                    t.LocalVariable.Name = "localString";
                    t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                    (t.LocalVariable as VString).value = tmp.value;
                }

            }
            else if (t.Variable != null && (t.Variable.GetVtype() == VTypes.Float || t.Variable != null && t.Variable.GetVtype() == VTypes.Double || t.Variable != null && t.Variable.GetVtype() == VTypes.Integer))
            {
                if (t.Variable.GetVtype() == VTypes.Float)
                {
                    vis = new FloatField();
                    var tmp = vis as FloatField;

                    if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Float)
                    {
                        tmp.value = (t.LocalVariable as VFloat).value;
                    }
                    else
                    {
                        t.LocalVariable = new VFloat();
                        t.LocalVariable.Name = "localFloat";
                        t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                        (t.LocalVariable as VFloat).value = tmp.value;
                    }

                    if (!PortsUtils.PlayMode)
                    {
                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (t.LocalVariable != null)
                                (t.LocalVariable as VFloat).value = tmp.value;
                        });
                    }
                }
                else if (t.Variable.GetVtype() == VTypes.Double)
                {
                    vis = new DoubleField();
                    var tmp = vis as DoubleField;

                    if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Double)
                    {
                        tmp.value = (t.LocalVariable as VDouble).value;
                    }
                    else
                    {
                        t.LocalVariable = new VDouble();
                        t.LocalVariable.Name = "localDouble";
                        t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                        (t.LocalVariable as VDouble).value = tmp.value;
                    }
                    if (!PortsUtils.PlayMode)
                    {
                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (t.LocalVariable != null)
                                (t.LocalVariable as VDouble).value = tmp.value;

                        });
                    }
                }
                else if (t.Variable.GetVtype() == VTypes.Integer)
                {
                    vis = new IntegerField();
                    var tmp = vis as IntegerField;

                    if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Integer)
                    {
                        tmp.value = (t.LocalVariable as VInteger).value;
                    }
                    else
                    {
                        t.LocalVariable = new VInteger();
                        t.LocalVariable.Name = "localInteger";
                        t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                        (t.LocalVariable as VInteger).value = tmp.value;
                    }
                    if (!PortsUtils.PlayMode)
                    {
                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (t.LocalVariable != null)
                                (t.LocalVariable as VInteger).value = tmp.value;

                        });
                    }
                }
            }
            else if (t.Variable != null && t.Variable.GetVtype() == VTypes.Boolean)
            {
                vis = new Toggle();
                var tmp = vis as Toggle;

                if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Boolean)
                {
                    tmp.value = (t.LocalVariable as VBoolean).value;
                }
                else
                {
                    t.LocalVariable = new VBoolean();
                    t.LocalVariable.Name = "localBoolean";
                    t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                    (t.LocalVariable as VBoolean).value = tmp.value;
                }
                if (!PortsUtils.PlayMode)
                {
                    tmp.RegisterValueChangedCallback((x) =>
                    {
                        if (t.LocalVariable != null)
                            (t.LocalVariable as VBoolean).value = tmp.value;
                    });
                }
            }
            else if (t.Variable != null && (t.Variable.GetVtype() == VTypes.Vector2 || t.Variable != null && t.Variable.GetVtype() == VTypes.Vector3 || t.Variable != null && t.Variable.GetVtype() == VTypes.Vector4))
            {
                if (t.Variable.GetVtype() == VTypes.Vector2)
                {
                    vis = new Vector2Field();
                    var tmp = vis as Vector2Field;

                    if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Vector2)
                    {
                        tmp.value = (t.LocalVariable as VVector2).value;
                    }
                    else
                    {
                        t.LocalVariable = new VVector2();
                        t.LocalVariable.Name = "localVector2";
                        t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                        (t.LocalVariable as VVector2).value = tmp.value;
                    }
                    if (!PortsUtils.PlayMode)
                    {
                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (t.LocalVariable != null)
                                (t.LocalVariable as VVector2).value = tmp.value;

                        });
                    }
                }
                else if (t.Variable.GetVtype() == VTypes.Vector3)
                {
                    vis = new Vector3Field();
                    var tmp = vis as Vector3Field;

                    if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Vector3)
                    {
                        tmp.value = (t.LocalVariable as VVector3).value;
                    }
                    else
                    {
                        t.LocalVariable = new VVector3();
                        t.LocalVariable.Name = "localVector3";
                        t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                        (t.LocalVariable as VVector3).value = tmp.value;
                    }
                    if (!PortsUtils.PlayMode)
                    {
                        tmp.RegisterValueChangedCallback((x) =>
                        {

                            if (t.LocalVariable != null)
                                (t.LocalVariable as VVector3).value = tmp.value;

                        });
                    }
                }
                else if (t.Variable.GetVtype() == VTypes.Vector4)
                {
                    vis = new Vector4Field();
                    var tmp = vis as Vector4Field;

                    if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Vector4)
                    {
                        tmp.value = (t.LocalVariable as VVector4).value;
                    }
                    else
                    {
                        t.LocalVariable = new VVector4();
                        t.LocalVariable.Name = "localVector4";
                        t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                        (t.LocalVariable as VVector4).value = tmp.value;
                    }
                    if (!PortsUtils.PlayMode)
                    {
                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (t.LocalVariable != null)
                                (t.LocalVariable as VVector4).value = tmp.value;
                        });
                    }
                }
            }
            else if (t.Variable != null && t.Variable.GetVtype() == VTypes.Transform)
            {
                vis = new ObjectField();
                var tmp = vis as ObjectField;
                tmp.objectType = typeof(Transform);

                if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Transform)
                {
                    tmp.value = (t.LocalVariable as VTransform).value;
                }
                else
                {
                    t.LocalVariable = new VTransform();
                    t.LocalVariable.Name = "localTransform";
                    t.LocalVariable.VarId = PortsUtils.variable.GlobalCounter++;
                    (t.LocalVariable as VTransform).value = tmp.value as Transform;
                }
                if (!PortsUtils.PlayMode)
                {
                    tmp.RegisterValueChangedCallback((x) =>
                    {
                        if (t.LocalVariable != null)
                            (t.LocalVariable as VTransform).value = tmp.value as Transform;
                    });
                }
            }
            else if (t.Variable != null && t.Variable.GetVtype() == VTypes.VList)
            {
                vis = new TextField();
            }

            vis.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            secondSlot.Add(txtLabel);
            secondSlot.Add(vis);
        }
    }
}