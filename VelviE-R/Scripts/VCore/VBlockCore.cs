using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VelvieR;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using VTasks;
using UnityEngine.Events;

public class VBlockCore : MonoBehaviour, IVNodes
{
    public static VListener<VBlockCoreEnterState> VBlockEnterListener = new VListener<VBlockCoreEnterState>();
    public int thisIndex { get; set; }
    public VelvieBlockComponent thisVblock { get; set; }
    public VBlockManager vmanager { get; set; }
    public bool VBlockIsPaused { get; set; } = false;

    private VTokenSource cts;
    void OnDisable()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
    private void RunningOperation(bool state)
    {
        if (state)
        {
            VBlockManager.runningVcoreBlock.Add(thisVblock);
        }
        else
        {
            VBlockManager.runningVcoreBlock.Remove(thisVblock);
        }
    }
    public virtual void ExecStartVBlock(int index)
    {
        thisIndex = index;

        if (VBlockManager.vblockState == VBlockState.Paused)
        {
            VBlockIsPaused = true;
            _ = FlowPaused(index);
        }
        else
        {
            StartVBlock(index);
        }
    }

    private async ValueTask FlowPaused(int index)
    {
        if (VBlockManager.vblockState == VBlockState.Paused)
        {
            cts = new VTokenSource();
            var pauseCtoke = cts.Token;
            VTokenManager.PoolVToken(cts);

            while (VBlockManager.vblockState == VBlockState.Paused)
            {
                if (pauseCtoke.IsCancellationRequested)
                {
                    break;
                }

                await VTask.VYield(pauseCtoke);
            }

            VTokenManager.CancelVToken(cts);
        }

        VBlockIsPaused = false;
        StartVBlock(index);
    }
    private void StartVBlock(int index)
    {
        VBlockEnterListener.Value = VBlockCoreEnterState.Enter;
        RunningOperation(true);

        #if UNITY_EDITOR
        if (VBlockManager.VgraphsOpenInstance)
            VBlockManager.vnodeTracking.Invoke(thisVblock.vnodeId, thisVblock.guid, index, true);
        #endif

        StartEnter().EndExit();
    }

    private void OnContinue(int index)
    {
        if (!stopFlag)
        {
            index = thisIndex;
        }
        else
        {
            if (stopFlagIndex.HasValue)
            {
                index = stopFlagIndex.Value;
            }

            var t = CheckFlags?.Invoke();

            if(t != null && !t.Value)
            {
                index = thisIndex;
                CheckFlags = null;
                stopFlag = false;
                stopFlagIndex = null;
            }
        }

        RunningOperation(false);

        #if UNITY_EDITOR
        if (VBlockManager.VgraphsOpenInstance && thisVblock != null)
            VBlockManager.vnodeTracking.Invoke(thisVblock.vnodeId, thisVblock.guid, index, false);
        #endif

        vmanager.ExecuteNext(index, true);
    }

    ///<summary> 
    ///The entry in VNodes
    ///</summary>
    public virtual void OnVEnter() { }

    ///<summary> 
    ///Exit point in VNodes
    ///</summary>
    // Second executed
    public virtual void OnVExit() { }

    ///<summary> 
    ///Inspector VBlock summary.
    ///</summary>
    #if UNITY_EDITOR
    public virtual string OnVSummary()
    {
        return string.Empty;
    }
    #endif

    public virtual void OnVContinue()
    {
        OnContinue(thisIndex);
    }
    public virtual void OnVSkipContinue(int indexToSkip)
    {
        OnContinue(thisIndex);
    }
    private VBlockCore StartEnter()
    {
        OnVEnter();
        return this;
    }
    private VBlockCore EndExit()
    {
        OnVExit();
        return this;
    }
    public void SetFlagIndex(bool boolFlagidx, int? stopflagidx, Func<bool> checkFlag)
    {
        CheckFlags = checkFlag;
        stopFlag = boolFlagidx;
        stopFlagIndex = stopflagidx;
    }
    private bool stopFlag = false;
    private int? stopFlagIndex;
    public Func<bool> CheckFlags;

}
