using System;

namespace Korn.Utils
{
    public class ProcessThread : IDisposable
    {
        public ProcessThread(int id) => ID = id;

        public readonly int ID;

        bool isThreadInitialized;
        ProcessThreadHandle thread;
        public ProcessThreadHandle Thread
        {
            get
            {
                if (!isThreadInitialized)
                {
                    isThreadInitialized = true;
                    thread = ProcessThreadHandle.Open(this);
                }

                return thread;
            }
        }

        public void Suspend() => Thread.Suspend();

        public void Resume() => Thread.Resume();

        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            if (isThreadInitialized)
                ProcessThreadHandle.Close(Thread);
        }
    }
}