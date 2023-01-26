using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using UnityEditor.UIElements;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(SetRotation))]
    public class SetRotationEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as SetRotation;

            root.Add(DrawObject(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        public VisualElement DrawObject(SetRotation t)
        {
            var rootBox = VUITemplate.GetTemplate("GameObject : ");
            var field = VUITemplate.GetField(rootBox);

            Func<VisualElement> makeItem = () =>
            {
                var vis = new VisualElement();
                var t = new ObjectField();
                t.name = "obj";
                var tt = new ObjectField();
                tt.name = "trans";
                var tv = new Vector3Field();
                tv.name = "vec";

                t.objectType = typeof(GameObject);
                t.allowSceneObjects = true;
                tt.objectType = typeof(Transform);
                tt.allowSceneObjects = true;
                vis.Add(t);
                vis.Add(tv);
                vis.Add(tt);
                return vis;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                foreach (var vis in e.Children().ToList())
                {
                    if (vis.name == "obj")
                    {
                        var astype = vis as ObjectField;
                        astype.value = t.go[i].gameobject;

                        if(!PortsUtils.PlayMode)
                        {
                            astype.RegisterValueChangedCallback((x) =>
                            {
                                t.go[i].gameobject = x.newValue as GameObject;
                            });
                        }

                    }
                    else if (vis.name == "trans")
                    {
                        var astype = vis as ObjectField;
                        astype.value = t.go[i].target;

                        if (!PortsUtils.PlayMode)
                        {
                            astype.RegisterValueChangedCallback((x) =>
                            {
                                t.go[i].target = x.newValue as Transform;
                            });
                        }

                    }
                    else if (vis.name == "vec")
                    {
                        var astype = vis as Vector3Field;
                        astype.value = t.go[i].euler;

                        if (!PortsUtils.PlayMode)
                        {
                            astype.RegisterValueChangedCallback((x) =>
                            {
                                t.go[i].euler = x.newValue;
                            });
                        }
                    }
                }
            };

            var btnAdd = new Button();
            btnAdd.style.width = 30;
            btnAdd.style.height = 20;
            btnAdd.text = "+";

            var btnRem = new Button();
            btnRem.style.width = 30;
            btnRem.style.height = 20;
            btnRem.text = "-";

            const int itemHeight = 70;
            var objField = new ListView(t.go, itemHeight, makeItem, bindItem);
            objField.reorderable = false;
            objField.selectionType = SelectionType.Single;
            objField.showBorder = true;
            objField.style.width = field.style.width;
            field.style.flexDirection = FlexDirection.Column;
            field.Add(objField);

            if (!PortsUtils.PlayMode)
            {
                btnAdd.clicked += () =>
                {
                    t.go.Add(new GameObjectClass());
                    objField.Rebuild();
                };
                btnRem.clicked += () =>
                {
                    if (objField.selectedItem != null)
                    {
                        t.go.RemoveAt(objField.selectedIndex);
                    }
                    else
                    {
                        if (t.go.Count > 0)
                            t.go.RemoveAt(t.go.Count - 1);
                    }
                    objField.Rebuild();
                };
            }

            var visEl = new VisualElement();
            visEl.style.flexDirection = FlexDirection.Row;
            field.Add(visEl);
            visEl.Add(btnAdd);
            visEl.Add(btnRem);
            return rootBox;
        }
    }
}