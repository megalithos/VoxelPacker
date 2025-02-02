using System.Runtime.CompilerServices;
using System;

namespace VoxelPacker
{
    public static class BitPackUtility
    {
        public const long ACCURACY = 1 << 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int Pack(float f, long accuracy = ACCURACY)
        {
            var i = (int)((f * accuracy) + (0.5f - ((*(uint*)&f) >> 31)));
            return (i >> 31) ^ (i << 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float Unpack(int i, float accuracy = ACCURACY)
        {
            return ((i >> 1) ^ -(i & 1)) / accuracy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ZigZagEncode(long i)
        {
            return (i >> 63) ^ (i << 1);
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ZigZagDecode(int i)
        {
            return (i >> 1) ^ -(i & 1);
        }

        private static readonly ulong MAX_VALUE_THAT_CAN_BE_REPRESENTED_AS_VARULONG = 144_115_188_075_855_872; // 2^(64-7)
        /// <returns>How many bytes were written</returns>
        public static int WriteVarU64(out ulong output, ulong value)
        {
            int count = 0;
            output = 0;
            int leftShiftAmount = 0;
            do
            {
                ulong n = (value & 0x7f);

                value >>= 7;

                if (value != 0)
                {
                    n |= 128;
                }

                n <<= leftShiftAmount;
                output |= n;

                leftShiftAmount += 8;

                count++;
            } while (value != 0);

            return count;
        }

        /// <returns>returns how many bytes were read</returns>
        public static int ReadVarU64(out ulong output, ulong readFrom)
        {
            var r = default(ulong);
            var s = 0;

            ulong n = readFrom;
            int count = 0;

            bool moreData = false;
            do
            {
                r |= ((ulong)(n & 0x7f)) << s;
                moreData = (n & 128) == 128;
                s += 7;
                n >>= 8;
                count++;
            } while (moreData);

            output = r;

            return count;
        }
    }
}
