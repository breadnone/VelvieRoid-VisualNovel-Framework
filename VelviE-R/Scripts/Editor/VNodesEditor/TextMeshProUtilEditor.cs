using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VelvieR;
using System;
using TMPro;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(TextMeshProUtil))]
    public class TextMeshProUtilEditor : Editor
    {
        private VisualElement dummy;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as TextMeshProUtil;
            dummy = new VisualElement();
            dummy.style.flexDirection = FlexDirection.Column;

            root.Add(DrawTMP(t));
            root.Add(DrawTMPTypes(t));
            root.Add(dummy);

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, t.OnVSummary);
            return root;
        }
        private void ClearDummyChild()
        {
            if(dummy == null)
                return;

            foreach(var child in dummy.Children().ToList())
            {
                if(child != null)
                    child.RemoveFromHierarchy();
            }
        }
        private VisualElement DrawTMP(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("TextmeshPro : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(TMP_Text);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.text;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.text = objField.value as TMP_Text;
            });

            return rootBox;
        }
        private VisualElement DrawTMPTypes(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Select : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.tmpUtil.ToString();
            objField.choices = Enum.GetNames(typeof(TMpUtilv)).ToList();

            objField.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    foreach(var asEnum in Enum.GetValues(typeof(TMpUtilv)))
                    {
                        var asetype = (TMpUtilv)asEnum;

                        if(x.newValue == asetype.ToString())
                        {
                            t.tmpUtil = asetype;
                            ClearDummyChild();

                            if(asetype == TMpUtilv.SetAlignment)
                            {
                                DrawAlignment(t);
                            }
                            else if(asetype == TMpUtilv.SetFontSize)
                            {
                                DrawFontSize(t);
                            }
                            else if(asetype == TMpUtilv.SetColor)
                            {
                                DrawColor(t);
                            }
                            else if(asetype == TMpUtilv.EnableDisableRaycast)
                            {
                                DrawRaycast(t);
                            }
                            else if(asetype == TMpUtilv.SetFont)
                            {
                                DrawFont(t);
                            }
                        }
                    }
                }
            });

            return rootBox;
        }
        /*
        private void DrawTextToModify(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Word to modify : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new TextField();
            objField.tooltip = "Type text you want to modify, and make sure that richTag enabled.";
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.textToModify;

            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.textToModify = objField.value;
                }
            });

            dummy.Add(rootBox);
        }
        */
        private void DrawAlignment(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Set alignment : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.choices = Enum.GetNames(typeof(TextAlignmentOptions)).ToList();
            objField.value = t.align.ToString();

            objField.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    foreach(var tenum in Enum.GetValues(typeof(TextAlignmentOptions)))
                    {
                        var asttype = (TextAlignmentOptions)tenum;
                        if(x.newValue == asttype.ToString())
                        {
                            t.align = asttype;
                        }
                    }
                }
            });

            dummy.Add(rootBox);
        }
        private void DrawFontSize(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Set fontSize : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.fontSize;
            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.fontSize = objField.value;
                }
            });

            dummy.Add(rootBox);
        }
        private void DrawColor(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Set textColor : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ColorField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.textColor;
            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.textColor = objField.value;
                }
            });

            dummy.Add(rootBox);
        }
        private void DrawRaycast(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Enable raycast : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);

            if(t.text == null)
            {
                objField.value = t.raycast;
            }
            else
            {
                objField.value = t.text.raycastTarget;
                t.raycast = objField.value;
            }

            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.raycast = objField.value;
                }
            });

            dummy.Add(rootBox);
        }

        private void DrawFont(TextMeshProUtil t)
        {
            var rootBox = VUITemplate.GetTemplate("Set font : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(TMP_FontAsset);
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.font;
            objField.RegisterValueChangedCallback((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    t.font = objField.value as TMP_FontAsset;
                }
            });

            dummy.Add(rootBox);
        }
    }
}