using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using VTasks;

namespace VelvieR
{
    [VTag("GameObject/InstantiateArea", "Instantiates gameObject/prefab inside a bound area of a 3D cube object on runtime.\n\n<b>IMPORTANT:</b> The object MUST be 3D cube!.", VColor.Blue, "Ia")]
    public class InstantiateArea : VBlockCore
    {
        [SerializeField, HideInInspector] public GameObject targetObject;
        [SerializeField, HideInInspector] public GameObject cubeObject;
        [SerializeField, HideInInspector] public int amount = 1;
        [SerializeField, HideInInspector] public string customName = string.Empty;
        [SerializeField, HideInInspector] public bool prefixes = false;
        [SerializeField, HideInInspector] public bool addToGameObjectManager = true;
        [SerializeField, HideInInspector] public bool randomRotation = false;
        private VTokenSource cts;

        void OnDisable()
        {
            if(cts != null && !cts.wasDisposed)
            {
                VTokenManager.CancelVToken(cts, true);
            }
        }
        public override void OnVEnter()
        {
            if (targetObject != null && amount > 0)
            {
                cts = new VTokenSource();
                InstantiateGo();
            }
        }
        private async void InstantiateGo()
        {
            var vts = cts.Token;
            VTokenManager.PoolVToken(cts);

            for (int i = 0; i < amount; i++)
            {
                // Random position within this transform
                
                Vector3 rndPosWithin;
                rndPosWithin = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                rndPosWithin = cubeObject.transform.TransformPoint(rndPosWithin * .5f);

                Quaternion rand = Quaternion.identity;

                if (randomRotation)
                {
                    rand = UnityEngine.Random.rotation;
                }

                var go = Instantiate(targetObject, rndPosWithin, rand);

                if (!String.IsNullOrEmpty(customName))
                {
                    go.name = customName;
                }

                if (prefixes)
                {
                    go.name += " " + i;
                }

                if (addToGameObjectManager)
                {
                    GameObjectManager.AddToGameObjectManager((go, go.name, (int)UnityEngine.Random.Range(int.MinValue, int.MaxValue), string.Empty));
                }

                if(cts.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(1, vts);
            }

            VTokenManager.CancelVToken(cts, true);
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if (targetObject == null)
            {
                summary += "Target object can't be empty!";
            }
            if (cubeObject == null)
            {
                summary += "Cube object can't be empty! And make sure it's a cube!";
            }
            if (amount < 1)
            {
                summary += "| Amount can't be less than 1!";
            }

            return summary;
        }
    }
}