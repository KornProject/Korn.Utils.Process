namespace Korn.Utils;
public unsafe struct ExternalProcess
{
    const int PROCESS_ALL_ACCESS = 0xF0000 | 0x100000 | 0xFFFF;

    ExternalProcess(IntPtr processHandle) => Handle = processHandle;

    public readonly IntPtr Handle;

    public ExternalMemory Memory => new ExternalMemory(Handle);

    public int GetParentProcessId()
    {
        var processBasicInformation = Ntdll.NtQueryBasicInformationProcess(Handle);
        var parentProcessId = (int)processBasicInformation.InheritedFromUniqueProcessID;

        return parentProcessId;
    }

    public void CreateThread(Address address, Address param, int stackSize = 0)
    {
        Kernel32.CreateRemoteThread(Handle, 0, stackSize, address, param, 0, null);
    }

    public void NtSuspendProcess() => Ntdll.NtSuspendProcess(Handle);
    public void NtResumeProcess() => Ntdll.NtResumeProcess(Handle);

    public static ExternalProcess Open(ExternalProcessId processId) => new ExternalProcess(Kernel32.OpenProcess(PROCESS_ALL_ACCESS, false, processId.ID));
    public static void Close(ExternalProcess process) => Kernel32.CloseHandle(process.Handle);
}
