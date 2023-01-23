using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System;

namespace VIEditor
{
    public class VEditorFunc
    {
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

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AddTag(string txt)
        {
            var arTags = InternalEditorUtility.tags;

            if(arTags == null || arTags.Length == 0)
                return;

            if(!Array.Exists(arTags, x => x == txt))
                InternalEditorUtility.AddTag(txt);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
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