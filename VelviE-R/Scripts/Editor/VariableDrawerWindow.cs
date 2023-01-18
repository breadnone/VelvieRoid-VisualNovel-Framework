using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor.Experimental;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using VelvieR;
using UnityEditor.SceneManagement;
using System.Linq;

namespace VIEditor
{
    public class VariableDrawerWindow : EditorWindow
    {
        private UnityEngine.UIElements.PopupWindow varWindow;
        public ListView mainListv { get; set; }
        void OnEnable()
        {
            var getIvar = PortsUtils.GetVariableScriptableObjects();

            if (getIvar == null || getIvar.Count == 0)
            {
                PortsUtils.CreateVariableAsset();
            }
            else
            {
                PortsUtils.variable = getIvar[0];
            }

            //Remove orphans!
            var scenes = AssetDatabase.FindAssets("t:Scene");

                for(int i = 0; i < PortsUtils.variable.ivar.Count; i++)
                {
                    if(PortsUtils.variable.ivar[i] == null)
                        continue;

                    for(int j = 0; j < PortsUtils.variable.ivar.Count; j++)
                    {
                        var iguid = PortsUtils.variable.ivar[j].SceneGuid;

                        if(String.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(iguid)))
                        {
                            PortsUtils.variable.ivar.RemoveAt(j);
                        }
                    }
                }
            
            ////////

            VVariableUtils.ActiveVariableWindow = this;
            VVariableUtils.variableWindowIsActive = true;
            SetVaribleListDrawer();
        }
        void OnDisable()
        {
            VVariableUtils.ActiveVariableWindow = null;
            VVariableUtils.variableWindowIsActive = false;
            PortsUtils.variable = null;
        }
        public void SetVaribleListDrawer()
        {
            if (varWindow == null)
            {
                varWindow = VariableWindow();
                rootVisualElement.Add(varWindow);

                SetToolbar();
                varWindow.Add(VariableListV());
            }
        }
        public void RebuildListV()
        {
            if (mainListv != null)
                mainListv.Rebuild();
        }

        public void SetToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.style.height = 30;

            var btnAdd = new Button(() =>
            {
                VVariableUtils.CreateEmptyVariable(mainListv);

                SortIvarList();
                
                if (mainListv != null && PortsUtils.variable != null && PortsUtils.variable.ivar.Count > 0)
                    mainListv.ScrollToId(PortsUtils.variable.ivar.Count - 1);
            });

            btnAdd.style.height = 25;
            btnAdd.style.width = 50;
            btnAdd.text = "ADD";

            var btnRem = new Button(() =>
            {
                if(!PortsUtils.PlayMode)
                {                
                    if (PortsUtils.variable.ivar.Count > 0 && mainListv.selectedItem != null)
                    {
                        if((mainListv.selectedItem as IVar).SceneGuid == AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                        {
                            PortsUtils.variable.ivar.Remove(mainListv.selectedItem as IVar);
                            mainListv.ClearSelection();
                            RebuildListV();
                            PortsUtils.SetActiveAssetDirty();
                        }
                    }
                }
            });

            btnRem.style.height = 25;
            btnRem.style.width = 50;
            btnRem.text = "DEL";

            var btnSort = new Button(() =>
            {
                if (PortsUtils.variable != null && PortsUtils.variable.ivar.Count > 0)
                {
                    mainListv.ClearSelection();
                    PortsUtils.variable.ivar.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));

                    for (int i = 0; i < PortsUtils.variable.ivar.Count; i++)
                    {
                        PortsUtils.variable.ivar[i].Vindex = i;
                    }

                    RebuildListV();
                    PortsUtils.SetActiveAssetDirty();
                }
            });

            btnSort.style.height = 25;
            btnSort.style.width = 50;
            btnSort.text = "SORT";
            /*
                        var btnSaveTest = new Button(() =>
                        {
                            if (PortsUtils.activeVGraphAssets != null && PortsUtils.activeVGraphAssets.variables.Count > 0)
                            {
                                var js = new VJsonAdapter();
                                js.StartSerializing(PortsUtils.activeVGraphAssets.variables, PortsUtils.activeVGraphAssets.govcoreid.ToString());
                            }
                        });
                        btnSaveTest.style.height = 25;
                        btnSaveTest.style.width = 50;
                        btnSaveTest.text = "Save Test!";

                        var btnSerializeOnDemand = new Button(() =>
                        {
                            if (PortsUtils.activeVGraphAssets != null && PortsUtils.activeVGraphAssets.variables.Count > 0)
                            {
                                VVariableUtils.ReSerializeOnEditorPlayMode(PortsUtils.activeVGraphAssets.govcoreid);
                            }
                        });
                        btnSerializeOnDemand.style.height = 25;
                        btnSerializeOnDemand.style.width = 90;
                        btnSerializeOnDemand.text = "SrlzOnDemand";
            */

            var searchBar = new ToolbarSearchField();
            searchBar.style.width = 230;
            searchBar.style.height = 25;
            searchBar.focusable = true;

            searchBar.RegisterCallback<KeyDownEvent>((x) =>
            {
                if (!PortsUtils.PlayMode)
                {
                    if (x.keyCode == KeyCode.Return)
                    {
                        if (!String.IsNullOrEmpty(searchBar.value))
                            SearchVariables(searchBar.value);
                    }
                }
            });

            toolbar.Add(btnAdd);
            toolbar.Add(btnRem);
            toolbar.Add(btnSort);
            toolbar.Add(searchBar);
            rootVisualElement.Add(toolbar);
        }
        public void SearchVariables(string varName)
        {
            int? index = PortsUtils.variable.ivar.FindIndex(x => x.Name.Equals(varName, StringComparison.OrdinalIgnoreCase));

            if (index.HasValue)
            {
                mainListv.ClearSelection();
                mainListv.ScrollToId(index.Value);
            }
        }
        private List<IVar> sortList;
        private void SortIvarList()
        {
                sortList = new List<IVar>();
                
                foreach(var ivar in PortsUtils.variable.ivar)
                {
                    if(ivar == null)
                        continue;

                    if (ivar.IsPublic || ivar.SceneGuid == AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                    {
                        sortList.Add(ivar);
                    }
                }

                if(mainListv != null)
                {
                    mainListv.itemsSource = sortList;
                    mainListv.Rebuild();
                }
        }
        public ListView VariableListV()
        {
            if (PortsUtils.activeVGraphAssets != null && VVariableUtils.ActiveVariableWindow != null)
            {
                SortIvarList();
                
                const int defHeight = 30;
                const int defWidth = 260;
                const int itemHeight = 30;

                Func<VVariableLabel> makeItem = () => new VVariableLabel("Var", VColor.Magenta, defHeight, defWidth, 0, true);

                Action<VisualElement, int> bindItem = (vv, i) =>
                {
                    if (sortList[i].IsPublic || sortList[i].SceneGuid == AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                    {
                        var vb = vv as VVariableLabel;
                        vb.userData = (int)i;

                        vb.VBlockLineNumber.text = (i + 1).ToString();
                        vb.VBlockContent.value = sortList[i].Name;
                        vb.VLabelIndex = i;

                        sortList[i].Vindex = i;
                        var objtype = sortList[i].GetVtype();

                                if(sortList[i].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                {
                                    vb.SetEnabled(false);
                                }
                                else
                                {
                                    vb.SetEnabled(true);
                                }

                        switch (objtype)
                        {
                            case VTypes.String:
                                var vstr = sortList[i] as VString;
                                vb.Indicator.text = "String";
                                var vstrval = vb.StringLabel(false, vstr.value, vstr.name);
                                vb.InsertUiField(vstrval, true);
                                vb.publicToggle.value = vstr.IsPublic;

                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                            case VTypes.Float:
                                var vfloat = sortList[i] as VFloat;
                                vb.Indicator.text = "Float";
                                var vfloatval = vb.FloatLabel(false, vfloat.value, vfloat.name);
                                vb.InsertUiField(vfloatval, true);
                                vb.publicToggle.value = vfloat.IsPublic;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                            case VTypes.Integer:
                                var vint = sortList[i] as VInteger;
                                vb.Indicator.text = "Integer";
                                var vintval = vb.IntLabel(false, vint.value, vint.name);
                                vb.InsertUiField(vintval, true);
                                vb.publicToggle.value = vint.IsPublic;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                            case VTypes.Boolean:
                                var vbool = sortList[i] as VBoolean;
                                vb.Indicator.text = "Boolean";
                                var vboolval = vb.BoolLabel(false, vbool.value, vbool.name);
                                vb.InsertUiField(vboolval, true);
                                vb.publicToggle.value = vbool.IsPublic;

                                if(sortList[i].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                {
                                    vb.SetEnabled(false);
                                }
                                else
                                {
                                    vb.SetEnabled(true);
                                }
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                            case VTypes.Double:
                                var vdob = sortList[i] as VDouble;
                                vb.Indicator.text = "Double";
                                var vdobval = vb.DoubleLabel(false, vdob.value, vdob.name);
                                vb.InsertUiField(vdobval, true);
                                vb.publicToggle.value = vdob.IsPublic;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if(sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                            case VTypes.Vector2:
                                var vvec2 = sortList[i] as VVector2;
                                vb.Indicator.text = "Vector2";
                                var vvec2val = vb.VecTwoLabel(false, vvec2.value, vvec2.name);
                                vb.InsertUiField(vvec2val, true);
                                vb.publicToggle.value = vvec2.IsPublic;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });

                                break;
                            case VTypes.Vector3:
                                var vvec3 = sortList[i] as VVector3;
                                vb.Indicator.text = "Vector3";
                                var vvec3val = vb.VecThreeLabel(false, vvec3.value, vvec3.name);
                                vb.InsertUiField(vvec3val, true);
                                vb.publicToggle.value = vvec3.IsPublic;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                            case VTypes.Vector4:
                                var vvec4 = sortList[i] as VVector4;
                                vb.Indicator.text = "Vector4";
                                var vvec4val = vb.VecFourLabel(false, vvec4.value, vvec4.name);
                                vb.InsertUiField(vvec4val, true);
                                vb.publicToggle.value = vvec4.IsPublic;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                            case VTypes.VList:
                                var vlist = sortList[i] as VList;
                                vb.Indicator.text = "VList";
                                var vlistval = vb.ListLabel(false, vlist.dataType, vlist.name);
                                vb.InsertUiField(vlistval, true);
                                vb.publicToggle.value = vlist.IsPublic;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if(sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });

                                break;
                            case VTypes.None:
                                var non = sortList[i];
                                non.Vindex = i;
                                vb.Indicator.text = "TYPE";
                                vb.publicToggle.value = false;
                                vb.publicToggle.RegisterValueChangedCallback(x =>
                                {
                                    var idx = i;

                                    if (x.newValue)
                                    {
                                        sortList[idx].IsPublic = true;
                                        PortsUtils.SetActiveAssetDirty();
                                    }
                                    else
                                    {
                                        sortList[idx].IsPublic = false;

                                        if (sortList[idx].SceneGuid != AssetDatabase.GUIDFromAssetPath(EditorSceneManager.GetActiveScene().path).ToString())
                                        {
                                            PortsUtils.SetActiveAssetDirty();
                                            mainListv?.Rebuild();
                                        }
                                    }
                                });
                                break;
                        }
                    }
                };

                if (varWindow != null && mainListv != null)
                    varWindow.Remove(mainListv);

                mainListv = new ListView(sortList, itemHeight, makeItem, bindItem);
                mainListv.virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
                mainListv.showBorder = true;
                mainListv.selectionType = SelectionType.Single;
                mainListv.reorderMode = ListViewReorderMode.Animated;
                mainListv.reorderable = true;
                mainListv.style.alignContent = Align.Center;

                mainListv.itemsSourceChanged += () =>
                {
                    //RecopyVariableToVCoreutilObject();
                    PortsUtils.SetActiveAssetDirty();
                };
            }
            return mainListv;
        }

        public UnityEngine.UIElements.PopupWindow VariableWindow()
        {
            var t = new UnityEngine.UIElements.PopupWindow();
            t.name = "variableWindow";
            t.text = "CREATE GLOBAL VARIABLES";
            t.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            t.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            t.StretchToParentSize();
            return t;
        }
    }
}
