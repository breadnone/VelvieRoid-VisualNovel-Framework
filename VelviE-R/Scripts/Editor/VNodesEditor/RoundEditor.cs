using System.Collections.Generic;
using UnityEditor;
using VelvieR;
using UnityEngine.UIElements;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(Round))]
    public class RoundEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            dummySlot = new VisualElement();

            var t = target as Round;
            root.Add(DrawToggle(t));
            root.Add(dummySlot);
            dummySlot.Add(DrawVars(t));
            root.Add(DrawBool(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawBool(Round t)
        {
            var rootBox = VUITemplate.GetTemplate("Round up/down : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.roundUp;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.roundUp = x.newValue;
                });
            }

            return rootBox;
        }
        private VisualElement DrawToggle(Round t)
        {
            var rootBox = VUITemplate.GetTemplate("Type : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.choices = new List<string> { "Float", "Double" };
            objField.value = t.vtype.ToString();

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    RemoveChild();

                    if (x.newValue == "Float")
                    {
                        t.vtype = VTypes.Float;
                        t.variable = null;
                    }
                    else
                    {
                        t.vtype = VTypes.Double;
                        t.variable = null;
                    }

                    dummySlot.Add(DrawVars(t));
                });
            }

            return rootBox;
        }
        private VisualElement DrawVars(Round t)
        {
            var varTemplate = VUITemplate.VariableTemplate(type: t.vtype);

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
                    if (x.GetVtype() == t.vtype)
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

        private void RemoveChild()
        {
            if (dummySlot.childCount > 0)
            {
                foreach (var child in dummySlot.Children().ToList())
                {
                    child.RemoveFromHierarchy();
                }
            }
        }
    }
}