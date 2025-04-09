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
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using VoxelPacker;
using VoxelPacker.def;

namespace TestProject1
{
    public class PackTests
    {
        [SetUp]
        public void Setup()
        {
        }

        // added this cus was getting IndexOutOfRangeException after adding
        // packing of color alphas
        [Test]
        public void test_single_voxel_pack_with_alpha()
        {
            List<voxeldata> list = new List<voxeldata>();
            list.Add(new voxeldata { damagedValue = 3, flags = voxelflags.Damaged | voxelflags.Placed, position = new vec3i(270, 25, 168), _color = new color { r = 34, g = 42, b = 26, a = 255 } });
            byte[] buf = Packer.Pack(list);
            var outlist = Packer.Unpack(buf);

            Assert.AreEqual(list.Count, outlist.Count);
            Assert.AreEqual(list[0]._color.a, outlist[0]._color.a);
        }

        [Test]
        public void random_test()
        {
            List<voxeldata> list = new List<voxeldata>();

            vec3i pos = new vec3i(0, 0, 0);

            for (int i = 0; i < 100; i++)
            {
                voxeldata basepos = new voxeldata();
                basepos.position = pos;
                list.Add(basepos);
                pos.z++;
            }

            byte[] bytes = Packer.Pack(list);
            Console.WriteLine(bytes.Length);

        }

        [Test]
        public void TestPackUnpackWorks_1()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "testdata_1");
            test_at_path(path);


            test_at_path(path);
        }

        [Test]
        public void TestPackUnpackWorksWithSmallBuffer()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "destroy_single_voxel");
            test_at_path(path);
        }

        private void test_at_path(string path)
        {
            List<voxeldata> datas = TestHelper.parse(path);

            Stopwatch sw = Stopwatch.StartNew();
            byte[] buffer = Packer.Pack(datas);
            sw.Stop();
            Console.WriteLine($"packing took {sw.Elapsed.TotalSeconds} s");
            List<voxeldata> out_list = Packer.Unpack(buffer);

            TestUtil.AssertListsEqual<voxeldata>(datas, out_list);
        }




        [Test]
        public void benchmark()
        {
            const string filename = "testdata_1";
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), filename);

            if (!File.Exists(path))
                throw new FileNotFoundException();

            List<voxeldata> datas = TestHelper.parse(path);

            byte[] outbuf = new byte[1024 * 1024 * 10];
            CompressedOutputStream ostream = new CompressedOutputStream(outbuf, 0, outbuf.Length);

            vec3i prevpos = default;
            for (int i = 0; i < datas.Count; i++)
            {
                var curr = datas[i];

                ModifiedVoxel mw = new ModifiedVoxel(curr.position, Color32ToColorInt(curr._color),
                    (curr.flags & voxelflags.Placed) != 0,
                    (curr.flags & voxelflags.Floating) != 0,
                    false,
                    ((byte)curr.damagedValue));

                mw.Serialize(ref ostream, prevpos);
                prevpos = mw.WorldPosition;
            }

            int bytes = ostream.Flush();
            float original_bpv = bytes * 8 / (float)datas.Count;

            byte[] buffer = Packer.Pack(datas);
            float new_bpv = buffer.Length * 8 / (float)datas.Count;

            // Calculate percentage ratio for the new solution.
            float percentage = (buffer.Length * 100f) / bytes;

            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            Console.WriteLine($"Voxel count: {datas.Count}\n");

            // Updated table header with additional "Percentage" column.
            Console.WriteLine("{0,-25} {1,-20} {2,-15} {3,-15}", "Solution", "Bytes", "Bits Per Voxel", "Percentage");
            Console.WriteLine(new string('-', 80));

            // Original solution (100% usage)
            Console.WriteLine("{0,-25} {1,-20} {2,-15:F1} {3,-15}",
                "Original solution", ByteAmount.FromBytes(bytes), original_bpv, "100%");

            // New solution with calculated percentage.
            Console.WriteLine("{0,-25} {1,-20} {2,-15:F1} {3,-15}",
                "New solution", ByteAmount.FromBytes(buffer.Length), new_bpv, $"{percentage:F0}%");

        }

        private static int Color32ToColorInt(color _color)
        {
            return _color.r << 16 | _color.g << 8 | _color.b;
        }
    }
}