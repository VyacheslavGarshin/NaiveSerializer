using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace NaiveSerializer.UnitTests
{
    public class NaiveSerializerUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNull()
        {
            ThereAndBack(null);
        }

        [TestCase(null)]
        [TestCase(1)]
        [TestCase(-1)]
        public void TestSByte(sbyte? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1)]
        public void TestByte(byte? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1)]
        [TestCase(-1)]
        public void TestShort(short? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase((ushort)1)]
        public void TestUShort(ushort? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1)]
        [TestCase(-1)]
        public void TestInt(int? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase((uint)1)]
        public void TestUInt(uint? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1)]
        [TestCase(-1)]
        public void TestLong(long? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1UL)]
        public void TestULong(ulong? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1f)]
        [TestCase(-1f)]
        public void TestFloat(float? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1)]
        [TestCase(-1)]
        public void TestDouble(double? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(1)]
        [TestCase(-1)]
        public void TestDecimal(decimal? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase(true)]
        [TestCase(false)]
        public void TestBool(bool? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase('a')]
        public void TestChar(char? value)
        {
            ThereAndBack(value);
        }

        [TestCase(null)]
        [TestCase("A")]
        [TestCase("")]
        public void TestString(string value)
        {
            ThereAndBack(value);
        }

        [TestCaseSource(nameof(TestTimeSpanCases))]
        public void TestTimeSpan(TimeSpan? value)
        {
            ThereAndBack(value);
        }

        static object[] TestTimeSpanCases =
        {
            new []{ (TimeSpan?)null },
            new []{ TimeSpan.FromMinutes(1) },
        };

        [TestCaseSource(nameof(TestDateTimeCases))]
        public void TestDateTime(DateTime? value)
        {
            ThereAndBack(value);
        }

        static object[] TestDateTimeCases =
        {
            new []{ (DateTime?)null },
            new []{ DateTime.MinValue },
            new []{ new DateTime(1000, 1, 1) },
        };

        [TestCaseSource(nameof(TestDateTimeOffsetCases))]
        public void TestDateTimeOffset(DateTimeOffset? value)
        {
            ThereAndBack(value);
        }

        static object[] TestDateTimeOffsetCases =
        {
            new []{ (DateTimeOffset?)null },
            new []{ DateTimeOffset.MinValue },
            new []{ new DateTimeOffset(DateTime.MinValue, TimeSpan.FromHours(-1)) },
        };

        [TestCaseSource(nameof(TestGuidCases))]
        public void TestGuid(Guid? value)
        {
            ThereAndBack(value);
        }

        static object[] TestGuidCases =
        {
            new []{ (Guid?)null },
            new []{ Guid.Empty },
            new []{ Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}") },
        };

        [TestCase(null)]
        [TestCase(DateTimeKind.Utc)]
        public void TestEnum(DateTimeKind? value)
        {
            ThereAndBack(value);
        }

        [TestCaseSource(nameof(TestIListCases))]
        public void TestIList(object value)
        {
            ThereAndBack(value);
        }

        static object[] TestIListCases =
        {
            new []{ new List<int?> { null, 1, 2 } },
            new []{ new List<string> { null, "AAA", string.Empty } },
            new []{ new string[] { null, "AAA", string.Empty } },
        };

        [TestCaseSource(nameof(TestObjectCases))]
        public void TestObject(object value)
        {
            ThereAndBack(value);
        }

        private static void ThereAndBack(object value)
        {
            using var stream = new MemoryStream();
            NaiveSerializer.Serialize(value, stream);
            stream.Position = 0;

            var objD = NaiveSerializer.Deserialize(stream, value?.GetType());

            objD.Should().BeEquivalentTo(value);
            stream.Position.Should().Be(stream.Length);
        }

        static object[] TestObjectCases =
        {
            new []{ new A {
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 5,
            } },
        };

        public class A
        {
            [IgnoreDataMember]
            public Guid GuidIgnored { get; set; }

            public Guid Guid { get; set; }
            
            public int Int { get; set; }
        }
    }
}