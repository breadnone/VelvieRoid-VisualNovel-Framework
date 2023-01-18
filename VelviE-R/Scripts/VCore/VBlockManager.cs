using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace VelvieR
{
    public class VBlockManager
    {
        public static VInputBuffer ActiveInput {get;set;}
        public static TaskScheduler UnityContext {get;set;}

        #if UNITY_EDITOR
        public static UnityAction<string, string, int, bool> vnodeTracking;
        public static bool VgraphsOpenInstance { get; set; }
        public static UnityAction<int> ReSerialize;
        #endif
        public static List<AnimationState> listOfAnimatingEntities = new List<AnimationState>();
        public static List<VCoreUtil> vcoreutils = new List<VCoreUtil>();
        public static List<VelvieBlockComponent> runningVcoreBlock = new List<VelvieBlockComponent>();
        public List<VelvieBlockComponent> currentRunning;
        //All velvieblocks/nodes attached to the gameObject in the hierarchy go here!
        public static VBlockState vblockState { get; set; }
        public bool ForcedCancelVBlock { get; set; }
        public static List<VelvieDialogue> ActiveDialogue = new List<VelvieDialogue>();
        public bool IsPaused{get;set;} = false;
        public static VelvieDialogue DefaultDialog{get;set;}
        public static float VInputDelays {get;set;} = 0.2f;
        public static Dictionary<int, IVar> variables;

        ///<summary>Execute VelvieBlockComponent </summary>
        public void StartExecutingVBlock(List<VelvieBlockComponent> vblock, bool singleExecution = false, VelvieBlockComponent vblockSingleExec = null)
        {
            VelvieBlockComponent gameStarted = null;
            int? index = null;

            currentRunning = new List<VelvieBlockComponent>(vblock);

            if(!singleExecution)
            {
                for (int i = 0; i < currentRunning.Count; i++)
                {
                    if (currentRunning[i] != null && currentRunning[i].isInGameStartedBlock && currentRunning[i].enable)
                    {
                        gameStarted = currentRunning[i];
                        index = i;
                        break;
                    }
                }

                if (gameStarted != null && index.HasValue)
                {
                    var vcomAsOriType = gameStarted.attachedComponent.component as VBlockCore;
                    vcomAsOriType.vmanager = this;
                    vcomAsOriType.thisVblock = currentRunning[index.Value];
                    vcomAsOriType.ExecStartVBlock(index.Value);
                }
            }
            else
            {
                if (vblockSingleExec != null)
                {
                    int? indexSingle = currentRunning.FindIndex(x => x == vblockSingleExec && x.enable);
                    
                    if(indexSingle.HasValue)
                    {
                        var vcomAsOriType = vblockSingleExec.attachedComponent.component as VBlockCore;
                        vcomAsOriType.vmanager = this;
                        vcomAsOriType.thisVblock = currentRunning[indexSingle.Value];
                        vcomAsOriType.ExecStartVBlock(indexSingle.Value);
                    }
                }
            }
        }

        ///<summary>Execute next VelvieBlockComponent. Will jump to next if disabled  </summary>
        public void ExecuteNext(int index, in bool next)
        {
            var count = currentRunning.Count;
            var nextIndex = 0;

            if(next)
            {
                nextIndex = index + 1;
            }
            else
            {
                nextIndex = index - 1;
            }

            if (count == 0 || nextIndex > count || nextIndex == -1 || ForcedCancelVBlock)
            {
                #if UNITY_EDITOR
                if(nextIndex == -1)
                    Debug.LogError("Index Out Of Range, previous index resulted to -1");

                if(nextIndex > count)
                    Debug.LogError("Index Out Of Range, can't skip above " + count + "max index");
                #endif

                ForcedCancelVBlock = false;
                return;
            }

            if (nextIndex < currentRunning.Count)
            {
                if (!currentRunning[nextIndex].enable)
                {
                    ExecuteNext(nextIndex, next);
                    return;
                }

                var vcomAsOriType = currentRunning[nextIndex].attachedComponent.component as VBlockCore;
                vcomAsOriType.vmanager = this;
                vcomAsOriType.thisVblock = currentRunning[nextIndex];
                vcomAsOriType.ExecStartVBlock(nextIndex);
            }
        }

        ///<summary>Jump to another VelvieBlockComponent. Will cancel if not exist. Used by VCall.</summary>
        public void ExecuteCallVBlock(VelvieBlockComponent velvieCom)
        {
            int? index = null;

            for (int i = 0; i < currentRunning.Count; i++)
            {
                if (currentRunning[i] != null && currentRunning[i].enable && currentRunning[i] == velvieCom)
                {
                    index = 0;
                    break;
                }
            }

            if(!index.HasValue)
                return;

            var jumpTo = currentRunning[index.Value].attachedComponent.component as VBlockCore;

            jumpTo.vmanager = this;
            jumpTo.thisVblock = currentRunning[index.Value];
            jumpTo.ExecStartVBlock(index.Value);
        }
        public void ExecuteSkipToIndex(in int skipIndex)
        {
            var count = currentRunning.Count;

            if (count == 0 || skipIndex > count || skipIndex == -1 || ForcedCancelVBlock)
            {
                #if UNITY_EDITOR
                if(skipIndex == -1)
                    Debug.LogError("Index Out Of Range, can't skip below -1 index");

                if(skipIndex > count)
                    Debug.LogError("Index Out Of Range, can't skip above " + count);

                #endif

                ForcedCancelVBlock = false;
                return;
            }

            if (!currentRunning[skipIndex].enable)
            {
                ExecuteSkipToIndex(skipIndex + 1);
            }

            var vcomAsOriType = currentRunning[skipIndex].attachedComponent.component as VBlockCore;
            vcomAsOriType.vmanager = this;
            vcomAsOriType.thisVblock = currentRunning[skipIndex];
            vcomAsOriType.ExecStartVBlock(skipIndex);
        }
        public void CancelVDialogue(VelvieDialogue vdialogue, int index)
        {
            vdialogue.AbortVDialogueExecution = false;
            vdialogue.AbortExecution();
            vdialogue.HideVDialogue(null, string.Empty, true, vdialogue.waitingForClick);
            
            var nextIndex = index + 1;
            Reiterate();

            void Reiterate()
            {
                if (nextIndex < currentRunning.Count)
                {
                    if (!currentRunning[nextIndex].enable || currentRunning[nextIndex].attachedComponent.component is SayWord)
                    {
                        nextIndex++;
                        Reiterate();
                        return;
                    }
                    
                    var vcomAsOriType = currentRunning[nextIndex].attachedComponent.component as VBlockCore;
                    vcomAsOriType.vmanager = this;
                    vcomAsOriType.thisVblock = currentRunning[nextIndex];
                    vcomAsOriType.ExecStartVBlock(nextIndex);
                }                
            }
        }
        public (bool state, VelvieDialogue vdialogue) NextIsVDialogue(int index)
        {
            var nextIndex = index + 1;
            VelvieDialogue vdial = null;
            bool exists = false;

            Reiterate();

            void Reiterate()
            {
                if (nextIndex < currentRunning.Count)
                {
                    if (!currentRunning[nextIndex].enable)
                    {
                        nextIndex++;
                        Reiterate();
                        return;
                    }
                    
                    var vdialComponent = currentRunning[nextIndex].attachedComponent.component;

                    if(vdialComponent is SayWord vdialobj)
                    {
                        exists = true;
                        vdial = vdialobj.VDialogue;
                    }
                }                
            }

            if(!exists)
            {
                var vdialComponent = currentRunning[index].attachedComponent.component;

                if(vdialComponent is SayWord vdialobj && vdialobj != null)
                {
                    vdial = vdialobj.VDialogue;
                }
            }

            return (exists, vdial);
        }
    }
}