using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System;
using VelvieR;

namespace VIEditor
{
    public class VEditorFunc
    {
        public static void SetUIDynamicSize(VisualElement element, int length, bool isWidth)
        {
            if(isWidth)
                element.style.width = new StyleLength(new Length(length, LengthUnit.Percent));
            else
                element.style.height = new StyleLength(new Length(length, LengthUnit.Percent));
        }
        public static bool VelvieTagIsExist()
        {
            var arTags = InternalEditorUtility.tags;

            if(arTags == null || arTags.Length == 0)
                return false;

            return Array.Exists(arTags, x => x == "VelvieRoid");
        }
        public static VelvieDialogue[] EditorGetVDialogues()
        {
            var t = GameObject.FindGameObjectsWithTag("VelvieRoid");

            if(t == null || t.Length == 0)
                return null;

            List<VelvieDialogue> dials = new List<VelvieDialogue>();

            for(int i = 0; i < t.Length; i++)
            {
                var com = t[i].GetComponent<VelvieDialogue>();

                if(com != null)
                {
                    dials.Add(com);
                }
            }
            
            return dials.ToArray();
        }
        public static VCoreUtil[] EditorGetVCoreUtils()
        {
            var t = GameObject.FindGameObjectsWithTag("VelvieRoid");

            if(t == null || t.Length == 0)
                return null;

            List<VCoreUtil> dials = new List<VCoreUtil>();

            for(int i = 0; i < t.Length; i++)
            {
                var com = t[i].GetComponent<VCoreUtil>();

                if(com != null)
                {
                    dials.Add(com);
                }
            }
            
            return dials.ToArray();
        }
        public static VStageUtil[] EditorGetVStageUtils()
        {
            var t = GameObject.FindGameObjectsWithTag("VelvieRoid");

            if(t == null || t.Length == 0)
                return null;

            List<VStageUtil> dials = new List<VStageUtil>();

            for(int i = 0; i < t.Length; i++)
            {
                var com = t[i].GetComponent<VStageUtil>();

                if(com != null)
                {
                    dials.Add(com);
                }
            }
            
            return dials.ToArray();
        }
        public static VCharacterUtil[] EditorGetVCharacterUtils()
        {
            var t = GameObject.FindGameObjectsWithTag("VelvieRoid");

            if(t == null || t.Length == 0)
                return null;

            List<VCharacterUtil> dials = new List<VCharacterUtil>();

            for(int i = 0; i < t.Length; i++)
            {
                var com = t[i].GetComponent<VCharacterUtil>();

                if(com != null)
                {
                    dials.Add(com);
                }
            }
            
            return dials.ToArray();
        }
        public static VMenuOption[] EditorGetVMenuUtils()
        {
            var t = GameObject.FindGameObjectsWithTag("VelvieRoid");

            if(t == null || t.Length == 0)
                return null;

            List<VMenuOption> dials = new List<VMenuOption>();

            for(int i = 0; i < t.Length; i++)
            {
                var com = t[i].GetComponent<VMenuOption>();

                if(com != null)
                {
                    dials.Add(com);
                }
            }
            
            return dials.ToArray();
        }
        public static VInputBuffer[] EditorGetVInput()
        {
            var t = EditorGetVCoreUtils();

            if(t == null || t.Length == 0)
                return null;

            List<VInputBuffer> dials = new List<VInputBuffer>();

            for(int i = 0; i < t.Length; i++)
            {
                var com = t[i].gameObject.GetComponent<VInputBuffer>();

                if(com != null)
                {
                    dials.Add(com);
                }
            }
            
            return dials.ToArray();
        }
        public static VStageComponent[] EditorGetVStageComponent()
        {
            var t = EditorGetVCharacterUtils();

            if(t == null || t.Length == 0)
                return null;

            List<VStageComponent> dials = new List<VStageComponent>();

            for(int i = 0; i < t.Length; i++)
            {
                var com = t[i].gameObject.GetComponent<VCharacterUtil>() as VStageComponent;

                if(com != null)
                {
                    dials.Add(com);
                }
            }
            
            return dials.ToArray();
        }
        public static void AssignAndAddTag(GameObject obj, string tag = "VelvieRoid")
        {
            if(obj == null)
                return;
            CreateTag(tag);
            obj.tag = tag;
        }
        public static void CreateTag(string txt = "VelvieRoid")
        {
            if(String.IsNullOrEmpty(txt))
                return;

            AddTag(txt);
        }
        public static void RemoveTag(string txt)
        {
            if(String.IsNullOrEmpty(txt))
                return;
            
            DelTag(txt);
        }

        private static void AddTag(string txt)
        {
            var arTags = InternalEditorUtility.tags;

            if(arTags == null || arTags.Length == 0)
                return;

            if(!Array.Exists(arTags, x => x == txt))
                InternalEditorUtility.AddTag(txt);
        }

        private static void DelTag(string txt)
        {
            var arTags = InternalEditorUtility.tags;

            if(arTags == null || arTags.Length == 0)
                return;

            if(Array.Exists(arTags, x => x == txt))
                InternalEditorUtility.RemoveTag(txt);
        }
    }
}