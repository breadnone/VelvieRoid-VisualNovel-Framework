using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VelvieR
{
    [VTag("GameObject/SetRotation", "Sets rotation of a gameObject in the scene via eulerAngles.", VColor.Green, "Ro")]
    public class SetRotation : VBlockCore
    {
        [SerializeField] public List<GameObjectClass> go = new List<GameObjectClass>();

        public override void OnVEnter()
        {
            if (go.Count > 0)
            {
                for (int i = 0; i < go.Count; i++)
                {
                    if (go[i] != null)
                    {
                        if(go[i].target == null)
                        go[i].gameobject.transform.eulerAngles = go[i].euler;
                        else
                        go[i].gameobject.transform.rotation = go[i].target.rotation;
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

            if (go.Count == 0)
            {
                summary += "Target object can't be empty!";
            }

            return summary;
        }
    }
}