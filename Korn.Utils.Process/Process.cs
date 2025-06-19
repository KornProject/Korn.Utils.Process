using Korn.Modules.WinApi;
using System;
using System.IO;

namespace Korn.Utils
{
    public class Process : IDisposable
    {
        public Process(int pid) => ID = pid;

        public Process(int id, int parentId) : this(id)
        {
            isParentIdKnown = true;
            this.parentId = parentId;
        }

        public Process(int id, int parentId, string name) : this(id, parentId)
        {
            isNameInitialized = true;
            this.name = name;
        }

        public readonly int ID;

        bool isHandleInitialized;
        ProcessHandle handle;
        public ProcessHandle Handle
        {
            get
            {
                if (!isHandleInitialized)
                {
                    isHandleInitialized = true;
                    handle = ProcessHandle.Open(this);
                }

                return handle;
            }
        }

        bool isParentIdKnown;
        int parentId;
        public int ParentId
        {
            get
            {
                if (!isParentIdKnown)
                {
                    isParentIdKnown = true;
                    parentId = Handle.GetParentProcessId();
                }

                return parentId;
            }
        }

        bool isModulesInitialized;
        ProcessModuleCollection modules;
        public ProcessModuleCollection Modules
        {
            get
            {
                if (!isModulesInitialized)
                {
                    isModulesInitialized = true;
                    modules = new ProcessModuleCollection(this);
                }

                return modules;
            }
        }

        bool isThreadsInitialized;
        ProcessThreadCollection threads;
        public ProcessThreadCollection Threads
        {
            get
            {
                if (!isThreadsInitialized)
                {
                    isThreadsInitialized = true;
                    threads = new ProcessThreadCollection(this);
                }

                return threads;
            }
        }

        string executablePath;
        bool isExecutablePathInitialized;
        public string ExecutablePath
        {
            get
            {
                if (!isExecutablePathInitialized)
                {
                    isExecutablePathInitialized = true;
                    executablePath = Handle.GetExecutableFilePath();
                }

                return executablePath;
            }
        }

        bool isNameInitialized;
        string name;
        public string Name
        {
            get
            {
                if (!isNameInitialized)
                {
                    isNameInitialized = true;
                    name = Path.GetFileNameWithoutExtension(ExecutablePath);
                }

                return name;
            }
        }

        public ProcessMemory Memory => Handle.Memory;

        public ProcessMemoryAllocator MemoryAllocator => Handle.MemoryAllocator;

        public void FastSuspend() => Handle.NtSuspendProcess();

        public void FastResume() => Handle.NtResumeProcess();

        public void Suspend() => Threads.SuspendThreads();

        public void Resume() => Threads.ResumeThreads();

        public ProcessThreadHandle CreateThread(IntPtr address, IntPtr param, int stackSize = 0) => Handle.CreateThread(address, param, stackSize);

        public void Kill(int exitCode = 0) => Handle.Kill(exitCode);

        static bool isCurrentInitialized;
        static Process current;
        public static Process Current
        {
            get
            {
                if (!isCurrentInitialized)
                {
                    isCurrentInitialized = true;
                    var id = Kernel32.GetCurrentProcessId();
                    current = new Process(id);
                }

                return current;
            }
        }

        static bool isProcessesInitialized;
        static ProcessCollection processes;
        public static ProcessCollection Processes
        {
            get
            {
                if (!isProcessesInitialized)
                {
                    isProcessesInitialized = true;
                    processes = new ProcessCollection(Current);
                }

                return processes;
            }
        }

        #region IDisposable
        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            if (isHandleInitialized)
                ProcessHandle.Close(handle);

            if (isModulesInitialized)
                modules.Dispose();

            if (isThreadsInitialized)
                threads.Dispose();
        }

        ~Process() => Dispose();
        #endregion IDisposable
    }
}