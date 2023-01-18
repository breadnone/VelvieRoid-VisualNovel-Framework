using System.Collections.Generic;
using UnityEditor;
using VelvieR;
using UnityEngine.UIElements;
using System;

namespace VIEditor
{
    [CustomEditor(typeof(RandomString))]
    public class RandomStringEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as RandomString;
            root.Add(DrawVars(t));
            root.Add(DrawStringList(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawVars(RandomString t)
        {
            var varTemplate = VUITemplate.VariableTemplate(type: VTypes.String);

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
                PortsUtils.variable.ivar.ForEach((x) =>
                {
                    if (x.GetVtype() == VTypes.String)
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
        public VisualElement DrawStringList(RandomString t)
        {
            var rootBox = VUITemplate.GetTemplate("Min : ");
            var field = VUITemplate.GetField(rootBox);

            Func<TextField> makeItem = () => 
            {
                var vis = new TextField();
                return vis;
            };

            Action<VisualElement, int> bindItem = (e, i) => 
            {
                int idx = i;
                
                (e as TextField).value = t.words[i];

                (e as TextField).RegisterValueChangedCallback((x)=>
                {
                    t.words[idx] = x.newValue;
                });
            };

            const int itemHeight = 20;
            var objField = new ListView(t.words, itemHeight, makeItem, bindItem);
            objField.showAddRemoveFooter = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            return rootBox;
        }
    }
}