using FluentAssertions;
using Naive.Serializer.Cogs;

namespace Naive.Serializer.UnitTests
{
    public class CogsBytesComparerUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(TestCases))]
        public void Test(byte[] arrayA, byte[] arrayB, bool result)
        {
            var comparator = new BytesComparer();

            comparator.Equals(arrayA, arrayB).Should().Be(result);
        }

        private static object[] TestCases = new object[]
        {
            new object[] { null, null, true },
            new object[] { new byte[1] { 1 }, new byte[1] { 1 }, true },
            new object[] { new byte[1] { 1 }, new byte[2] { 1, 2 }, false },
            new object[] { new byte[2] { 1, 2 }, new byte[2] { 1, 3 }, false },
        };
    }
}