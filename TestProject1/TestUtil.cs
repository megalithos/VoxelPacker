using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoxelPacker;
using VoxelPacker.def;

namespace TestProject1
{
    internal static class TestUtil
    {
        public static void AssertListsEqual<T>(IList<T> list1, IList<T> list2) where T : struct, IEquatable<T>
        {
            if (list1 == null || list2 == null)
                throw new ArgumentNullException();

            if (list1.Count != list2.Count)
                throw new InvalidOperationException("List lengths do not match.");

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    throw new InvalidOperationException($"Element mismatch at index {i}.");
            }
        }
    }
}
