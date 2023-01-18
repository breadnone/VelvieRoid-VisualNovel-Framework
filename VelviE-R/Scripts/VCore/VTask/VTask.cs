using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Linq;
//using System.Threading.Tasks.Sources;
using VelvieR;


namespace VTasks
{
    class VTasksListener<T>  
    {
        private T _value;

        public Action ValueChanged;

        public T Value
        {
            get => _value;

            set
            {
                if(!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    OnValueChanged();
                }
            }
        }

        protected virtual void OnValueChanged() => ValueChanged?.Invoke() ;
    }

    public static class VTask
    {
        ///This would trigger the method after Awake, without having to instantiate
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeMethodLoad()
        {
            SceneManager.activeSceneChanged += (x, y) => 
            { 
                CancelAllVTokens();
            };

            Application.quitting += ()=> 
            {
                foreach(var t in VBlockManager.ActiveDialogue)
                {
                    t.CancelVDialogTokens();
                }

                CancelAllVTokens();
            };            
        }
        
        private static HashSet<VTokenSource> VTokenSources = new HashSet<VTokenSource>();
        private static int instances = 0;
        ///<summary>
        ///Cancels all vtokens.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CancelAllVTokens(bool dispose = true)
        {
            if (VTokenSources.Count > 0)
            {
                var tokeAsList = VTokenSources.ToArray();

                for (int i = tokeAsList.Length; i-- > 0;)
                {
                    if (tokeAsList[i] == null || tokeAsList[i].wasDisposed)
                        continue;

                    try
                    {
                        tokeAsList[i].Cancel();
                    }
                    catch(OperationCanceledException)
                    {
                        continue;
                    }

                    if (dispose)
                    {
                        tokeAsList[i].Dispose();
                    }
                }

                VTokenSources.Clear();
            }
        }
        ///<summary>
        ///Cancels single vtoken.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CancelVToken(VTokenSource cts, bool dispose = false)
        {
            VTokenSource vts = null;
            var token = VTokenSources.TryGetValue(cts, out vts);

            if (vts != null)
            {
                try
                {
                    if(!vts.wasDisposed)
                    {
                        vts.Cancel();
                    }
                }
                catch(OperationCanceledException)
                {
                    //no need to throwException, tasks excp won't stacked
                }

                try
                {
                    if (dispose && !vts.wasDisposed)
                    {
                        vts.Dispose();
                        VTokenSources.Remove(vts);
                    }
                }
                catch(ObjectDisposedException)
                {
                    return;
                }
            }
        }
        ///<summary>
        ///Add to VTasks's internal pools for easy cancellation on scene changes or application quit. Guaranteed to be cancelled and disposed.
        ///</summary>
        public static void PoolVToken(VTokenSource cts)
        {
            VTokenSources.Add(cts);
        }
        public static (VTokenSource cts, CancellationToken vts) AskSourceAskToken()
        {
            var cts = new VTokenSource();
            var vts = cts.Token;
            vts.ThrowIfCancellationRequested();
            return (cts, vts);
        }
        ///<summary>
        ///Skip one frame. Equals to yield return null.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask VYield(CancellationToken cancellationToken, int frame = 0, bool immediateDispose = true)
        {
            //TODO: Make a custom awaitable for this!
            int prevFrame = Time.frameCount + frame;
            instances++;

            while(prevFrame == Time.frameCount)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    instances--;
                    break;
                }
                try
                {
                    await Task.Delay(1, cancellationToken: cancellationToken);
                }
                catch(Exception e)
                {
                    if(e is OperationCanceledException || e is ObjectDisposedException)
                    {
                        instances--;
                        break;
                    }

                    throw e;
                }
            }

            instances--;
        }
        ///<summary>
        ///Waits for frames.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<bool> VWaitFrames(int frames, CancellationToken cancellationToken, bool immediateDispose = true)
        {
            int prevFrame = Time.frameCount;
            int targetFrame = prevFrame + frames;
            
            instances++;

            while(prevFrame != targetFrame)
            {
                if(VTokenManager.VRootToken.vtokenSource.IsCancellationRequested)
                {
                    instances--;
                    return false;
                }
                try
                {
                    await Task.Delay(1, cancellationToken: cancellationToken);
                }
                catch(Exception e)
                {
                    if(e is OperationCanceledException || e is ObjectDisposedException)
                    {
                        instances--;
                        return false;
                    }

                    throw e;
                }
            }
            
            instances--;
            return true;
        }
        ///<summary>
        ///Waits in seconds.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<bool> VWaitSeconds(float time, CancellationToken cancellationToken, bool immediateDispose = true, Action exec = null)
        {
            instances++;

            if(exec != null)
            {
                using var registration = cancellationToken.Register(exec);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(time), cancellationToken: cancellationToken);
            }
            catch(Exception e)
            {
                if(e is OperationCanceledException || e is ObjectDisposedException)
                {
                    instances--;
                    return false;
                }
                else
                {
                    throw e;
                }                
            }

            instances--;
            return true;
        }
        ///<summary>
        ///Waits until return true. May not be next frame.
        ///</summary>
        public static async ValueTask<bool> VWaitUntil(Func<bool> tcondition, CancellationToken vts)
        {
            if (!tcondition.Invoke())
            {
                while (!tcondition.Invoke())
                {
                    if (vts.IsCancellationRequested)
                    {
                        return false;
                    }
                    try
                    {
                        await VTask.VYield(vts);
                    }
                    catch(OperationCanceledException)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        ///<summary>
        ///Simple hash algorithm.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int VHash(int hashCode)
        {
            unchecked
            {
                UnityEngine.Random.InitState(Time.frameCount + 1);
                int valA = UnityEngine.Random.Range(int.MinValue, int.MaxValue); 
                UnityEngine.Random.InitState(Time.frameCount + 3);
                int valB = UnityEngine.Random.Range(int.MinValue, int.MaxValue); ; 
                return (valA, valB).GetHashCode();
            }
        }
        ///<summary>
        ///Waits all vtasks instances.
        ///</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<bool> VWaitAll<T>(ValueTask<ValueTask> vtask, CancellationToken cts)
        {
            while(instances > 0)
            {
                try
                {
                    await Task.Yield();
                }
                catch(Exception)
                {
                    await vtask;
                    return true;
                }
            }

            await vtask;            
            return true;
        }
        ///<summary>
        ///Run on threadPool.
        ///</summary>
        public static async ValueTask<T> RunVOnThread<T>(ValueTask<T> vtask, CancellationToken ctoke)
        {
            return await Task.Run(async () => 
            {
                if(vtask != null)
                {
                    await vtask;
                }
                
                return await new ValueTask<T>();
            }, ctoke);
        }
        ///<summary>
        ///Tries to cancel task. May fail but will safely be catched internally. The exception won't get stacked.
        ///</summary>
        public static T TryCancelVTasks<T>(this ValueTask<T> val, CancellationTokenSource cts = null, bool dispose = false)
        {
            //TODO: Unity doesn't support consuming valuetask via GetAwaiter().GetResult() yet
            try
            {
                if(cts != null)
                {
                    cts.Cancel();
                    
                    if(dispose)
                        cts.Dispose();
                }

                if(!val.IsCompleted)
                    val = default(ValueTask<T>);
                
                return val.GetAwaiter().GetResult();
            }
            catch(Exception e)
            {
                if(e is OperationCanceledException)
                {
                }
                else if (e is ObjectDisposedException)
                {
                    throw e;
                }
            }

            return val.GetAwaiter().GetResult();
        }
        ///<summary>
        ///Tries to cancel task. May fail but will safely be catched internally. The exception won't get stacked.
        ///</summary>
        public static void TryCancelVTasks(this ValueTask vtask, VTokenSource vtokenSource = null, bool dispose = false, Action continueWith = null)
        {
            //TODO: Unity doesn't support consuming valuetask via GetAwaiter().GetResult() yet
            try
            {
                bool success = false;

                if(vtokenSource != null)
                {
                    try
                    {
                        CancelVToken(vtokenSource, true);
                        vtask.GetAwaiter().GetResult();
                        success = true;
                    }
                    catch(Exception e)
                    {
                        success = false;
                        throw e;
                    }
                }
                else
                {
                    return;
                }

                if(success && continueWith != null)
                {
                    continueWith.Invoke();
                }
            }
            catch(Exception e)
            {
                if(e is OperationCanceledException)
                {

                }
                else if (e is ObjectDisposedException)
                {
                    throw e;
                }                
            }
        }
        ///<summary>
        ///Fire and forget via discards operator.
        ///</summary>
        public static void VRunSynchronous(this ValueTask vtask)
        {
            try
            {
                _= vtask;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        ///<summary>
        ///Fire and forget via discards operator.
        ///</summary>
        public static ValueTask<T> VRunSynchronous<T>(this ValueTask<T> vtask)
        {
            try
            {
                return _= vtask;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        ///<summary>
        ///Identical to async void.
        ///</summary>
        public static async void VSyncVoid(this ValueTask vtask)
        {
            try
            {
                await vtask;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}