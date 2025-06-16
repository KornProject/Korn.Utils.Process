using Korn.Utils;

var processes = Process.Processes.GetProcessesByName("notepad");

var process = Process.Current;

var threads = process.Threads.GetThreads();

var modules = process.Modules.GetModules();

Thread.Sleep(-1);
