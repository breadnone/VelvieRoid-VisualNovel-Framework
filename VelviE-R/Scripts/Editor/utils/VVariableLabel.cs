using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VelvieR;
using System.Linq;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using UnityEditor;
namespace VIEditor
{
    public class VVariableLabel : VisualElement
    {
        public string VBlockId { get; set; }
        public TextField VBlockContent { get; set; }
        public Label VBlockLineNumber { get; set; }
        public Box FieldContainer { get; set; }
        private int DefHeight;
        private Toolbar thistoolbar;
        public int VLabelIndex { get; set; }
        public Toggle publicToggle { get; set; }
        public VVariableLabel(string titleCon, VColor col, int defHeight, int defWidth, int blockCounter, bool manipulator)
        {
            VBlockId = Guid.NewGuid().ToString();
            DefHeight = defHeight;
            SetDefault(titleCon, col, defHeight, defWidth, blockCounter);
        }
        public void SetDefault(string titleCon, VColor col, int defHeight, int defWidth, int blockCounter)
        {
            this.pickingMode = PickingMode.Position;
            this.style.marginTop = 3;
            this.style.height = defHeight;
            this.style.width = defWidth;
            this.style.flexDirection = FlexDirection.Row;
            this.name = "VariableDrawer";

            var toolbar = new Toolbar();
            toolbar.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            var no = new Label();
            no.style.flexWrap = Wrap.Wrap;
            no.style.unityTextAlign = TextAnchor.MiddleCenter;
            no.style.color = Color.white;
            no.style.backgroundColor = Color.red;
            no.style.alignSelf = Align.FlexStart;
            no.style.height = defHeight;
            no.style.width = 40;
            no.style.borderLeftWidth = 1;
            no.style.borderBottomWidth = 1;
            no.style.borderTopWidth = 1;
            no.style.borderLeftColor = Color.black;
            no.style.borderBottomColor = Color.black;
            no.style.borderTopColor = Color.black;
            VBlockLineNumber = no;
            toolbar.Add(no);

            var content = new TextField();
            content.name = "VContent";
            content.style.flexWrap = Wrap.Wrap;
            content.style.unityTextAlign = TextAnchor.MiddleCenter;
            content.style.color = Color.black;
            content.style.alignSelf = Align.FlexStart;
            content.style.height = defHeight;
            content.style.width = 300;
            content.style.borderBottomWidth = 1;
            content.style.borderTopWidth = 1;
            content.style.borderLeftWidth = 1;
            content.style.borderRightWidth = 1;
            content.style.borderBottomColor = Color.black;
            content.style.borderTopColor = Color.black;
            content.style.borderLeftColor = Color.black;
            content.style.borderRightColor = Color.black;
            content.tooltip = "Variable name can't be empty";
            VBlockContent = content;

            var indicator = new ToolbarMenu { text = "TYPE", variant = ToolbarMenu.Variant.Default };
            indicator.menu.AppendAction("Integer", a => { SetActive(); indicator.text = "Integer"; InsertUiField(IntLabel(true, 0, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("Double", a => { SetActive(); indicator.text = "Double"; InsertUiField(DoubleLabel(true, 0, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("Float", a => { SetActive(); indicator.text = "Float"; InsertUiField(FloatLabel(true, 0f, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("String", a => { SetActive(); indicator.text = "String"; InsertUiField(StringLabel(true, "", "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("Vector2", a => { SetActive(); indicator.text = "Vector2"; InsertUiField(VecTwoLabel(true, Vector2.zero, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("Vector3", a => { SetActive(); indicator.text = "Vector3"; InsertUiField(VecThreeLabel(true, Vector3.zero, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("Vector4", a => { SetActive(); indicator.text = "Vector4"; InsertUiField(VecFourLabel(true, Vector4.zero, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("VList", a => { SetActive(); indicator.text = "VList"; InsertUiField(ListLabel(true, VTypes.None, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("Boolean", a => { SetActive(); indicator.text = "Boolean"; InsertUiField(BoolLabel(true, false, "Var"), true); }, a => DropdownMenuAction.Status.Normal);
            indicator.menu.AppendAction("None", a => { SetActive(); indicator.text = "TYPE"; InsertUiField(null, false); }, a => DropdownMenuAction.Status.Normal);
            indicator.name = "variableDropDown";
            indicator.style.width = 130;
            indicator.style.height = defHeight;
            indicator.style.flexWrap = Wrap.Wrap;
            indicator.style.unityTextAlign = TextAnchor.MiddleCenter;
            indicator.style.backgroundColor = Color.white;
            indicator.style.alignSelf = Align.FlexStart;
            indicator.style.borderBottomWidth = 1;
            indicator.style.borderTopWidth = 1;
            indicator.style.borderBottomColor = Color.black;
            indicator.style.borderTopColor = Color.black;
            indicator.text = "TYPE";
            indicator.tooltip = "Choose variable type";
            Indicator = indicator;

            void SetActive()
            {
                VVariableUtils.ActiveVariableWindow.mainListv.ClearSelection();
                VVariableUtils.ActiveVariableWindow.mainListv.SetSelection(VLabelIndex);
            }

            toolbar.Add(indicator);
            toolbar.Add(content);

            var box = new Box();
            box.style.flexWrap = Wrap.Wrap;
            box.style.unityTextAlign = TextAnchor.MiddleCenter;
            box.style.backgroundColor = Color.white;
            box.style.alignSelf = Align.FlexStart;
            box.style.width = 300;
            box.style.borderBottomWidth = 1;
            box.style.borderTopWidth = 1;
            box.style.borderBottomColor = Color.black;
            box.style.borderTopColor = Color.black;
            box.name = "boxContainer";
            FieldContainer = box;
            toolbar.Add(box);

            thistoolbar = toolbar;
            this.Add(toolbar);

            var dropd = new Toggle();
            dropd.text = "Public";
            dropd.style.width = 50;
            dropd.value = false;
            publicToggle = dropd;
            this.Add(dropd);
        }
        public ToolbarMenu Indicator { get; set; }
        //Color state when selected. 
        public void SetContentColor(Color col)
        {
            VBlockContent.style.backgroundColor = col;
        }
        public void SetVVariableColor(int defWidth, Color defColor)
        {
            this.style.borderTopWidth = defWidth;
            this.style.borderBottomWidth = defWidth;
            this.style.borderLeftWidth = defWidth;
            this.style.borderRightWidth = defWidth;

            this.style.borderTopColor = defColor;
            this.style.borderBottomColor = defColor;
            this.style.borderLeftColor = defColor;
            this.style.borderRightColor = defColor;
        }
        public IntegerField IntLabel(bool addToList, int value, string name)
        {
            IntegerField intField = new IntegerField();
            intField.name = "intField";
            intField.style.width = 295;
            intField.style.height = DefHeight - 9;
            intField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VInteger { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    intField.SetEnabled(false);
                }
            }

            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];

                    if (inst == null || inst is not VInteger)
                        return;

                    (inst as VInteger).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });

            intField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VInteger).value = intField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return intField;
        }
        public Vector2Field VecTwoLabel(bool addToList, Vector2 value, string guid)
        {
            var vecField = new Vector2Field();
            vecField.name = "vec2Field";
            vecField.style.width = 295;
            vecField.style.height = DefHeight - 9;
            vecField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VVector2 { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    vecField.SetEnabled(false);
                }
            }

            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];

                    if (inst == null || inst is not VVector2)
                        return;

                    (inst as VVector2).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            vecField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VVector2).value = vecField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return vecField;
        }
        public Vector3Field VecThreeLabel(bool addToList, Vector3 value, string name)
        {
            var vecField = new Vector3Field();
            vecField.name = "vec3Field";
            vecField.style.width = 295;
            vecField.style.height = DefHeight - 9;
            vecField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VVector3 { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    vecField.SetEnabled(false);
                }
            }
            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];

                    if (inst == null || inst is not VVector3)
                        return;

                    (inst as VVector3).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            vecField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VVector3).value = vecField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return vecField;
        }
        public Vector4Field VecFourLabel(bool addToList, Vector4 value, string name)
        {
            var vecField = new Vector4Field();
            vecField.name = "vec4Field";
            vecField.style.width = 295;
            vecField.style.height = DefHeight - 9;
            vecField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VVector4 { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    vecField.SetEnabled(false);
                }
            }
            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];

                    if (inst == null || inst is not VVector4)
                        return;

                    (inst as VVector4).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            vecField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VVector4).value = vecField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return vecField;
        }
        public DoubleField DoubleLabel(bool addToList, Double value, string name)
        {
            var dobField = new DoubleField();
            dobField.name = "doubleField";
            dobField.style.width = 295;
            dobField.style.height = DefHeight - 9;
            dobField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VDouble { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    dobField.SetEnabled(false);
                }
            }
            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    var vcores = Resources.FindObjectsOfTypeAll<VCoreUtil>();

                    if (inst == null || inst is not VDouble)
                        return;

                    (inst as VDouble).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            dobField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VDouble).value = dobField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return dobField;
        }
        public FloatField FloatLabel(bool addToList, float value, string name)
        {
            var floatField = new FloatField();
            floatField.name = "floatField";
            floatField.style.width = 295;
            floatField.style.height = DefHeight - 9;
            floatField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VFloat { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    floatField.SetEnabled(false);
                }
            }
            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];

                    if (inst == null || inst is not VFloat)
                        return;

                    (inst as VFloat).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            floatField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VFloat).value = floatField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return floatField;
        }
        public TextField StringLabel(bool addToList, string value, string name)
        {
            var strField = new TextField();
            strField.name = "stringField";
            strField.style.width = 295;
            strField.style.height = DefHeight - 9;
            strField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VString { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    strField.SetEnabled(false);
                }
            }
            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];

                    if (inst == null || inst is not VString)
                        return;

                    (inst as VString).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            strField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VString).value = strField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return strField;
        }
        public Toggle BoolLabel(bool addToList, bool value, string name)
        {
            var boolField = new Toggle();
            boolField.text = " Value";
            boolField.name = "boolField";
            boolField.style.width = 295;
            boolField.style.height = DefHeight - 9;
            boolField.value = value;

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VBoolean { Name = VVariableUtils.GetVarNames(name), guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    boolField.SetEnabled(false);
                }
            }

            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];

                    if (inst == null || inst is not VBoolean)
                        return;

                    (inst as VBoolean).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            boolField.RegisterCallback<FocusOutEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    var inst = PortsUtils.variable.ivar[(int)this.userData];
                    (inst as VBoolean).value = boolField.value;
                    PortsUtils.SetActiveAssetDirty();
                }
            });
            return boolField;
        }
        //TODO : BROKEN!

        public Label ListLabel(bool addToList, VTypes value, string name)
        {
            var lblField = new Label();
            lblField.style.backgroundColor = Color.green;
            lblField.name = "listField";
            lblField.style.width = 295;
            lblField.style.height = DefHeight - 9;
            lblField.tooltip = "C# Generic List. \nCan be looped with For-loop vblock";

            if (addToList)
            {
                if(PortsUtils.variable.ivar[(int)this.userData] != null)
                {
                    IVar ivar = new VList { guid = PortsUtils.variable.GlobalCounter++, sceneGuid = AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() };
                    PortsUtils.variable.ivar[(int)this.userData] = ivar;
                    PortsUtils.variable.ivar[(int)this.userData].Name = VVariableUtils.GetVarNames(name);
                    PortsUtils.SetActiveAssetDirty();
                }
            }
            else
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if(AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString() != inst.SceneGuid)
                {
                    lblField.SetEnabled(false);
                }
            }
            var dropd = new ToolbarMenu { text = "SELECT TYPE", variant = ToolbarMenu.Variant.Default };
            dropd.style.height = DefHeight - 9;
            dropd.style.backgroundColor = Color.green;
            dropd.style.unityTextAlign = TextAnchor.MiddleCenter;
            dropd.text = value.ToString();

            dropd.menu.AppendAction("String", a =>
                {
                    dropd.text = nameof(VTypes.String);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<string>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.String;
                    PortsUtils.SetActiveAssetDirty();
                });

            dropd.menu.AppendAction("Float", a =>
                {
                    dropd.text = nameof(VTypes.Float);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<float>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.Float;
                    PortsUtils.SetActiveAssetDirty();
                });

            dropd.menu.AppendAction("Double", a =>
                {
                    dropd.text = nameof(VTypes.Double);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<double>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.Double;
                    PortsUtils.SetActiveAssetDirty();
                });

            dropd.menu.AppendAction("Integer", a =>
                {
                    dropd.text = nameof(VTypes.Integer);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<int>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.Integer;
                    PortsUtils.SetActiveAssetDirty();
                });

            dropd.menu.AppendAction("Boolean", a =>
                {
                    dropd.text = nameof(VTypes.Boolean);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<bool>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.Boolean;
                    PortsUtils.SetActiveAssetDirty();
                });

            dropd.menu.AppendAction("Vector2", a =>
                {
                    dropd.text = nameof(VTypes.Vector2);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<Vector2>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.Vector2;
                    PortsUtils.SetActiveAssetDirty();
                });

            dropd.menu.AppendAction("Vector3", a =>
                {
                    dropd.text = nameof(VTypes.Vector3);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<Vector3>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.Vector3;
                    PortsUtils.SetActiveAssetDirty();
                });

            dropd.menu.AppendAction("Vector4", a =>
                {
                    dropd.text = nameof(VTypes.Vector4);
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).value = new List<Vector4>();
                    (PortsUtils.variable.ivar[(int)this.userData] as VList).dataType = VTypes.Vector4;
                    PortsUtils.SetActiveAssetDirty();
                });

            lblField.Add(dropd);

            this.VBlockContent.RegisterCallback<FocusOutEvent>((x) =>
            {
                var inst = PortsUtils.variable.ivar[(int)this.userData];

                if (inst == null || (inst is VList val && val == null))
                    return;

                (inst as VList).Name = VVariableUtils.GetVarNames(this.VBlockContent.value);
                PortsUtils.SetActiveAssetDirty();
            });
            return lblField;
        }

        public void InsertUiField(VisualElement vis, bool insertToVcontainer)
        {
            if (FieldContainer.childCount > 0)
            {
                foreach (var items in FieldContainer.Children())
                {
                    FieldContainer.Remove(items);
                    break;
                }
            }

            if (insertToVcontainer)
                FieldContainer.Add(vis);
        }
    }
}