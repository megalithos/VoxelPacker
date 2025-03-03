/*
MIT License

Copyright (c) 2025 Akseli Vanhamaa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
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
