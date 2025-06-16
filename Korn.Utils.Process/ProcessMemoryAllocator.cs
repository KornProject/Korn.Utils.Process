using System;

namespace Korn.Utils
{
    public unsafe struct ProcessMemoryAllocator
    {
        public ProcessMemoryAllocator(IntPtr processHandle) => ProcessHandle = processHandle;

        IntPtr ProcessHandle;

        public ProcessMemoryRegion AllocateRegion(long size) => AllocateRegion(ProcessHandle, size);
        public ProcessMemoryRegion AllocateRegion(Address address, long size) => AllocateRegion(ProcessHandle, address, size);

        public void Free(Address address) => FreeRegion(ProcessHandle, address);

        public static ProcessMemoryRegion AllocateRegion(IntPtr process, long size) => AllocateRegion(process, IntPtr.Zero, size);
        public static ProcessMemoryRegion AllocateRegion(IntPtr process, Address address, long size)
        {
            var nativeRegion = Kernel32.VirtualAllocEx(process, address, size, MemoryState.Commit | MemoryState.Reserve, MemoryProtect.ExecuteReadWrite);
            var region = new ProcessMemoryRegion(process, nativeRegion);
            return region;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-virtualfree: for MemoryFreeType.Release size should be zero
        public static void FreeRegion(IntPtr process, IntPtr address) => Kernel32.VirtualFreeEx(process, address, 0, MemoryFreeType.Release);
    }
}