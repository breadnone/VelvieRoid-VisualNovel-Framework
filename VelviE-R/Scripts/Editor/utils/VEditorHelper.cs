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
    
        public static void AssignAndAddTag(GameObject obj, string tag = "VelvieRoid")
        {
            if(obj == null)
                return;
            CreateTag(tag);
            obj.tag = tag;
        }
        public static void CreateTag(string txt)
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