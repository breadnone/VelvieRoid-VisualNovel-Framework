using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(VMenuProperty))]
    public class VMenuPropertyEditor : Editor
    {
        private VisualElement container;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as VMenuProperty;
            container = new VisualElement();

            root.Add(DrawObject(t));
            root.Add(container);
            container.Add(DrawShowEffect(t));
            container.Add(DrawSetAsDefault(t));

            if (t.vmenu == null)
            {
                container.SetEnabled(false);
            }
            else
            {
                container.SetEnabled(true);
            }

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        private VisualElement DrawObject(VMenuProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("VMenu : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(VMenuOption);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.vmenu;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.vmenu = objField.value as VMenuOption;

                    if (t.vmenu != null)
                    {
                        container.SetEnabled(true);
                    }
                    else
                    {
                        container.SetEnabled(false);
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawShowEffect(VMenuProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Anim type : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.vmenu != null)
                objField.value = t.animType.ToString();

            if (!PortsUtils.PlayMode)
            {
                objField.choices = Enum.GetNames(typeof(MenuAnimType)).ToList();

                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.vmenu != null)
                    {
                        foreach (var vals in Enum.GetValues(typeof(MenuAnimType)))
                        {
                            var astype = (MenuAnimType)vals;

                            if (astype.ToString() == x.newValue)
                                t.animType = astype;
                        }
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawSetAsDefault(VMenuProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Set as default : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.vmenu != null)
                objField.value = t.setAsDefault;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.vmenu != null)
                    {
                        t.setAsDefault = objField.value;
                    }
                });
            }

            return rootBox;
        }
    }
}