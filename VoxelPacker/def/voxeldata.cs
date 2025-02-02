using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker.def
{
    public struct voxeldata : IEquatable<voxeldata>
    {
        public const int max_bytes_per_voxeldata = 11;

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
