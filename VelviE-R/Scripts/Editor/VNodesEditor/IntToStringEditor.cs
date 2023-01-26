using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Linq;
using TMPro;

namespace VIEditor
{
    [CustomEditor(typeof(IntToString))]
    public class IntToStringEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as IntToString;
            
            root.Add(DrawVars(t));
            root.Add(DrawObject(t));
            root.Add(DrawType(t));
            root.Add(DrawAppend(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root; 
        }
        private VisualElement DrawObject(IntToString t)
        {
            var rootBox = VUITemplate.GetTemplate("Text component : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(TMP_Text);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.text;

            if(!PortsUtils.PlayMode)
            {
            objField.RegisterValueChangedCallback((x)=>
            {
                t.text = objField.value as TMP_Text;
            });
            }

            return rootBox;
        }
        private VisualElement DrawAppend(IntToString t)
        {
            var rootBox = VUITemplate.GetTemplate("Append text : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.appendText;

            if(!PortsUtils.PlayMode)
            {
            objField.RegisterValueChangedCallback((x)=>
            {
                t.appendText = objField.value;
            });
            }

            return rootBox;
        }
        private VisualElement DrawVars(IntToString t)
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
        private VisualElement DrawType(IntToString t)
        {
            var rootBox = VUITemplate.GetTemplate("PrefixSuffix : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            objField.value = t.prefixType.ToString();
            field.Add(objField);            

            if(!PortsUtils.PlayMode)
            {
                objField.choices = Enum.GetNames(typeof(PrefixSuffix)).ToList();
            objField.RegisterValueChangedCallback((x)=>
            {
                    foreach(var vals in Enum.GetValues(typeof(PrefixSuffix)))
                    {
                        var astype = (PrefixSuffix)vals;

                        if(astype.ToString() == x.newValue)
                            t.prefixType = astype;
                    }
            });
            }

            return rootBox;
        }
    }
}