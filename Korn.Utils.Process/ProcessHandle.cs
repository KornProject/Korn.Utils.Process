using System;
using System.Diagnostics;

namespace Korn.Utils
{
    public unsafe struct ProcessHandle
    {
        const int PROCESS_ALL_ACCESS = 0xF0000 | 0x100000 | 0xFFFF;

        ProcessHandle(IntPtr processHandle) => Handle = processHandle;

        public readonly IntPtr Handle;

        public ProcessMemory Memory => new ProcessMemory(Handle);

        public ProcessMemoryAllocator MemoryAllocator => new ProcessMemoryAllocator(Handle);

        public int GetParentProcessId()
        {
            var processBasicInformation = Ntdll.NtQueryBasicInformationProcess(Handle);
            var parentProcessId = (int)processBasicInformation.InheritedFromUniqueProcessID;

            return parentProcessId;
        }

        public string GetExecutableFilePath() => Psapi.GetBaseModuleFileNameEx(Handle);

        public ProcessThreadHandle CreateThread(Address address, Address param, int stackSize = 0)
        {
            var threadHandle = Kernel32.CreateRemoteThread(Handle, 0, stackSize, address, param, 0, null);
            return *(ProcessThreadHandle*)&threadHandle;
        }

        public void NtSuspendProcess() 
        {
#if DEBUG
            Debug.WriteLine($"NtSuspendProcess: {Handle.ToHexString()}");
#endif
            Ntdll.NtSuspendProcess(Handle); 
        }

        public void NtResumeProcess()
        {
#if DEBUG
            Debug.WriteLine($"NtResumeProcess: {Handle.ToHexString()}");
#endif
            Ntdll.NtResumeProcess(Handle);
        }

        public void Kill(int exitCode = 0) => Kernel32.TerminateProcess(Handle, (uint)exitCode);

        public static ProcessHandle Open(Process processId) => new ProcessHandle(Kernel32.OpenProcess(PROCESS_ALL_ACCESS, false, processId.ID));
        public static void Close(ProcessHandle process) => Kernel32.CloseHandle(process.Handle);

        public static implicit operator Address(ProcessHandle self) => self.Handle;
        public static implicit operator IntPtr(ProcessHandle self) => self.Handle;
    }
}