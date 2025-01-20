namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();

            Assert.IsTrue(1 == 1);
        }

        [Test]
        public void test()
        {
            int a = 5;
            int b = 7;
            Assert.IsTrue(a + b == 12);
        }
    }
}