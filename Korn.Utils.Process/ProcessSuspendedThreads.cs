using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korn.Utils
{
    public class ProcessSuspendedThreads
    {
        public ProcessSuspendedThreads(ProcessThreadCollection processThreads, List<ProcessThread> suspendedThreads)
            => (this.processThreads, this.suspendedThreads) = (processThreads, suspendedThreads);

        ProcessThreadCollection processThreads;
        List<ProcessThread> suspendedThreads;

        public void Resume()
        {
            foreach (var thread in suspendedThreads)
                thread.Resume();
        }
    }
}