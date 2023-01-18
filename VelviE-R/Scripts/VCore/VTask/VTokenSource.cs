using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System;

namespace VTasks
{
    public class VTokenSource : CancellationTokenSource
    {
        public VTokenSource(bool rootToken = false)
        {
            RootToken = this.Token;            
        }

        public bool wasDisposed { get; private set; }
        private CancellationTokenSource[] ctokes;
        private List<CancellationTokenSource> ctokesDynamic;
        private bool wasInitialized = false;
        public bool wasInitializedDynamic {get;set;} = false;
        public int dynamicUsedCounter {get;set;} = 0;
        public CancellationToken RootToken{get;set;}
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            wasDisposed = true;
        }
        public virtual VTokenSource PreserveSlot(int length)
        {
            if(wasInitialized)
                throw new Exception("Error, sas initialized! Use PreserveDynamicSlot instead.");
            else
                wasInitialized = true;

            ctokes = new CancellationTokenSource[length];
            return this;
        }
        public virtual VTokenSource PreserveDynamicSlot(int length = 4)
        {
            if(!wasInitializedDynamic)
            {
                wasInitializedDynamic = true;
                ctokesDynamic = new List<CancellationTokenSource>(4);
            }
            
            ctokes = new CancellationTokenSource[length];
            return this;
        }
        public virtual CancellationTokenSource AddChild(CancellationToken externalToken)
        {
            if(!wasInitialized)
                throw new Exception("To use AddChild, PreserveSlot must be initialized.");

            return CancellationTokenSource.CreateLinkedTokenSource(RootToken, externalToken);            

        }
        public virtual CancellationTokenSource AddDynamicChild()
        {
            if(!wasInitializedDynamic)
                throw new Exception("To use AddDynamicChild, PreserveDynamicSlot must be initialized.");

            var toke = VTokenSource.CreateLinkedTokenSource(this.Token);
            ctokesDynamic.Add(toke);
            return toke; 
        }
        public virtual void DisposeChild(CancellationTokenSource cts)
        {
            if(wasInitialized)
            {
                if(Array.Exists(ctokes, x => x == cts))
                    cts.Dispose();
                else
                {
                    throw new Exception("Child source not found!");
                }
            }
            if(wasInitializedDynamic)
            {
                if(ctokesDynamic.Contains(cts))
                    cts.Dispose();
                else
                {
                    throw new Exception("Child source not found!");
                }
            }
        }
        public virtual void CancelChild(CancellationTokenSource cts, bool dispose = false)
        {
            if(wasInitialized)
            {
                if(Array.Exists(ctokes, x => x == cts))
                {
                    try
                    {
                        cts.Cancel();

                        if(dispose)
                            cts.Dispose();
                    }
                    catch(ObjectDisposedException)
                    {
                        return;
                    }
                }
                else
                {
                    throw new Exception("Child source not found!");
                }
            }
            if(wasInitializedDynamic)
            {
                if(ctokesDynamic.Contains(cts))
                {
                    cts.Cancel();

                    if(dispose)
                        cts.Dispose();
                }
                else
                {
                    throw new Exception("Child source not found!");
                }
            }

        }
        public virtual void CancelChildAfter(CancellationTokenSource cts, int seconds)
        {
            cts.CancelAfter(seconds);
        }
    }
}
