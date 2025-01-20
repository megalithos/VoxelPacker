using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker
{
    public struct ByteAmount : IEquatable<ByteAmount>
    {
        private const long BytesPerKilobyte = 1000;
        private const long BytesPerMegabyte = BytesPerKilobyte * 1000;

        private long totalBytes;

        public ByteAmount(long bytes)
        {
            totalBytes = bytes;
        }

        public long Bytes
        {
            get => totalBytes;
        }

        public double Kilobytes
        {
            get => totalBytes / (double)BytesPerKilobyte;
        }

        public double Megabytes
        {
            get => totalBytes / (double)BytesPerMegabyte;
        }

        public override string ToString()
        {
            if (totalBytes >= BytesPerMegabyte)
                return $"{Megabytes:0.##} MB";
            else if (totalBytes >= BytesPerKilobyte)
                return $"{Kilobytes:0.##} KB";
            else
                return $"{Bytes} bytes";
        }

        public static ByteAmount FromBytes(long bytes)
        {
            return new ByteAmount(bytes);
        }

        public static ByteAmount FromKilobytes(double kilobytes)
        {
            return new ByteAmount((long)(Math.Round(kilobytes * BytesPerKilobyte)));
        }

        public static ByteAmount FromMegabytes(double megabytes)
        {
            return new ByteAmount((long)(Math.Round(megabytes * BytesPerMegabyte)));
        }

        public static bool operator ==(ByteAmount lhs, ByteAmount rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ByteAmount lhs, ByteAmount rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static bool operator <(ByteAmount lhs, ByteAmount rhs)
        {
            return lhs.Bytes < rhs.Bytes;
        }

        public static bool operator >(ByteAmount lhs, ByteAmount rhs)
        {
            return lhs.Bytes > rhs.Bytes;
        }

        public override int GetHashCode()
        {
            return totalBytes.GetHashCode();
        }

        public bool Equals(ByteAmount other)
        {
            return totalBytes == other.totalBytes;
        }
    }
}
