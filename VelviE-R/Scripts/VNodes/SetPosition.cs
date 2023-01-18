using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VelvieR
{
    [System.Serializable]
    public class GameObjectClass
    {
        public GameObject gameobject;
        public Vector3 position;
        public Vector3 euler;
        public Vector3 scale;
        public Transform target;
    }
    
    [VTag("GameObject/SetPosition", "Sets position of a gameObject in the scene.", VColor.Green, "Sa")]
    public class SetPosition : VBlockCore
    {
        [SerializeField] public List<GameObjectClass> go = new List<GameObjectClass>();
        [SerializeField] public bool isLocal = false;
        
        public override void OnVEnter()
        {
            if(go.Count > 0)
            {
                for(int i = 0; i < go.Count; i++)
                {
                    if(go[i] != null)
                    {
                        if(!isLocal)
                        {
                            if(go[i].target == null)
                            go[i].gameobject.transform.position = go[i].position;
                            else
                            go[i].gameobject.transform.position = go[i].target.transform.position;
                        }
                        else
                        {
                            if(go[i].target == null)
                            go[i].gameobject.transform.localPosition = go[i].position;
                            else
                            go[i].gameobject.transform.localPosition = go[i].target.transform.localPosition;
                        }
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

            if(go.Count == 0)
            {
                summary += "Target object can't be empty!";
            }

            return summary;
        }
    }
}