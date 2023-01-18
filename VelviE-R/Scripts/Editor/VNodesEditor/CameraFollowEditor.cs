using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using VelvieR;
using System.Linq;

namespace VIEditor
{
    [CustomEditor(typeof(CameraFollow))]
    public class CameraFollowEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            var t = target as CameraFollow;
            root.Add(DrawEnable(t));
            root.Add(DrawTargetObject(t));
            root.Add(DrawObjectFollowing(t));
            root.Add(DrawVector(t));
            root.Add(DrawSmoot(t));
            root.Add(DrawFollowType(t));
            root.Add(DrawLeanProp(t));
            root.Add(DrawLookAt(t));
            

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }

        private VisualElement DrawTargetObject(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("Target to follow : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Transform);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.targetToFollow as Transform;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.targetToFollow = objField.value as Transform;
            });

            return rootBox;
        }
        private VisualElement DrawObjectFollowing(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("Camera : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new ObjectField();
            objField.objectType = typeof(Camera);
            objField.allowSceneObjects = true;
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.cam;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.cam = objField.value as Camera;
            });

            return rootBox;
        }
        private VisualElement DrawFollowType(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("Type : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.followType.ToString();
            objField.choices = Enum.GetNames(typeof(FollowTypes)).ToList();
            objField.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    foreach(var asEnum in Enum.GetValues(typeof(FollowTypes)))
                    {
                        var asetype = (FollowTypes)asEnum;

                        if(x.newValue == asetype.ToString())
                        {
                            t.followType = asetype;
                        }
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawLeanProp(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("Mode : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.followProperty.ToString();
            objField.choices = Enum.GetNames(typeof(LeanProp)).ToList();
            objField.RegisterCallback<ChangeEvent<string>>((x)=>
            {
                if(!PortsUtils.PlayMode)
                {
                    foreach(var asEnum in Enum.GetValues(typeof(LeanProp)))
                    {
                        var asetype = (LeanProp)asEnum;

                        if(x.newValue == asetype.ToString())
                        {
                            t.followProperty = asetype;
                        }
                    }
                }
            });

            return rootBox;
        }
        private VisualElement DrawLookAt(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("LookAt : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.lookAtTarget;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.lookAtTarget = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawEnable(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("Enable : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Toggle();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.enable;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.enable = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawVector(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("Offset : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new Vector3Field();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.offset;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.offset = objField.value;
            });

            return rootBox;
        }
        private VisualElement DrawSmoot(CameraFollow t)
        {
            var rootBox = VUITemplate.GetTemplate("Smooth : ");
            var field = VUITemplate.GetField(rootBox);

            var objField = new FloatField();
            objField.style.width = field.style.width;
            field.Add(objField);
            objField.value = t.smooth;

            objField.RegisterValueChangedCallback((x)=>
            {
                t.smooth = objField.value;
            });

            return rootBox;
        }
    }
}