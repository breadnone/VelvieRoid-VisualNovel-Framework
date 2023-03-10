using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(VDialogProperty))]
    public class VDialogPropertyEditor : Editor
    {
        private VisualElement container;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as VDialogProperty;
            container = new VisualElement();

            root.Add(DrawObject(t));
            root.Add(container);
            container.Add(DrawDialogType(t));
            container.Add(DrawShowEffect(t));
            container.Add(DrawEnableWritingIndicator(t));
            container.Add(DrawSpeed(t));
            container.Add(DrawString(t));
            container.Add(DrawSetAsDefault(t));

            if (t.vdialog == null)
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
        private VisualElement DrawObject(VDialogProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Vdialog : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(VelvieDialogue);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.vdialog;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.vdialog = objField.value as VelvieDialogue;

                    if (t.vdialog != null)
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
        private VisualElement DrawString(VDialogProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Write indicator : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new TextField();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.vdialog != null)
                objField.value = t.writeIndicatorString;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.writeIndicatorString = objField.value;
                });
            }
            return rootBox;
        }
        private VisualElement DrawSpeed(VDialogProperty t)
        {
            var root = VUITemplate.VDropDownFieldTemplate(Enum.GetNames(typeof(VTextSpeed)).ToList(), "Writing Speed", false);
            //Enum slot writing speed

            root.child.value = t.textSpeed.ToString();

            if (!PortsUtils.PlayMode)
            {
                root.child.RegisterValueChangedCallback(x =>
                {
                    var enumss = Enum.GetValues(typeof(VTextSpeed));

                    foreach (var numsVals in enumss)
                    {
                        if (numsVals == null)
                            continue;

                        if (numsVals.ToString() == x.newValue)
                        {
                            t.textSpeed = (VTextSpeed)numsVals;
                            EditorUtility.SetDirty(t);
                            break;
                        }
                    }
                });
            }
            return root.root;
        }
        private VisualElement DrawShowEffect(VDialogProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Show effect : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.choices = Enum.GetNames(typeof(ShowHideEffect)).ToList();

            if (t.vdialog != null)
                objField.value = t.showEffect.ToString();

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.vdialog != null)
                    {
                        foreach (var vals in Enum.GetValues(typeof(ShowHideEffect)))
                        {
                            var astype = (ShowHideEffect)vals;

                            if (astype.ToString() == x.newValue)
                                t.showEffect = astype;
                        }
                    }
                });
            }
            return rootBox;
        }
        private VisualElement DrawDialogType(VDialogProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Dialog type : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.choices = Enum.GetNames(typeof(DialogType)).ToList();

            if (t.vdialog != null)
                objField.value = t.dialogType.ToString();

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.vdialog != null)
                    {
                        foreach (var vals in Enum.GetValues(typeof(DialogType)))
                        {
                            var astype = (DialogType)vals;

                            if (astype.ToString() == x.newValue)
                                t.dialogType = astype;
                        }
                    }
                });
            }
            return rootBox;
        }
        private VisualElement DrawEnableWritingIndicator(VDialogProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Enable write indic : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.vdialog != null)
                objField.value = t.enableWritingIndicator;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.vdialog != null)
                    {
                        t.enableWritingIndicator = objField.value;
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawSetAsDefault(VDialogProperty t)
        {
            var rootBox = VUITemplate.GetTemplate("Set as default : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);

            if (t.vdialog != null)
                objField.value = t.setThisAsdefaultDialog;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    if (t.vdialog != null)
                    {
                        t.setThisAsdefaultDialog = objField.value;
                    }
                });
            }

            return rootBox;
        }
    }
}