using System.Collections.Generic;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VIEditor
{
    [CustomEditor(typeof(RandomInt))]
    public class RandomIntEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as RandomInt;
            root.Add(DrawVars(t));
            root.Add(DrawMin(t));
            root.Add(DrawMax(t));
            root.Add(DrawToggle(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawVars(RandomInt t)
        {
            var varTemplate = VUITemplate.VariableTemplate(type: VTypes.Integer);

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
                    if(x.GetVtype() == VTypes.Integer)
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
        public VisualElement DrawMin(RandomInt t)
        {
            var rootBox = VUITemplate.GetTemplate("Min : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.min;
            
            objField.RegisterValueChangedCallback((x)=>
            {
                t.min = objField.value;
            });

            return rootBox;
        }
        public VisualElement DrawMax(RandomInt t)
        {
            var rootBox = VUITemplate.GetTemplate("Max : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new IntegerField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.max;
            
            objField.RegisterValueChangedCallback((x)=>
            {
                t.max = objField.value;
            });

            return rootBox;
        }
        public VisualElement DrawToggle(RandomInt t)
        {
            var rootBox = VUITemplate.GetTemplate("Shuffle : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.shuffle;
            
            objField.RegisterValueChangedCallback((x)=>
            {
                t.shuffle = objField.value;
            });

            return rootBox;
        }
    }
}