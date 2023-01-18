using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using UnityEngine.SceneManagement;
using System.Buffers;

namespace VTasks
{
    public class VTokenManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeMethodLoad()
        {
            VTokenManager.VRootToken = new VToken
            {
                vtokenSource = new VTokenSource(),
                wasCancelled = false,
                wasDisposed = false,
                usedCounter = 0
            };

            VTokenManager.PoolVToken(VTokenManager.VRootToken.vtokenSource);
        }
        public static int globalVId {get;set;} 
        private static ConcurrentQueue<(int, VTokenSource)> VTokes = new ConcurrentQueue<(int, VTokenSource)>();
        private static ConcurrentQueue<Action> STasks = new ConcurrentQueue<Action>();
        private static HashSet<VTokenSource> VTokenSources = new HashSet<VTokenSource>();
        public static VToken VRootToken;

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
                    //no need to throw, tasks excp won't stacked
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
        public static void PoolVToken(VTokenSource cts)
        {
            VTokenSources.Add(cts);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int VHash(int hashCode)
        {
            unchecked
            {
                int valA = UnityEngine.Random.Range(int.MinValue, int.MaxValue); 
                int valB = hashCode; 

                return (valA, valB).GetHashCode();            
            }
        }
    }
}
