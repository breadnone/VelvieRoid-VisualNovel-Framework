using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace VelvieR
{
    [VTag("Flow/LoadScene", "Loads a new scene available from build-settings.\n\nMake sure scenes are added in build-settings ", VColor.White, "Ls")]
    public class LoadScene : VBlockCore
    {
        [SerializeField] public List<string> scenes = new List<string>();
        [SerializeField] public LoadSceneMode sceneMode = LoadSceneMode.Single;
        [SerializeField] public string load = string.Empty;
        public override void OnVEnter()
        {
            SceneManager.LoadScene(load, sceneMode);
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(String.IsNullOrEmpty(load))
            {
                summary += "No selected scene!.";
            }
            return summary;
        }
    }
}