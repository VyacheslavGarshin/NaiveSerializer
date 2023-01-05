using FluentAssertions;
using Naive.Serializer.Cogs;
using System.Text;

namespace Naive.Serializer.UnitTests
{
    public class CogsQuickTableUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestOneCharList()
        {
            var quickTable = new QuickTable<string>();
            var list = GetAllChars();

            foreach (var item in list)
            {
                quickTable.Add(Encoding.UTF8.GetBytes(item), item);
            }

            var count = quickTable.Count();

            count.Should().Be(list.Count);
        }

        [Test]
        public void TestTwoCharList()
        {
            var quickTable = new QuickTable<string>();
            var list = new List<string>();

            foreach (var first in GetAllChars())
            {
                foreach (var second in GetAllChars())
                {
                    list.Add(first + second);
                }
            }

            foreach (var item in list)
            {
                quickTable.Add(Encoding.UTF8.GetBytes(item), item);
            }

            var count = quickTable.Count();

            count.Should().Be(byte.MaxValue + 1);
        }

        private static List<string> GetAllChars()
        {
            var result = new List<string>();

            for (var i = '0'; i <= '9'; i++)
            {
                result.Add(i.ToString());
            }

            for (var i = 'A'; i <= 'Z'; i++)
            {
                result.Add(i.ToString());
            }

            for (var i = 'a'; i <= 'z'; i++)
            {
                result.Add(i.ToString());
            }

            return result;
        }
    }
}