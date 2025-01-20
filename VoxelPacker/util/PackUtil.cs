using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker.util
{
    internal static class PackUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ZigZagDecode(long i)
        {
            return (i >> 1) ^ -(i & 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ZigZagEncode(int i)
        {
            return (i >> 31) ^ (i << 1);
        }
    }
}
