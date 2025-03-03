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
    public struct vec3i
    {
        public int x;
        public int y;
        public int z;

        public vec3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static vec3i operator -(vec3i a, vec3i b)
        {
            return new vec3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static vec3i operator +(vec3i a, vec3i b)
        {
            return new vec3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static vec3i operator *(vec3i a, int scalar)
        {
            return new vec3i(a.x * scalar, a.y * scalar, a.z * scalar);
        }

        public static bool operator ==(vec3i a, vec3i b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(vec3i a, vec3i b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is vec3i other)
            {
                return this == other;
            }
            return false;
        }

        public override string ToString()
        {
            return $"vec3i ({x} / {y} / {z})";
        }
    }
}
