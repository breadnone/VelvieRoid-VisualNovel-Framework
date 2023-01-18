using UnityEditor;
using VelvieR;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;
using TMPro;
using System.Collections.Generic;

namespace VIEditor
{
    [CustomEditor(typeof(GetTime))]
    public class GetTimeEditor : Editor
    { 
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as GetTime;
            root.Add(DrawCategory(t));
            root.Add(DrawVars(t));
            root.Add(DrawText(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawVars(GetTime t)
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
                    if(x.GetVtype() == VTypes.String)
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
        public VisualElement DrawCategory(GetTime t)
        {
            var rootBox = VUITemplate.GetTemplate("Type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.timeCategory.ToString();
            objField.choices = Enum.GetNames(typeof(TimeCategory)).ToList();

            objField.RegisterValueChangedCallback((x)=>
            {
                foreach(var tt in Enum.GetValues(typeof(TimeCategory)))
                {
                    var astype = (TimeCategory)tt;

                    if(tt.ToString() == x.newValue)
                    {
                        t.timeCategory = astype;
                    }
                }
            });

            return rootBox;
        }
        public VisualElement DrawText(GetTime t)
        {
            var rootBox = VUITemplate.GetTemplate("Text : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.allowSceneObjects = true;
            objField.objectType = typeof(TMP_Text);
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.text;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.text = x.newValue as TMP_Text;
            });

            return rootBox;
        }
    }
}