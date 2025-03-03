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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPacker;
using VoxelPacker.def;
using VoxelPacker.util;

namespace TestProject1
{
    public static class TestHelper
    {
        public static List<voxeldata> parse(string path)
        {
            List<voxeldata> list = new List<voxeldata>();

            string[] contents = File.ReadAllLines(path);

            vec3i prev = default;

            for (int i = 0; i < contents.Length; i++)
            {
                string line = contents[i];
                string[] components = line.Split(' ');

                vec3i vec = default;

                byte flags = (byte)int.Parse(components[0]);

                bool isSolid = (flags & 1) != 0 ? true : false;
                bool damaged = (flags & 2) != 0 ? true : false;

                vec.x = int.Parse(components[1]);
                vec.y = int.Parse(components[2]);
                vec.z = int.Parse(components[3]);

                vec3i delta = vec - prev;
                prev = vec;

                int color_r = default;
                int color_g = default;
                int color_b = default;
                if (isSolid)
                {
                    color_r = int.Parse(components[4]);
                    color_g = int.Parse(components[5]);
                    color_b = int.Parse(components[6]);
                }

                int damaged_value = 0;
                if (damaged)
                {
                    damaged_value = int.Parse(components[7]);
                }

                bool useDouble = false;

                int dx = PackUtil.ZigZagEncode(delta.x);
                int dy = PackUtil.ZigZagEncode(delta.y);
                int dz = PackUtil.ZigZagEncode(delta.z);

                if (dx > 255 || dy > 255 || dz > 255)
                {
                    flags |= 4;
                    useDouble = true;
                }

                voxeldata data = default;
                data.position = vec;
                
                if (isSolid)
                {
                    color c = default;
                    c.r = Convert.ToByte(color_r);
                    c.g = Convert.ToByte(color_g);
                    c.b = Convert.ToByte(color_b);
                    data._color = c;
                }

                if (damaged)
                {
                    data.damagedValue = damaged_value;
                }

                data.flags = (voxelflags)flags;

                list.Add(data);
            }
            return list;
        }
    }
}
