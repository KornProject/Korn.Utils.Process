using System;

namespace Korn.Utils
{
    public struct ProcessMemoryRegion : IDisposable
    {
        public ProcessMemoryRegion(IntPtr processHandle, IntPtr address) => (ProcessHandle, Address) = (processHandle, address);

        public readonly IntPtr ProcessHandle, Address;

        public void Free() => ProcessMemoryAllocator.FreeRegion(ProcessHandle, Address);

        public void Dispose() => Free();

        public static implicit operator IntPtr(ProcessMemoryRegion region) => region.Address;
    }
}