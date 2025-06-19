using Korn.Modules.WinApi;
using Korn.Modules.WinApi.Kernel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Korn.Utils
{
    public unsafe class ProcessCollection : Toolhelp32Snapshot
    {
        public ProcessCollection(Process process) : base(process, SnapshotFlags.Process) { }

        bool Thread32First(ProcessEntry32* thread) => Kernel32.Process32First(Handle, thread);
        bool Thread32Next(ProcessEntry32* thread) => Kernel32.Process32Next(Handle, thread);

#if NET8_0
        [SkipLocalsInit]
#endif
        public List<Process> GetProcesses()
        {
            var processes = new List<Process>(512);

            var entry = stackalloc ProcessEntry32[1];
            entry->Size = sizeof(ProcessEntry32);

            if (Thread32First(entry))
                do
                    processes.Add(new Process(entry->ProcessID, entry->ParentProcessID));
                while (Thread32Next(entry));

            return processes;
        }

#if NET8_0
        [SkipLocalsInit]
#endif
        public bool IsProcessExists(string prename)
        {
            var prenameLength = prename.Length;
            var prenameFootprint = StringFootprint.Footprint(prename);

            int nameLength;
            var entry = stackalloc ProcessEntry32[1];
            entry->Size = sizeof(ProcessEntry32);

            if (Thread32First(entry))
                do
                {
                    var fileName = entry->GetProcessNameWithoutExtension(&nameLength);

                    if (prenameLength != nameLength)
                        continue;

                    if (prenameFootprint != StringFootprint.Footprint(fileName, nameLength))
                        continue;

                    return true;
                }
                while (Thread32Next(entry));


            return false;
        }

#if NET8_0
        [SkipLocalsInit]
#endif
        public List<Process> GetProcessesByName(string prename)
        {
            var prenameLength = prename.Length;
            var prenameFootprint = StringFootprint.Footprint(prename);
            var processes = new List<Process>(8);

            int nameLength;
            var entry = stackalloc ProcessEntry32[1];
            entry->Size = sizeof(ProcessEntry32);

            if (Thread32First(entry))
                do
                {
                    var fileName = entry->GetProcessNameWithoutExtension(&nameLength);

                    if (prenameLength != nameLength)
                        continue;

                    if (prenameFootprint != StringFootprint.Footprint(fileName, nameLength))
                        continue;

                    if (nameLength <= 8)
                    {
                        var process = new Process(entry->ProcessID, entry->ParentProcessID);
                        processes.Add(process);
                    }
                    else
                    {
                        var name = new string((sbyte*)fileName, 0, nameLength);
                        if (name != prename)
                            continue;

                        var process = new Process(entry->ProcessID, entry->ParentProcessID, name);
                        processes.Add(process);
                    }
                }
                while (Thread32Next(entry));

            return processes;
        }
    }
}