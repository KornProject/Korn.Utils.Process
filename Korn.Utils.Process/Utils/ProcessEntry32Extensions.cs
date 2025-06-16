namespace Korn.Utils
{
    public unsafe static class ProcessEntry32Extensions
    {
        public static byte* GetProcessNameWithoutExtension(this ProcessEntry32 entry, int* length)
        {
            const int ExeSuffix = '.' << 0x00 | 'e' << 0x08 | 'x' << 0x10 | 'e' << 0x18;

            var path = entry.ExeFile;
            var cursor = path;

            while (true)
            {
                var value = *(ulong*)cursor;
                var mask = 0xFFUL;

                for (var byteIndex = 0; byteIndex < 8; byteIndex++)
                    if ((value & mask) == 0)
                    {
                        if (*(int*)&cursor[-4] == ExeSuffix)
                            *length = (int)(cursor - path) - 4;
                        else *length = (int)(cursor - path);

                        return path;
                    }
                    else
                    {
                        cursor++;
                        mask <<= 8;
                    }
            }
        }
    }
}