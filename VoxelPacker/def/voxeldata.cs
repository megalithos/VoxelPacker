using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker.def
{
    public struct voxeldata
    {
        public voxelflags flags;
        public vec3i position;
        public color _color;
        public int damagedValue;
    }
}
