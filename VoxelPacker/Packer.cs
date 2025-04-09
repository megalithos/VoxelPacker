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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPacker.def;
using VoxelPacker.util;

namespace VoxelPacker
{
    public static class Packer
    {
        public static byte[] Pack(List<voxeldata> datas)
        {
            int len = datas.Count;
            int write_index = 0;
            byte[] buffer = new byte[len * voxeldata.max_bytes_per_voxeldata];

            vec3i prev = default;
            for (int i = 0; i < len; i++)
            {
                voxeldata curr = datas[i];

                vec3i delta = curr.position - prev;
                prev = curr.position;

                bool has_color = (curr.flags & voxelflags.Placed) != 0;
                bool hasDamagedValue = (curr.flags & voxelflags.Damaged) != 0;

                int dx = PackUtil.ZigZagEncode(delta.x);
                int dy = PackUtil.ZigZagEncode(delta.y);
                int dz = PackUtil.ZigZagEncode(delta.z);

                bool use_ushorts = false;
                if (dx > 255 || dy > 255 || dz > 255)
                {
                    use_ushorts = true;
                }

                if (use_ushorts)
                {
                    voxelflags flags = curr.flags;
                    flags |= voxelflags.UseUShorts;
                    curr.flags = flags;
                }

                buffer[write_index++] = (byte)curr.flags;

                if (!use_ushorts)
                {
                    buffer[write_index++] = Convert.ToByte(dx);
                    buffer[write_index++] = Convert.ToByte(dy);
                    buffer[write_index++] = Convert.ToByte(dz);
                }
                else
                {
                    buffer[write_index++] = Convert.ToByte(dx & 0xFF);
                    buffer[write_index++] = Convert.ToByte((dx >> 8) & 0xFF);
                    buffer[write_index++] = Convert.ToByte(dy & 0xFF);
                    buffer[write_index++] = Convert.ToByte((dy >> 8) & 0xFF);
                    buffer[write_index++] = Convert.ToByte(dz & 0xFF);
                    buffer[write_index++] = Convert.ToByte((dz >> 8) & 0xFF);
                }

                if (has_color)
                {
                    buffer[write_index++] = Convert.ToByte(curr._color.r);
                    buffer[write_index++] = Convert.ToByte(curr._color.g);
                    buffer[write_index++] = Convert.ToByte(curr._color.b);
                    buffer[write_index++] = Convert.ToByte(curr._color.a);
                }

                if (hasDamagedValue)
                {
                    buffer[write_index++] = Convert.ToByte(curr.damagedValue);
                }
            }

            int written_length = write_index;

            byte[] compressed = compress(buffer);

            bool useCompressed = compressed.Length < written_length;

            // header
            //   compressed flag (1 b)
            //   count_voxels (31 b)
            byte[] outbuf = null;
            if (!useCompressed)
            {
                outbuf = new byte[written_length + 4];
                Buffer.BlockCopy(buffer, 0, outbuf, 4, written_length);
            }
            else
            {
                outbuf = new byte[compressed.Length + 4];
                Buffer.BlockCopy(compressed, 0, outbuf, 4, compressed.Length);
            }

            int header = (useCompressed ? (byte)1 : (byte)0) | (datas.Count << 1);

            outbuf[0] = (byte)((header) & 0xFF);
            outbuf[1] = (byte)((header >> 8) & 0xFF);
            outbuf[2] = (byte)((header >> 16) & 0xFF);
            outbuf[3] = (byte)((header >> 24) & 0xFF);

            return outbuf;
        }

        public static List<voxeldata> Unpack(byte[] buffer)
        {
            if (buffer == null || buffer.Length < 4)
                throw new ArgumentException("Invalid buffer.");

            int read_index = 0;

            int header = buffer[read_index++] |
                             (buffer[read_index++] << 8) |
                             (buffer[read_index++] << 16) |
                             (buffer[read_index++] << 24);

            bool compressed = (header & 1) == 0 ? false : true;

            int count = (header >> 1);

            if (compressed)
            {
                buffer = decompress(buffer, 4, buffer.Length - 4);
                read_index = 0;
            }

            if (buffer.Length < count + 4)
                throw new ArgumentException("Buffer size does not match the encoded length.");

            List<voxeldata> datas = new List<voxeldata>(count);
            vec3i prev = default;

            for (int i = 0; i < count; i++)
            {
                voxeldata current = new voxeldata();
                current.flags = (voxelflags)buffer[read_index++];

                bool use_ushorts = (current.flags & voxelflags.UseUShorts) != 0;
                bool has_color = (current.flags & voxelflags.Placed) != 0;
                bool hasDamagedValue = (current.flags & voxelflags.Damaged) != 0;

                int dx, dy, dz;

                if (!use_ushorts)
                {
                    dx = (int)PackUtil.ZigZagDecode(buffer[read_index++]);
                    dy = (int)PackUtil.ZigZagDecode(buffer[read_index++]);
                    dz = (int)PackUtil.ZigZagDecode(buffer[read_index++]);
                }
                else
                {
                    dx = (int)PackUtil.ZigZagDecode(buffer[read_index++] | (buffer[read_index++] << 8));
                    dy = (int)PackUtil.ZigZagDecode(buffer[read_index++] | (buffer[read_index++] << 8));
                    dz = (int)PackUtil.ZigZagDecode(buffer[read_index++] | (buffer[read_index++] << 8));
                }

                vec3i delta = new vec3i(dx, dy, dz);
                current.position = prev + delta;
                prev = current.position;

                if (has_color)
                {
                    current._color = new color
                    {
                        r = buffer[read_index++],
                        g = buffer[read_index++],
                        b = buffer[read_index++],
                        a = buffer[read_index++]
                    };
                }

                if (hasDamagedValue)
                {
                    current.damagedValue = buffer[read_index++];
                }

                datas.Add(current);
            }

            return datas;
        }

        private static byte[] compress(byte[] buffer)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(memoryStream, CompressionLevel.Fastest, leaveOpen: true))
                {
                    deflateStream.Write(buffer, 0, buffer.Length);
                }
                return memoryStream.ToArray();
            }
        }

        private static byte[] decompress(byte[] buffer, int startIdx, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (startIdx < 0 || length < 0 || startIdx + length > buffer.Length)
                throw new ArgumentOutOfRangeException();

            using (var inputStream = new MemoryStream(buffer, startIdx, length))
            using (var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                deflateStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }
    }
}
