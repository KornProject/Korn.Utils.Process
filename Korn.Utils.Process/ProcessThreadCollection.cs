using Korn.Modules.WinApi;
using Korn.Modules.WinApi.Kernel;
using System.Collections.Generic;

namespace Korn.Utils
{
    public unsafe class ProcessThreadCollection : Toolhelp32Snapshot
    {
        const int THREAD_STATE_ACCESS = 0x0002;

        public ProcessThreadCollection(Process process) : base(process, SnapshotFlags.Thread) { }

        bool Thread32First(ThreadEntry32* thread) => Kernel32.Thread32First(Handle, thread);
        bool Thread32Next(ThreadEntry32* thread) => Kernel32.Thread32Next(Handle, thread);

        public List<ProcessThread> GetThreads()
        {
            var threads = new List<ProcessThread>();

            var id = ProcessID;
            var entry = stackalloc ThreadEntry32[1];
            entry->Size = sizeof(ThreadEntry32);

            if (Thread32First(entry))
                do
                    if (entry->OwnerProcessID == id)
                        threads.Add(new ProcessThread(entry->ThreadID));
                while (Thread32Next(entry));

            return threads;
        }

        public void SuspendThreads()
        {
            var threads = GetThreads();
            foreach (var thread in threads)
                thread.Suspend();
        }

        public ProcessSuspendedThreads SuspendAndReturnThreads()
        {
            var threads = GetThreads();
            foreach (var thread in threads)
                thread.Suspend();

            var suspendedThreads = new ProcessSuspendedThreads(this, threads);
            return suspendedThreads;
        }

        public void ResumeThreads()
        {
            var threads = GetThreads();
            foreach (var thread in threads)
                thread.Resume();
        }
    }
}