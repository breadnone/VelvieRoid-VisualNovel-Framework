using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    public class VStageUtil : MonoBehaviour
    {
        [SerializeField] public List<VStageClass> TwoDStage = new List<VStageClass>();
        [SerializeField] public List<VStageClass> ThreeDStage = new List<VStageClass>();
        [SerializeField, HideInInspector] public string vstageId = string.Empty;
        [SerializeField, HideInInspector] public string vstageName = string.Empty;
        [SerializeField, HideInInspector] public GameObject twodobj;
        [SerializeField, HideInInspector] public GameObject threedobj;
        [SerializeField] private bool dim = false;
        public bool Dim{get{return dim;} set{dim = value;}}
        
        void OnValidate()
        {
            /* Reset in editor tests!
            twodobj = null;
            threedobj = null;
            TwoDStage.Clear();
            ThreeDStage.Clear();
*/
            if(String.IsNullOrEmpty(vstageId))
            {
                vstageId = Guid.NewGuid().ToString() + (int)UnityEngine.Random.Range(1985, int.MaxValue);
            }
            if(String.IsNullOrEmpty(vstageName) || vstageName != gameObject.name)
            {
                vstageName = gameObject.name;
            }

            if(twodobj == null)
            {
                var objs = transform.Find("2DStage");
                twodobj = objs.gameObject;

                if(objs != null)
                {
                    var child = objs.GetComponentsInChildren<RectTransform>();

                    if(child != null && child.Length > 0)
                    {
                        foreach(var stage in child)
                        {
                            if(stage.name == "2DStage")
                                continue;

                            var nuClass = new VStageClass
                            {
                                name = stage.name,
                                stageObject = stage.gameObject,
                                stageDefaultPos = stage.position,
                                stageDefaultScale = stage.localScale,
                                stageHashId = Guid.NewGuid().ToString(),
                                rect = stage,
                                stageTransform = stage
                            };

                            if(!TwoDStage.Exists(x => x.name == stage.name))
                            {
                                TwoDStage.Add(nuClass);
                            }
                        }
                    }
                }
            }
            if(threedobj == null)
            {
                var objs = transform.Find("3DStage");
                threedobj = objs.gameObject;

                var child = objs.GetComponentsInChildren<Transform>();

                if(child != null && child.Length > 0)
                {
                    foreach(var stage in child)
                    {
                        if(stage.name == "3DStage")
                            continue;

                        var nuClass = new VStageClass
                        {
                            name = stage.name,
                            stageObject = stage.gameObject,
                            stageDefaultPos = stage.position,
                            stageDefaultScale = stage.localScale,
                            stageHashId = Guid.NewGuid().ToString(),
                            stageTransform = stage
                        };

                        if(!ThreeDStage.Exists(x => x.name == stage.name))
                        {
                            ThreeDStage.Add(nuClass);
                        }
                    }
                }
            }
        }
        void Awake()
        {
            VStageManager.InsertRootVStage(this);
        }
        void Start()
        {

        }
        void OnEnable()
        {

        }
        void OnDisable()
        {

        }
    }
}