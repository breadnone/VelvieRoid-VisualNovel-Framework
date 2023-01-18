using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using VelvieR;
using UnityEngine.SceneManagement;

public static class VInit
{
    private static VInputBuffer vinput;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeMethodLoad()
    {
        if (VBlockManager.UnityContext == null)
            VBlockManager.UnityContext = TaskScheduler.FromCurrentSynchronizationContext();

        vinput = (VInputBuffer)UnityEngine.Object.FindObjectOfType<VInputBuffer>();

        if (vinput != null)
        {
            if (VBlockManager.ActiveInput == null)
            {
                VBlockManager.ActiveInput = vinput;
                vinput.InitPooling();
                vinput.SubscribeVEvents(true);
            }
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogWarning("VWarning: No VGraph instances in the scene!");
            #endif
        }

        var t = Resources.FindObjectsOfTypeAll<VStageUtil>();

        if (t.Length > 0)
        {
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].TwoDStage.Count == 0)
                    continue;

                foreach (var stage in t[i].TwoDStage)
                {
                    if (stage == null)
                        continue;

                    if (VCharacterManager.dummyStage == null)
                    {
                        VCharacterManager.dummyStage = stage;
                    }

                    stage.stageIs2d = true;

                    if (!VCharacterManager.AllStages.Contains(stage))
                        VCharacterManager.AllStages.Add(stage);
                }

                if (t[i].ThreeDStage.Count == 0)
                    continue;

                foreach (var stage in t[i].ThreeDStage)
                {
                    if (stage == null)
                        continue;

                    stage.stageIs2d = false;

                    if (!VCharacterManager.AllStages.Contains(stage))
                        VCharacterManager.AllStages.Add(stage);
                }
            }
        }

        try
        {
            Variables vcontainer = (Variables)Resources.Load("VelvieData/VDAT-sr-nh");

            if (vcontainer.ivar.Count > 0)
            {
                VBlockManager.variables = new Dictionary<int, IVar>(vcontainer.ivar.Count);

                for (int i = 0; i < vcontainer.ivar.Count; i++)
                {
                    if (vcontainer.ivar[i] == null)
                        continue;

                    if (!VBlockManager.variables.ContainsKey(vcontainer.ivar[i].VarId))
                        VBlockManager.variables.Add(vcontainer.ivar[i].VarId, vcontainer.ivar[i]);
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("VError: Variable container can't be found!");
        }
        /*
        SceneManager.activeSceneChanged += (x, y)=>
        {
            LocalVariablePool();
        };
        */
    }
}
