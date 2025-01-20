using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VoxelPacker
{
    public class cmain
    {
        private static readonly string path = Path.Combine("C:\\Users\\islai\\source\\repos\\VoxelPacker\\VoxelPacker", "destroy_single_voxel");

        // fn pack(list<voxel_t> voxels) -> byte[]
        // fn unpack(byte[] buf, out list<voxel_t>()) -> list<voxel_t>

        public static void Main(string[] args)
        {
            string[] contents = File.ReadAllLines(path);

            vec3i prev = default;

            int max_component_delta = 0;

            MemoryStream stream = new MemoryStream(contents.Length * 3);
            int count = contents.Length;

            int size_before = 0;
            int print = 1000;
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

                vec3i delta = vec - prev;
                prev = vec;

                if (i < print)
                    Console.WriteLine(delta);

                if (i > 0)
                {
                    max_component_delta = Math.Max(max_component_delta, delta.x);
                    max_component_delta = Math.Max(max_component_delta, delta.y);
                    max_component_delta = Math.Max(max_component_delta, delta.z);
                }

                bool useDouble = false;

                int dx = ZigZagEncode(delta.x);
                int dy = ZigZagEncode(delta.y);
                int dz = ZigZagEncode(delta.z);

                if (dx > 255 || dy > 255 || dz > 255)
                {
                    flags |= 4;
                    useDouble = true;
                }

                stream.WriteByte(flags);
                size_before += 1;

                if (!useDouble)
                {
                    stream.WriteByte(Convert.ToByte(dx));
                    stream.WriteByte(Convert.ToByte(dy));
                    stream.WriteByte(Convert.ToByte(dz));
                    size_before += 3;
                }
                else
                {
                    // write short
                    stream.WriteByte(Convert.ToByte(dx & 0xFF));
                    stream.WriteByte(Convert.ToByte((dx >> 8) & 0xFF));
                    stream.WriteByte(Convert.ToByte(dy & 0xFF));
                    stream.WriteByte(Convert.ToByte((dy >> 8) & 0xFF));
                    stream.WriteByte(Convert.ToByte(dz & 0xFF));
                    stream.WriteByte(Convert.ToByte((dz >> 8) & 0xFF));
                    size_before += 6;
                }

                if (isSolid)
                {
                    stream.WriteByte((byte)color_r);
                    stream.WriteByte((byte)color_g);
                    stream.WriteByte((byte)color_b);
                    size_before += 3;
                }

                if (damaged)
                {
                    stream.WriteByte((byte)damaged_value);
                    size_before += 1;
                }
            }

            Console.WriteLine($"max component delta: {max_component_delta}");

            stream.Flush();
            byte[] buf = stream.GetBuffer();

            Stopwatch sw = Stopwatch.StartNew();
            byte[] compressed = CompressBuffer(buf);
            sw.Stop();

            int compressed_length = compressed.Length;

            Console.WriteLine($"count: {count}," +
                $"size before compression: {ByteAmount.FromBytes(size_before)}," +
                $"size after compression: {ByteAmount.FromBytes(compressed_length)}, " +
                $"took: {(int)(sw.Elapsed.TotalSeconds * 1000f)} ms");

            Console.ReadLine();
        }

        private static byte[] CompressBuffer(byte[] buffer)
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

    }
}
