using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker.def
{
    public struct color
    {
        public byte r;
        public byte g;
        public byte b;

        public override bool Equals(object obj)
        {
            if (obj is color other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(color other)
        {
            return r == other.r && g == other.g && b == other.b;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + r.GetHashCode();
            hash = hash * 31 + g.GetHashCode();
            hash = hash * 31 + b.GetHashCode();
            return hash;
        }

        public static bool operator ==(color left, color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(color left, color right)
        {
            return !(left == right);
        }
    }
}
