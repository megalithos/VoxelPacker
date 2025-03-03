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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker.def
{
    [StructLayout(LayoutKind.Sequential)]
    public class ModifiedVoxel
    {
        public vec3i WorldPosition { get; private set; }
        public int Color { get; private set; }
        public bool Solid { get; private set; }

        public bool Falling { get; private set; }
        public bool IsDummy { get; private set; }
        public int VersionNumber { get; set; }

        public byte Id => 0;

        public byte DamagedValue { get; private set; }

        public ModifiedVoxel() { }

        public ModifiedVoxel(vec3i worldPosition, int color, bool solid, bool falling, bool isDummy = false, byte damagedValue = 0)
        {
            WorldPosition = worldPosition;
            Color = color;
            Solid = solid;
            Falling = falling;
            IsDummy = isDummy;
            DamagedValue = damagedValue;
        }

        private bool EqualsInternal(ModifiedVoxel x, ModifiedVoxel y)
        {
            if (x == null || y == null)
                return false;

            return x.WorldPosition == y.WorldPosition;
        }

        private int GetHashCodeInternal(ModifiedVoxel obj)
        {
            return WorldPosition.GetHashCode();
        }

        public void Serialize(ref CompressedOutputStream outputStream, vec3i previousPosition)
        {
            bool xChanged = WorldPosition.x != previousPosition.x;
            bool yChanged = WorldPosition.y != previousPosition.y;
            bool zChanged = WorldPosition.z != previousPosition.z;
            bool hasColor = Color != 0;

            Bool8 bitfield = default;
            bitfield[0] = xChanged;
            bitfield[1] = yChanged;
            bitfield[2] = zChanged;
            bitfield[3] = Solid;
            bitfield[4] = IsDummy;
            bool isDamaged = DamagedValue > 0;
            bitfield[5] = isDamaged;
            bitfield[6] = hasColor;
            bitfield[7] = Falling;

            outputStream.WriteRawBits(bitfield.ByteValue, 8);
            if (isDamaged)
            {
                outputStream.WriteRawBits(DamagedValue, 2);
            }

            if (hasColor)
            {
                outputStream.WritePackedUInt((uint)Color);
            }

            if (xChanged)
            {
                outputStream.WritePackedUIntDelta((uint)WorldPosition.x, (uint)previousPosition.x);
            }
            if (yChanged)
            {
                outputStream.WritePackedUIntDelta((uint)WorldPosition.y, (uint)previousPosition.y);
            }
            if (zChanged)
            {
                outputStream.WritePackedUIntDelta((uint)WorldPosition.z, (uint)previousPosition.z);
            }

            outputStream.WritePackedUInt((uint)VersionNumber);
        }

        public override string ToString()
            => $"ModifiedVoxel (WorldPosition: {WorldPosition}, Color: {Color}, Solid: {Solid}, DamagedValue: {DamagedValue}, VersionNumber: {VersionNumber}, Falling: {Falling})";

        public void Reset()
        {
            WorldPosition = default;
            Color = default;
            Solid = default;
            Falling = default;
            IsDummy = default;
            VersionNumber = default;
            DamagedValue = default;
        }

        public void CopyFrom(ModifiedVoxel other)
        {
            WorldPosition = other.WorldPosition;
            Color = other.Color;
            Solid = other.Solid;
            Falling = other.Falling;
            IsDummy = other.IsDummy;
            VersionNumber = other.VersionNumber;
            DamagedValue = other.DamagedValue;
        }

        public void ResetToDefaultState()
        {
            Reset();
        }
    }

}
