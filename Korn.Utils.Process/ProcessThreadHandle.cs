using Korn.Modules.WinApi;
using System;

namespace Korn.Utils
{
    public struct ProcessThreadHandle
    {
        const int THREAD_STATE_ACCESS = 0x0002;
        const uint WAIT_TIMEOUT = 0x00000102;

        public ProcessThreadHandle(IntPtr handle) => Handle = handle;

        public readonly IntPtr Handle;

        public void Suspend() => Kernel32.SuspendThread(Handle);

        public void Resume() => Kernel32.ResumeThread(Handle);

        public void Join() => Kernel32.WaitForSingleObject(Handle, -1);

        /// <returns>false if timeout has occurred</returns>
        public bool Join(int timeout) => Kernel32.WaitForSingleObject(Handle, timeout) != WAIT_TIMEOUT;

        public static ProcessThreadHandle Open(ProcessThread threadId) => new ProcessThreadHandle(Kernel32.OpenThread(THREAD_STATE_ACCESS, false, threadId.ID));
        public static void Close(ProcessThreadHandle thread) => Kernel32.CloseHandle(thread.Handle);
    }
}