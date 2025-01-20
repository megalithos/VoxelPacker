using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker.def
{
    [Flags]
    public enum voxelflags : byte
    {
        Placed = 1,
        Damaged = 2,
        UseUShorts = 4,
        Floating = 8,
    }
}
