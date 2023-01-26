using System.Collections.Generic;
using UnityEditor;
using VelvieR;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VIEditor
{
    [CustomEditor(typeof(RandomFloat))]
    public class RandomFloatEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as RandomFloat;
            root.Add(DrawVars(t));
            root.Add(DrawMin(t));
            root.Add(DrawMax(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawVars(RandomFloat t)
        {
            var varTemplate = VUITemplate.VariableTemplate(type: VTypes.Float);

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
                    if (x.GetVtype() == VTypes.Float)
                        varlist.Add(x.Name);
                });

                varlist.Add("<None>");
                varTemplate.child.choices = varlist;
            }

            if (!PortsUtils.PlayMode)
            {
                varTemplate.child.RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    if (PortsUtils.variable.ivar.Count > 0)
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
            }

            return varTemplate.root;
        }
        public VisualElement DrawMin(RandomFloat t)
        {
            var rootBox = VUITemplate.GetTemplate("Min : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.min;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.min = objField.value;
                });
            }

            return rootBox;
        }
        public VisualElement DrawMax(RandomFloat t)
        {
            var rootBox = VUITemplate.GetTemplate("Max : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.max;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.max = objField.value;
                });
            }

            return rootBox;
        }
    }
}