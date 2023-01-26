using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace VIEditor
{
    [CustomEditor(typeof(VBlendShape))]
    public class VRAnimExpressionEditor : Editor
    {
        private VisualElement dummy;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as VBlendShape;
            dummy = new VisualElement();

            root.Add(DrawMesh(t));
            root.Add(dummy);
            dummy.Add(DrawList(t));
            root.Add(DrawCancel(t));
            root.Add(DrawForce(t));
            root.Add(DrawWait(t));

            if (t.vmesh == null)
            {
                dummy.SetEnabled(false);
            }
            else
            {
                dummy.SetEnabled(true);
            }

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private VisualElement DrawMesh(VBlendShape t)
        {
            var rootBox = VUITemplate.GetTemplate("SkinnedMesh : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new ObjectField();
            objField.objectType = typeof(SkinnedMeshRenderer);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.vmesh;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.vmesh = objField.value as SkinnedMeshRenderer;

                    if (t.vmesh == null)
                    {
                        dummy.SetEnabled(false);
                    }
                    else
                    {
                        dummy.SetEnabled(true);
                    }
                });
            }

            return rootBox;
        }
        private VisualElement DrawCancel(VBlendShape t)
        {
            var rootBox = VUITemplate.GetTemplate("Cancel previous : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.cancelPrevious;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.cancelPrevious = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawWait(VBlendShape t)
        {
            var rootBox = VUITemplate.GetTemplate("WaitUntilFinished : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.waitUntilFinished;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.waitUntilFinished = objField.value;
                });
            }

            return rootBox;
        }
        private VisualElement DrawForce(VBlendShape t)
        {
            var rootBox = VUITemplate.GetTemplate("Force complete : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.forceCompletePrevious;

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.forceCompletePrevious = objField.value;
                });
            }

            return rootBox;
        }
        private List<DropdownField> drops = new List<DropdownField>();
        private VisualElement DrawList(VBlendShape t)
        {
            Func<VisualElement> makeItem = () =>
            {
                var nu = new VisualElement();
                nu.style.flexDirection = FlexDirection.Column;

                var rootOne = new VisualElement();
                rootOne.style.flexDirection = FlexDirection.Row;
                var lbl = new Label();
                lbl.style.width = 100;
                lbl.text = "Face : ";
                var dropd = new DropdownField();
                drops.Add(dropd);
                dropd.style.width = 110;
                rootOne.Add(lbl);
                rootOne.Add(dropd);

                var list = new List<string>();

                for (int i = 0; i < 1000; i++)
                {
                    if (i < t.vmesh.sharedMesh.blendShapeCount)
                    {
                        string getVal = t.vmesh.sharedMesh.GetBlendShapeName(i);

                        if (String.IsNullOrEmpty(getVal))
                        {
                            continue;
                        }

                        list.Add(getVal);
                    }
                    else
                    {
                        break;
                    }
                }

                list.Add("<None>");
                dropd.choices = list;

                var rootThree = new VisualElement();
                rootThree.style.marginTop = 5;
                rootThree.style.flexDirection = FlexDirection.Row;
                var lblThree = new Label();
                lblThree.style.width = 100;
                lblThree.text = "Power : ";
                var val = new Slider();
                val.lowValue = 0;
                val.highValue = 100;
                val.style.width = 80;
                rootThree.Add(lblThree);
                rootThree.Add(val);

                var rootFour = new VisualElement();
                rootFour.style.marginTop = 5;
                rootFour.style.flexDirection = FlexDirection.Row;
                var lblFour = new Label();
                lblFour.style.width = 100;
                lblFour.text = "ResetPrevValue : ";
                var tog = new Toggle();
                rootFour.Add(lblFour);
                rootFour.Add(tog);

                var floats = new FloatField();
                floats.style.width = 30;
                rootThree.Add(floats);

                var rootFive = new VisualElement();
                rootFive.style.marginTop = 5;
                rootFive.style.flexDirection = FlexDirection.Row;
                var lblFive = new Label();
                lblFive.style.width = 100;
                lblFive.text = "Speed : ";
                var flo = new FloatField();
                flo.style.width = 110;
                rootFive.Add(lblFive);
                rootFive.Add(flo);

                var rootSix = new VisualElement();
                rootSix.style.marginTop = 5;
                rootSix.style.flexDirection = FlexDirection.Row;
                var lblSix = new Label();
                lblSix.style.width = 100;
                lblSix.text = "Min/Max : ";
                var floSix = new FloatField();
                floSix.style.width = 55;
                var floSixTwo = new FloatField();
                floSixTwo.style.width = 55;
                rootSix.Add(lblSix);
                rootSix.Add(floSix);
                rootSix.Add(floSixTwo);

                nu.userData = (val, tog, dropd, floats, flo, floSix, floSixTwo);
                nu.Add(rootOne);
                nu.Add(rootThree);
                nu.Add(rootFive);
                nu.Add(rootFour);
                nu.Add(rootSix);
                return nu;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                if (t.values[i] == null)
                {
                    t.values[i] = new VBlendShapes();
                }

                (Slider, Toggle, DropdownField, FloatField, FloatField, FloatField, FloatField) astype = ((Slider, Toggle, DropdownField, FloatField, FloatField, FloatField, FloatField))e.userData;

                astype.Item4.value = t.values[i].blendValue;
                astype.Item1.value = t.values[i].blendValue;
                astype.Item2.value = t.values[i].zeroedPrevValue;
                astype.Item3.value = t.values[i].blendName;
                astype.Item5.value = t.values[i].duration;
                astype.Item6.value = t.values[i].min;
                astype.Item7.value = t.values[i].max;
                astype.Item1.highValue = t.values[i].max;
                astype.Item1.lowValue = t.values[i].min;

                if (!PortsUtils.PlayMode)
                {
                    astype.Item6.RegisterCallback<ChangeEvent<float>>((x) =>
                    {
                        var idx = i;
                        t.values[idx].min = x.newValue;
                        astype.Item1.lowValue = x.newValue;

                        if (t.values[idx].blendValue < astype.Item1.lowValue)
                        {
                            t.values[idx].blendValue = astype.Item1.lowValue;
                            astype.Item1.value = astype.Item1.lowValue;
                        }
                    });
                    astype.Item7.RegisterCallback<ChangeEvent<float>>((x) =>
                    {
                        var idx = i;
                        t.values[idx].max = x.newValue;
                        astype.Item1.highValue = x.newValue;

                        if (t.values[idx].blendValue > astype.Item1.highValue)
                        {
                            t.values[idx].blendValue = astype.Item1.highValue;
                            astype.Item1.value = astype.Item1.highValue;
                        }
                    });
                }

                t.values[i].blendShapeindex = i;

                if (String.IsNullOrEmpty(t.values[i].blendName))
                {
                    astype.Item1.SetEnabled(false);
                }
                else
                {
                    astype.Item1.SetEnabled(true);
                }

                if (String.IsNullOrEmpty(t.values[i].blendName))
                {
                    astype.Item3.value = "<None>";
                }

                if (!PortsUtils.PlayMode)
                {
                    astype.Item1.RegisterCallback<ChangeEvent<float>>((x) =>
                    {
                        var idx = i;
                        t.values[idx].blendValue = x.newValue;
                        astype.Item4.value = x.newValue;
                    });
                    astype.Item5.RegisterCallback<ChangeEvent<float>>((x) =>
                    {
                        var idx = i;
                        t.values[idx].duration = x.newValue;
                    });
                    astype.Item3.RegisterCallback<ChangeEvent<string>>((x) =>
                    {
                        var idx = i;
                        t.values[idx].blendName = x.newValue;

                        if (String.IsNullOrEmpty(t.values[idx].blendName) || x.newValue == "<None>")
                        {
                            astype.Item1.SetEnabled(false);
                            t.values[idx].blendName = string.Empty;
                        }
                        else
                        {
                            astype.Item1.SetEnabled(true);
                        }

                        EditorUtility.SetDirty(t.gameObject);
                    });

                    astype.Item2.RegisterValueChangedCallback((x) =>
                    {
                        var idx = i;
                        t.values[idx].zeroedPrevValue = x.newValue;
                    });
                }
            };

            const int itemHeight = 100;
            var rootBox = VUITemplate.GetTemplate("Value : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ListView(t.values, itemHeight, makeItem, bindItem);
            objField.reorderable = false;
            objField.showBorder = true;
            objField.showAddRemoveFooter = true;
            objField.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            field.Add(objField);
            return rootBox;
        }
    }
}