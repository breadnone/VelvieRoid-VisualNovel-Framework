using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//TODO : Refactor ALL GameObject vblocks to account for this behavior! Notably, to be able to pull out the data from GameObjectManager!

namespace VelvieR
{
    [System.Serializable]
    public enum GameObjectProperty
    {
        SetPosition,
        SetScale,
        SetRotation,
        Destroy,
        None
    }
    [VTag("GameObject/Find", "Finds single or multiple gameObjects in the scene then pooled into GameObjectManager which later can be accessed by other VBlocks.", VColor.Blue, "Fi")]
    public class Find : VBlockCore
    {
        [SerializeField, HideInInspector] public string objectName;
        [SerializeField, HideInInspector] public bool findViaTag;
        [SerializeField, HideInInspector] public Vector3 vec;
        [SerializeField, HideInInspector] public GameObjectProperty goProp = GameObjectProperty.None;
        [SerializeField, HideInInspector] public bool isLocal = false;
        [SerializeField, HideInInspector] public bool animate = false;
        [SerializeField, HideInInspector] public float duration;
        [SerializeField, HideInInspector] public float angle;
        [SerializeField, HideInInspector] public Transform target;
        [SerializeField, HideInInspector] public RotateType rotateType = RotateType.Vector3Left;
        public override void OnVEnter()
        {
            if (!String.IsNullOrEmpty(objectName) && goProp != GameObjectProperty.None)
            {
                if (!findViaTag)
                {
                    var objs = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == objectName).ToList();

                    if (objs.Count > 0)
                    {
                        foreach (var obj in objs)
                        {
                            if (obj == null)
                                continue;

                            NextProcess(obj);
                        }
                    }
                }
                else
                {
                    var objs = GameObject.FindGameObjectsWithTag(objectName);

                    if (objs.Length > 0)
                    {
                        foreach (var obj in objs)
                        {
                            if (obj == null)
                                continue;

                            NextProcess(obj);
                        }
                    }
                }
            }
        }
        private void NextProcess(GameObject obj)
        {
            if(goProp == GameObjectProperty.Destroy)
            {
                Destroy(obj);
            }
            else if(goProp == GameObjectProperty.SetPosition)
            {
                if(!isLocal)
                {
                    if(!animate)
                    {
                        obj.transform.position = vec;
                    }
                    else
                    {
                        if(target == null)
                        LeanTween.move(obj, vec, duration);
                        else
                        LeanTween.move(obj, target, duration);
                    }
                }
                else
                {
                    if(!animate)
                    {
                        obj.transform.localPosition = vec;
                    }
                    else
                    {
                        if(target == null)
                        LeanTween.moveLocal(obj, vec, duration);
                        else
                        LeanTween.moveLocal(obj, target.transform.localPosition, duration);
                    }   
                }
            }
            else if(goProp == GameObjectProperty.SetRotation)
            {
                if(!animate)
                {
                    if(!isLocal)
                    obj.transform.eulerAngles = vec;
                    else
                    obj.transform.localEulerAngles = vec;
                }
                else
                {
                    Vector3 rotVal = Vector3.zero;

                    if(rotateType == RotateType.Vector3Left)
                    {
                        rotVal = Vector3.left;
                    }
                    else if(rotateType == RotateType.Vector3Right)
                    {
                        rotVal = Vector3.right;
                    }
                    else if(rotateType == RotateType.Vector3Forward)
                    {
                        rotVal = Vector3.forward;
                    }
                    else if(rotateType == RotateType.Vector3Back)
                    {
                        rotVal = Vector3.back;
                    }
                    else if(rotateType == RotateType.Vector3Up)
                    {
                        rotVal = Vector3.up;
                    }
                    else if(rotateType == RotateType.Vector3Down)
                    {
                        rotVal = Vector3.down;
                    }

                    if(!isLocal)
                    {
                        LeanTween.rotateAround(obj, rotVal, angle, duration);
                    }
                    else
                    {
                        LeanTween.rotateAroundLocal(obj, rotVal, angle, duration);
                    }
                }
            }
            else if(goProp == GameObjectProperty.SetScale)
            {
                if(!animate)
                {
                    obj.transform.localScale = vec;
                }
                else
                {
                    if(target == null)
                    LeanTween.scale(obj, vec, duration);
                    else
                    LeanTween.scale(obj, target.localScale, duration);
                }
            }
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (String.IsNullOrEmpty(objectName))
            {
                if (!findViaTag)
                    summary += "Object name can't be empty!";
                else
                    summary += "Object tag can't be empty!";
            }

            return summary;
        }
    }
}