using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace VelvieR
{
    public class SceneRefUtil : MonoBehaviour
    {
        [SerializeField, HideInInspector] public List<VGameObject> gameObjects;
        [SerializeField, HideInInspector] public List<VTransform> transforms;
        [SerializeField, HideInInspector] public List<VTmp> tmps;
        [SerializeField, HideInInspector] public Variables variables;

        public GameObject GetGameObject(string vname, int vid)
        {
            return gameObjects.Find(x => x.Name == vname && x.VarId == vid).value;
        } 
        public Transform GetTransform(string vname, int vid)
        {
            return transforms.Find(x => x.Name == vname && x.VarId == vid).value;
        }
        public TMP_Text GetTMP_TEXT(string vname, int vid)
        {
            return tmps.Find(x => x.Name == vname && x.VarId == vid).value;
        }
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        public void ReValidate()
        {
            if(variables == null)
            {
                variables = (Variables)Resources.Load("VelvieData/VDAT-sr-nh");

                if(variables == null)
                {
                    throw new Exception("VError: no variable asset found!");
                }
            }

            if (variables != null && variables.ivar.Count > 0)
            {
                if(gameObjects.Count != 0)
                {
                    for (int i = gameObjects.Count; i --> 0; )
                    {
                        if(gameObjects[i] == null)
                            continue;

                        if(!variables.ivar.Exists(x => x.VarId == gameObjects[i].VarId && x.Name == gameObjects[i].Name))
                        {
                            gameObjects.RemoveAt(i);
                        }
                    }
                }

                if(transforms.Count != 0)
                {
                    for (int i = transforms.Count; i --> 0; )
                    {
                        if(transforms[i] == null)
                            continue;

                        if(!variables.ivar.Exists(x => x.VarId == transforms[i].VarId && x.Name == transforms[i].Name))
                        {
                            transforms.RemoveAt(i);
                        }
                    }
                }
                if(tmps.Count != 0)
                {
                    for (int i = tmps.Count; i --> 0; )
                    {
                        if(tmps[i] == null)
                            continue;

                        if(!variables.ivar.Exists(x => x.VarId == tmps[i].VarId && x.Name == tmps[i].Name))
                        {
                            tmps.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}