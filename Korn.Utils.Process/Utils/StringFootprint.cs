#pragma warning disable IDE0044 // Add readonly modifier
namespace Korn.Utils
{
    public unsafe struct StringFootprint
    {
        ulong value;

        // "some string"u16 -> *(ulong*)&"some str"u8
        // "som"u16 -> *(ulong*)&"som\0\0\0\0\0"u8
        public static StringFootprint Footprint(string inputString)
        {
            fixed (char* chars = inputString)
            {
                var a = *(ulong*)chars;
                var b = *(ulong*)&chars[4];

                var c =
                    a >> 0x00 & 0xFFUL << 0x00 |
                    a >> 0x08 & 0xFFUL << 0x08 |
                    a >> 0x10 & 0xFFUL << 0x10 |
                    a >> 0x18 & 0xFFUL << 0x18 |
                    b << 0x20 & 0xFFUL << 0x20 |
                    b << 0x18 & 0xFFUL << 0x28 |
                    b << 0x10 & 0xFFUL << 0x30 |
                    b << 0x08 & 0xFFUL << 0x38;

                return c;
            }
        }

        public static StringFootprint Footprint(byte* inputU8String) => *(StringFootprint*)inputU8String;
        public static StringFootprint Footprint(byte* inputU8String, int length) => unchecked(*(long*)inputU8String & ~(-1L << (length > 8 ? 8 : length) * 8));

        public static implicit operator ulong(StringFootprint self) => self.value;
        public static implicit operator StringFootprint(ulong value) => *(StringFootprint*)&value;
        public static implicit operator StringFootprint(long value) => *(StringFootprint*)&value;
    }
}