using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    [VTag("GameObject/Instantiate", "Instantiates gameObject/prefab to the scene on runtime.", VColor.Blue, "Is")]
    public class Instantiate : VBlockCore
    {
        [SerializeField, HideInInspector] public GameObject targetObject;
        [SerializeField, HideInInspector] public Vector3 position;
        [SerializeField, HideInInspector] public Transform target;
        [SerializeField, HideInInspector] public int amount = 1;
        [SerializeField, HideInInspector] public string customName = string.Empty;
        [SerializeField, HideInInspector] public bool prefixes = false;
        [SerializeField, HideInInspector] public bool addToGameObjectManager = true;

        public override void OnVEnter()
        {
            if(targetObject != null && amount > 0)
            {
                InstantiateGo();
            }
        }
        private void InstantiateGo()
        {
            for(int i = 0; i < amount; i++)
            {
                if(target == null)
                {
                    var go = Instantiate(targetObject, position, Quaternion.identity);

                    if(!String.IsNullOrEmpty(customName))
                    {
                        go.name = customName;
                    }

                    if(prefixes)
                    {
                        go.name += " " + i;
                    }

                    if(addToGameObjectManager)
                    {
                        GameObjectManager.AddToGameObjectManager((go, go.name, (int)UnityEngine.Random.Range(int.MinValue, int.MaxValue), string.Empty));
                    }
                }
                else
                {
                    var go = Instantiate(targetObject, target.position, target.rotation);

                    if(!String.IsNullOrEmpty(customName))
                    {
                        go.name = customName;
                    }

                    if(prefixes)
                    {
                        go.name += " " + i;
                    }

                    if(addToGameObjectManager)
                    {
                        GameObjectManager.AddToGameObjectManager((go, go.name, (int)UnityEngine.Random.Range(int.MinValue, int.MaxValue), string.Empty));
                    }
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

            if(targetObject == null)
            {
                summary += "Target object can't be empty!";
            }

            if(amount < 1)
            {
                summary += "| Amount can't be less than 1!";
            }

            return summary;
        }
    }
}