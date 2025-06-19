using Korn.Modules.WinApi;
using Korn.Modules.WinApi.Kernel;
using System;

namespace Korn.Utils
{
    public class Toolhelp32Snapshot : IDisposable
    {
        public Toolhelp32Snapshot(Process processId, SnapshotFlags flags) => Handle = Kernel32.CreateToolhelp32Snapshot(flags, ProcessID = processId.ID);

        public readonly int ProcessID;
        public readonly IntPtr Handle;

        #region IDisposable
        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            Kernel32.CloseHandle(Handle);
        }

        ~Toolhelp32Snapshot() => Dispose();
        #endregion
    }
}