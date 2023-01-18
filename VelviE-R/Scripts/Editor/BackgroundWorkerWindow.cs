using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor.Experimental;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    public class BackgroundWorkerWindow : EditorWindow
    {
        private VisualElement root;
        public ListView mainListv { get; set; }
        void OnEnable()
        {
            var getIvar = PortsUtils.GetVariableScriptableObjects();

            if(getIvar == null || getIvar.Count == 0)
            {
                PortsUtils.CreateVariableAsset();
            }
            else
            {
                PortsUtils.variable = getIvar[0];
            }

            if(PortsUtils.variable == null)
            {
                PortsUtils.variable = PortsUtils.GetVariableScriptableObjects()[0];
            }

            SetToolbar();
        }
        void OnDisable()
        {

        }

        public void RebuildListV()
        {
            if (mainListv != null)
                mainListv.Rebuild();
        }

        public void SetToolbar()
        {
            var toolbar = new Toolbar();
            toolbar.style.height = 50;


            root = rootVisualElement;
            rootVisualElement.Add(toolbar);
            rootVisualElement.Add(Container());
        }
        private VisualElement pOne;
        private VisualElement pTwo;

        private VisualElement Container()
        {
            var vis = new VisualElement();
            vis.style.flexDirection = FlexDirection.Row;

            var parentOne = new VisualElement();
            parentOne.style.flexDirection = FlexDirection.Column;
            parentOne.style.width = 200;
            parentOne.style.marginLeft = 10;
            pOne = parentOne;
            vis.Add(parentOne);

            var parentTwo = new VisualElement();
            parentTwo.style.width = 400;
            parentTwo.style.marginLeft = 10;
            pTwo = parentTwo;
            vis.Add(parentTwo);

            DrawParentOneContent();
            DrawParentTwoContent();
            
            return vis;

        }
        private void DrawParentOneContent()
        {
            var summary = new Label();
            summary.style.width = 200;
            summary.style.height = 59;
            summary.style.marginTop = 5;
            summary.style.marginLeft = 3;

            var vis = new DropdownField();
            pOne.Add(vis);
            pOne.Add(summary);

            vis.style.width = 200;
            vis.style.height = 59;
            vis.style.marginTop = 5;
            vis.choices = new List<string>{"RunOnThreadPool", "RunOnUpdate"};

            if(PortsUtils.variable.scheduler.Count > 0)
            {
                if(PortsUtils.variable.scheduler.Find(x => x != null).RunOnThreadPool)
                {
                    vis.value = "RunOnThreadPool";
                    summary.text = "Will run on separate thread.<b>\nNOT COMPATIBLE WITH WegGL.</b>";
                }
                else
                {
                    vis.value = "RunOnUpdate";
                    summary.text = "Will run on Unity's Update function.<b>\nCOMPATIBLE WITH WegGL.</b>";
                }
            }
            else
            {
                vis.value = "RunOnThreadPool";
            }
            
            vis.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(x.newValue == "RunOnThreadPool")
                {
                    summary.text = "Will run on separate thread.<b>\nNOT COMPATIBLE WITH WegGL.</b>";

                    foreach(var t in PortsUtils.variable.scheduler)
                    {
                        t.RunOnThreadPool = true;
                    }
                }
                else
                {
                    summary.text = "Will run on Unity's Update function.<b>\nCOMPATIBLE WITH WegGL.</b>";

                    foreach(var t in PortsUtils.variable.scheduler)
                    {
                        t.RunOnThreadPool = false;
                    }
                }
            });
        }

        private void DrawParentTwoContent()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            Func<VisualElement> makeItem = () => 
            {
                var vis = new VisualElement();
                vis.style.flexDirection = FlexDirection.Row;
                vis.style.width = 730;

                var txt = new TextField();
                txt.value = "New scheduler";
                txt.name = "txtField";
                txt.style.width = 160;
                vis.Add(txt);

                var time = new DoubleField();
                time.name = "timeField";
                time.style.width = 50;
                vis.Add(time);

                var drop = new DropdownField();
                drop.name = "dropD";
                drop.style.width = 160;
                drop.value = "Every-N-seconds";
                drop.choices = new List<string>{"Every N seconds", "Every N minutes"};
                vis.Add(drop);

                var dropV = new DropdownField();
                dropV.name = "vgraph";
                dropV.style.width = 160;
                dropV.value = "<AssignVgraph>";

                var graphs = PortsUtils.GetVGprahsScriptableObjects();
                var rlist = new List<string>();

                graphs.ForEach(x => 
                {
                    rlist.Add(x.vgraphGOname);
                });

                dropV.choices = rlist;
                vis.Add(dropV);

                var nodev = new DropdownField();
                nodev.name = "vnode";
                nodev.style.width = 160;
                nodev.value = "<AssignVnode>";

                var nlist = new List<string>();

                foreach(var port in graphs)
                {
                    if(port.vports.Count > 0)
                    {
                        foreach(var node in port.vports)
                        {
                            if(node.vport != null)
                            nlist.Add(node.vport.vnodeProperty.nodeName);
                        }
                    }
                }
                nodev.choices = nlist;
                vis.Add(nodev);
                return vis;
            };

            Action<VisualElement, int> bindItem = (e, i) => 
            {
                //(e as Label).text = items[i];
                foreach(var t in e.Children().ToList())
                {
                    if(t.name == "txtField")
                    {
                        var astype = t as TextField;
                        
                        if(String.IsNullOrEmpty(PortsUtils.variable.scheduler[i].name))
                        {
                            astype.value = "New scheduler";
                        }
                        else
                        {
                            astype.value = PortsUtils.variable.scheduler[i].name;
                        }
                        
                        astype.RegisterValueChangedCallback((x)=>
                        {
                            PortsUtils.variable.scheduler[i].name = x.newValue;
                            EditorUtility.SetDirty(PortsUtils.variable);
                        });
                    }
                    else if(t.name == "timeField")
                    {
                        var astype = t as DoubleField;
                        astype.value = PortsUtils.variable.scheduler[i].nvalue;

                        astype.RegisterValueChangedCallback((x)=>
                        {
                            PortsUtils.variable.scheduler[i].nvalue = x.newValue;
                            EditorUtility.SetDirty(PortsUtils.variable);
                        });
                    }
                    else if(t.name == "dropD")
                    {
                        var astype = t as DropdownField;

                        if(PortsUtils.variable.scheduler[i].isMinutes)
                        {
                            astype.value = "Every N minutes";
                        }
                        else
                        {
                            astype.value = "Every N seconds";
                        }

                        astype.RegisterCallback<ChangeEvent<string>>((x)=>
                        {
                            if(x.newValue == "Every N minutes")
                            {
                                PortsUtils.variable.scheduler[i].isMinutes = true;
                                EditorUtility.SetDirty(PortsUtils.variable);
                            }
                            else
                            {
                                PortsUtils.variable.scheduler[i].isMinutes = false;
                                EditorUtility.SetDirty(PortsUtils.variable);
                            }
                        });
                    }
                    else if(t.name == "vgraph")
                    {
                        var astype = t as DropdownField;

                        if(String.IsNullOrEmpty(PortsUtils.variable.scheduler[i].vcore))
                        {
                            astype.value = "<AssignVgraph>";
                        }
                        else
                        {
                            astype.value = PortsUtils.variable.scheduler[i].vcore;
                        }

                        astype.RegisterCallback<ChangeEvent<string>>((x)=>
                        {  
                            PortsUtils.variable.scheduler[i].vcore = x.newValue;
                            EditorUtility.SetDirty(PortsUtils.variable);
                        });
                    }
                    else if(t.name == "vnode")
                    {
                        var astype = t as DropdownField;

                        if(String.IsNullOrEmpty(PortsUtils.variable.scheduler[i].vnode))
                        {
                            astype.value = "<AssignVnode>";
                        }
                        else
                        {
                            astype.value = PortsUtils.variable.scheduler[i].vnode;
                        }

                        astype.RegisterCallback<ChangeEvent<string>>((x)=>
                        {  
                            PortsUtils.variable.scheduler[i].vnode = x.newValue;
                            EditorUtility.SetDirty(PortsUtils.variable);
                        });
                    }
                }
            };

            const int itemHeight = 60;
            var listView = new ListView(PortsUtils.variable.scheduler, itemHeight, makeItem, bindItem);
            listView.style.width = 730;
            listView.showBorder = true;
            listView.selectionType = SelectionType.Single;
            listView.reorderable = true;

            var con = new VisualElement();
            con.style.flexDirection = FlexDirection.Row;

            var btnAdd = new Button();
            btnAdd.style.width = 40;
            btnAdd.style.height = 40;
            btnAdd.text = "+";

            btnAdd.clicked += ()=>
            {
                PortsUtils.variable.scheduler.Add(new BackgroundScheduler());
                listView.Rebuild();
                EditorUtility.SetDirty(PortsUtils.variable);
            };

            var btnRem = new Button();
            btnRem.style.width = 40;
            btnRem.style.height = 40;
            btnRem.text = "-";

            btnRem.clicked += ()=>
            {
                if(listView.selectedItem != null)
                PortsUtils.variable.scheduler.RemoveAt(listView.selectedIndex);
                else if(PortsUtils.variable.scheduler.Count > 0)
                PortsUtils.variable.scheduler.RemoveAt(PortsUtils.variable.scheduler.Count - 1);
                listView.Rebuild();

                EditorUtility.SetDirty(PortsUtils.variable);
            };

            con.Add(btnAdd);
            con.Add(btnRem);

            root.Add(listView);
            root.Add(con);
            pTwo.Add(root);
         }
    }
}
