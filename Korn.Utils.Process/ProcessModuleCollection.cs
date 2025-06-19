using Korn.Modules.WinApi;
using Korn.Modules.WinApi.Kernel;
using System;
using System.Collections.Generic;

namespace Korn.Utils
{
    public unsafe class ProcessModuleCollection : Toolhelp32Snapshot
    {
        public ProcessModuleCollection(Process processId) : base(processId, SnapshotFlags.Module | SnapshotFlags.Module32) => process = processId.Handle;

        ProcessHandle process;

        bool Module32First(ModuleEntry32* module) => Kernel32.Module32First(Handle, module);
        bool Module32Next(ModuleEntry32* module) => Kernel32.Module32Next(Handle, module);

        bool GetModuleEntry(string nameWithExtension, ModuleEntry32* module) => GetModuleEntry(nameWithExtension, StringFootprint.Footprint(nameWithExtension), module);
        bool GetModuleEntry(string nameWithExtension, StringFootprint footprint, ModuleEntry32* module)
        {
            module->Size = sizeof(ModuleEntry32);
            if (Module32First(module))
                do
                    if (*(ulong*)module->ModuleName == footprint)
                        if (module->ModuleNameString == nameWithExtension)
                            return true;
                while (Module32Next(module));

            return false;
        }

        public ProcessModule GetModule(string nameWithExtension) => GetModule(nameWithExtension, StringFootprint.Footprint(nameWithExtension));
        public ProcessModule GetModule(string nameWithExtension, StringFootprint footprint)
        {
            ModuleEntry32 moduleEntry;
            if (!GetModuleEntry(nameWithExtension, footprint, &moduleEntry))
                return default;

            var handle = moduleEntry.ModuleHandle;
            var name = moduleEntry.ModuleNameString;
            var path = moduleEntry.ExePathString;
            var module = new ProcessModule(handle, name, path);
            return module;
        }

        public IntPtr GetModuleHandle(string nameWithExtension) => GetModuleHandle(nameWithExtension, StringFootprint.Footprint(nameWithExtension));
        public IntPtr GetModuleHandle(string nameWithExtension, StringFootprint footprint)
        {
            ModuleEntry32 moduleEntry;
            if (GetModuleEntry(nameWithExtension, footprint, &moduleEntry))
                return moduleEntry.ModuleHandle;
            return IntPtr.Zero;
        }
        
        public List<ProcessModule> GetModules()
        {
            var modules = new List<ProcessModule>(128);

            var moduleEntry = stackalloc ModuleEntry32[1];
            moduleEntry->Size = sizeof(ModuleEntry32);

            if (Module32First(moduleEntry))
                do
                {
                    var handle = moduleEntry->ModuleHandle;
                    var name = moduleEntry->ModuleNameString;
                    var path = moduleEntry->ExePathString;
                    var module = new ProcessModule(handle, name, path);
                    modules.Add(module);
                }
                while (Module32Next(moduleEntry));

            return modules;
        }

        public bool ContainsModule(string nameWithExtension) => GetModuleHandle(nameWithExtension) != IntPtr.Zero;        
    }
}