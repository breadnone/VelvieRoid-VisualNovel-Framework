using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using System;
using UnityEngine.UIElements;
using System.Linq;
using TMPro;

namespace VIEditor
{
    [CustomEditor(typeof(While))]
    public class WhileEditor : Editor
    {
        private VisualElement Root;
        private VisualElement firstSlot;
        private VisualElement secondSlot;
        public override VisualElement CreateInspectorGUI()
        {
            firstSlot = new VisualElement();
            secondSlot = new VisualElement();
            VisualElement root = new VisualElement();
            var t = target as While;
            Root = root;

            root.Add(DrawOperator(t));
            root.Add(DrawVars(t));
            root.Add(firstSlot);
            root.Add(secondSlot);

            DrawVariableDrawer(t);

            if (t.isLocal)
            {
                tmenu.text = "Local";
                DrawLocalComparer(t);
                t.isLocal = true;
            }
            else
            {
                tmenu.text = "Value";
                DrawValue(t);
                t.isLocal = false;
            }
            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawVars(While t)
        {
            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = "While : ";

            ToolbarMenu vis = new ToolbarMenu();
            vis.style.width = 220;

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
                    t.anyType.type = VTypes.None;
                    vis.text = "<None>";
                    t.Variable = null;
                    t.LocalVariable = null;
                    DrawValue(t);

                    if (!t.isLocal)
                    {
                        DrawValue(t);
                    }
                    else
                    {
                        DrawLocalComparer(t);
                    }

                    PortsUtils.SetActiveAssetDirty();
                });

                foreach (var vars in PortsUtils.variable.ivar)
                {
                    vis.menu.AppendAction(vars.Name, (x) =>
                    {
                        t.ECondition = EnumCondition.None;
                        DrawVariableDrawer(t);

                        t.anyType.type = vars.GetVtype();
                        vis.text = vars.Name;
                        t.Variable = vars;
                        t.LocalVariable = null;
                        DrawVariableDrawer(t);

                        if (!t.isLocal)
                        {
                            DrawValue(t);
                        }
                        else
                        {
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
        private void ClearFirstSlot()
        {
            if (firstSlot != null && firstSlot.childCount > 0)
            {
                foreach (var items in firstSlot.Children().ToList())
                {
                    if (items != null)
                        items.RemoveFromHierarchy();
                }
            }
        }
        private void DrawVariableDrawer(While t, bool isDummy = false)
        {
            ClearFirstSlot();
            firstSlot.style.marginBottom = 5;
            firstSlot.style.marginTop = 5;
            firstSlot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = "Condition : ";

            firstSlot.Add(txtLabel);

            if (!isDummy)
            {
                if (t.Variable != null)
                {
                    if (t.Variable.GetVtype() == VTypes.String)
                    {
                        firstSlot.Add(StringActions(t));
                    }
                    else if (t.Variable.GetVtype() == VTypes.Float || t.Variable.GetVtype() == VTypes.Double || t.Variable.GetVtype() == VTypes.Integer)
                    {
                        firstSlot.Add(ValueActions(t));
                    }
                    else if (t.Variable.GetVtype() == VTypes.Boolean)
                    {
                        firstSlot.Add(BooleanActions(t));
                    }
                    else if (t.Variable.GetVtype() == VTypes.Vector2 || t.Variable.GetVtype() == VTypes.Vector3 || t.Variable.GetVtype() == VTypes.Vector4)
                    {
                        firstSlot.Add(VectorActions(t));
                    }
                    else
                    {
                        var tt = NoneAction(t);
                        tt.name = "<None>";
                        firstSlot.Add(tt);
                    }
                }
            }
            else
            {
                ToolbarMenu tb = new ToolbarMenu();
                tb.style.width = 220;
                tb.SetEnabled(false);
                firstSlot.Add(tb);
            }
        }

        private ToolbarMenu BooleanActions(While t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = 220;
            dropd.text = t.ECondition.ToString();

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

            return dropd;
        }
        private ToolbarMenu VListActions(While t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = 220;
            dropd.text = t.ECondition.ToString();

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

            return dropd;
        }

        private ToolbarMenu ValueActions(While t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = 220;

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

            return dropd;
        }
        private ToolbarMenu StringActions(While t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = 220;
            dropd.text = t.ECondition.ToString();

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

            return dropd;
        }
        private ToolbarMenu VectorActions(While t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = 220;
            dropd.text = t.ECondition.ToString();

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

            return dropd;
        }

        private ToolbarMenu NoneAction(While t)
        {
            ToolbarMenu dropd = new ToolbarMenu();
            dropd.style.width = 220;
            dropd.text = t.ECondition.ToString();

            dropd.menu.AppendAction("<None>", (x) =>
            {
                dropd.text = "<None>";
                PortsUtils.SetActiveAssetDirty();
            });

            return dropd;
        }
        private ToolbarMenu tmenu;
        private VisualElement DrawOperator(While t)
        {
            VisualElement subroot = new VisualElement();
            subroot.style.marginBottom = 5;
            subroot.style.marginTop = 5;
            subroot.style.flexDirection = FlexDirection.Row;

            Label txtLabel = new Label();
            txtLabel.style.width = 120;
            txtLabel.text = "Comparer type : ";

            ToolbarMenu vis = new ToolbarMenu();
            tmenu = vis;
            vis.style.width = 220;

            if (!t.isLocal)
            {
                vis.text = "Local";
            }
            else
            {
                vis.text = "Value";
            }

            vis.menu.AppendAction("Local", (x) =>
            {
                vis.text = "Local";
                t.LocalVariable = null;
                t.ECondition = EnumCondition.None;
                t.isLocal = true;
                DrawLocalComparer(t);

                PortsUtils.SetActiveAssetDirty();
            });
            vis.menu.AppendAction("Value", (x) =>
            {
                vis.text = "Value";
                t.LocalVariable = null;
                t.ECondition = EnumCondition.None;
                t.isLocal = false;
                DrawValue(t);
                PortsUtils.SetActiveAssetDirty();
            });

            subroot.Add(txtLabel);
            subroot.Add(vis);
            return subroot;
        }

        private void DrawLocalComparer(While t)
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
            txtLabel.style.width = 120;
            txtLabel.text = "Variable comparer : ";

            ToolbarMenu vis = new ToolbarMenu();
            vis.style.width = 220;

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

            secondSlot.Add(txtLabel);
            secondSlot.Add(vis);
        }
        private void DrawValue(While t)
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
            txtLabel.style.width = 120;
            txtLabel.text = "Value comparer : ";

            VisualElement vis = new VisualElement();

            if (t.Variable != null)
            {
                if (t.Variable.GetVtype() == VTypes.String)
                {
                    vis = new TextField();
                    var tmp = vis as TextField;

                    if (t.anyType.type == VTypes.String)
                    {
                        tmp.value = t.anyType.strVal;
                    }

                    tmp.RegisterValueChangedCallback((x) =>
                    {
                        if (!PortsUtils.PlayMode)
                        {
                            t.anyType.strVal = tmp.value;
                        }
                    });

                }
                else if (t.Variable.GetVtype() == VTypes.Float || t.Variable.GetVtype() == VTypes.Double || t.Variable.GetVtype() == VTypes.Integer)
                {
                    if (t.Variable.GetVtype() == VTypes.Float)
                    {
                        vis = new FloatField();
                        var tmp = vis as FloatField;

                        if (t.anyType.type == VTypes.Float)
                        {
                            tmp.value = t.anyType.floatVal;
                        }

                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                t.anyType.floatVal = tmp.value;
                            }
                        });
                    }
                    else if (t.Variable.GetVtype() == VTypes.Double)
                    {
                        vis = new DoubleField();
                        var tmp = vis as DoubleField;

                        if (t.anyType.type == VTypes.Double)
                        {
                            tmp.value = t.anyType.doubleVal;
                        }

                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                t.anyType.doubleVal = tmp.value;
                            }
                        });
                    }
                    else if (t.Variable.GetVtype() == VTypes.Integer)
                    {
                        vis = new IntegerField();
                        var tmp = vis as IntegerField;

                        if (t.anyType.type == VTypes.Integer)
                        {
                            tmp.value = t.anyType.intVal;
                        }

                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                t.anyType.intVal = tmp.value;
                            }
                        });
                    }
                }
                else if (t.Variable.GetVtype() == VTypes.Boolean)
                {
                    vis = new Toggle();
                    var tmp = vis as Toggle;

                    if (t.anyType.type == VTypes.Boolean)
                    {
                        tmp.value = t.anyType.boolVal;
                    }

                    tmp.RegisterValueChangedCallback((x) =>
                    {
                        if (!PortsUtils.PlayMode)
                        {
                            t.anyType.boolVal = tmp.value;
                        }
                    });
                }
                else if (t.Variable.GetVtype() == VTypes.Vector2 || t.Variable.GetVtype() == VTypes.Vector3 || t.Variable.GetVtype() == VTypes.Vector4)
                {
                    if (t.Variable.GetVtype() == VTypes.Vector2)
                    {
                        vis = new Vector2Field();
                        var tmp = vis as Vector2Field;

                        if (t.anyType.type == VTypes.String)
                        {
                            tmp.value = t.anyType.vec2Val;
                        }

                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                t.anyType.vec2Val = tmp.value;
                            }
                        });
                    }
                    else if (t.Variable.GetVtype() == VTypes.Vector3)
                    {
                        vis = new Vector3Field();
                        var tmp = vis as Vector3Field;

                        if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Vector3)
                        {
                            tmp.value = t.anyType.vec3Val;
                        }

                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                t.anyType.vec3Val = tmp.value;
                            }
                        });
                    }
                    else if (t.Variable.GetVtype() == VTypes.Vector4)
                    {
                        vis = new Vector4Field();
                        var tmp = vis as Vector4Field;

                        if (t.LocalVariable != null && t.LocalVariable.GetVtype() == VTypes.Vector4)
                        {
                            tmp.value = t.anyType.vec4Val;
                        }

                        tmp.RegisterValueChangedCallback((x) =>
                        {
                            if (!PortsUtils.PlayMode)
                            {
                                t.anyType.vec4Val = tmp.value;
                            }
                        });
                    }
                }
            }
            vis.style.width = 220;
            secondSlot.Add(txtLabel);
            secondSlot.Add(vis);
        }
    }
}
