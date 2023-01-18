using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace VTasks
{
    public struct VToken
    {
        public VToken(int length)
        {
            vtokenSource = new VTokenSource();
            vtoken = new CancellationToken[length];
            usedCounter = 0;
            wasDisposed = false;
            wasCancelled = false;
        }
        public VTokenSource vtokenSource;
        public CancellationToken[] vtoken;
        public int usedCounter;
        public bool wasDisposed;
        public bool wasCancelled;
    }
}