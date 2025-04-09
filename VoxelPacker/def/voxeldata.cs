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

namespace VoxelPacker.def
{
    public struct voxeldata : IEquatable<voxeldata>
    {
        public const int max_bytes_per_voxeldata = 12;

        public voxelflags flags;
        public vec3i position;
        public color _color;
        public int damagedValue;

        public override bool Equals(object obj)
        {
            if (obj is voxeldata other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(voxeldata other)
        {
            return (byte)flags == (byte)other.flags
                && position == other.position
                && _color.Equals(other._color)
                && damagedValue == other.damagedValue;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + flags.GetHashCode();
            hash = hash * 31 + position.GetHashCode();
            hash = hash * 31 + _color.GetHashCode();
            hash = hash * 31 + damagedValue.GetHashCode();
            return hash;
        }

        public static bool operator ==(voxeldata left, voxeldata right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(voxeldata left, voxeldata right)
        {
            return !(left == right);
        }
    }
}
