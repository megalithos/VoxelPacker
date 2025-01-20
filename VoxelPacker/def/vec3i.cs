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
