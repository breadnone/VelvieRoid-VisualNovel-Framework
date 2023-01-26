using System.Collections.Generic;
using VelvieR;
using UnityEditor;
using UnityEngine.UIElements;

namespace VIEditor
{
    [CustomEditor(typeof(LoadPlayerPrefs))]
    public class LoadPlayerPrefsEditor : Editor
    {
        private VisualElement dummySlot;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as LoadPlayerPrefs;
            dummySlot = new VisualElement();
            root.Add(DrawKey(t));
            root.Add(dummySlot);
            dummySlot.Add(DrawVars(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawKey(LoadPlayerPrefs t)
        {
            var rootBox = VUITemplate.GetTemplate("Key : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.key;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.key = objField.value;
                });
            }

            return rootBox;
        }

        private VisualElement DrawVars(LoadPlayerPrefs t)
        {
            var varTemplate = VUITemplate.VariableTemplate();

            if (t.localVariable == null)
            {
                varTemplate.child.value = "<None>";
            }
            else
            {
                if (PortsUtils.variable.ivar.Exists(x => x.Name == t.localVariable.Name))
                {
                    varTemplate.child.value = t.localVariable.Name;
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

            if (!PortsUtils.PlayMode)
            {
                varTemplate.child.RegisterCallback<ChangeEvent<string>>((evt) =>
                {
                    if (PortsUtils.variable.ivar.Count > 0)
                    {
                        if (evt.newValue == "<None>")
                        {
                            t.localVariable = null;
                            PortsUtils.SetActiveAssetDirty();
                        }
                        else
                        {
                            t.localVariable = PortsUtils.variable.ivar.Find(x => x.Name == evt.newValue);
                            PortsUtils.SetActiveAssetDirty();
                        }
                    }
                });
            }

            return varTemplate.root;
        }
    }
}