using FluentAssertions;
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
            new []{ new object[] { null, 1, true, "AAA", 'c' } },
        };

        [TestCaseSource(nameof(TestObjectWithoutContractCases))]
        public void TestObjectWithoutContract(ObjectWithoutContract value)
        {
            var result = (ObjectWithoutContract)ThereAndBack(value, false, false);

            result.GuidIgnored.Should().Be(Guid.Empty);
            result.Guid.Should().Be(value.Guid);
            result.Int.Should().Be(value.Int);
        }
        
        static object[] TestObjectWithoutContractCases =
        {
            new []{ new ObjectWithoutContract {
                GuidIgnored = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 5,
            } },
        };

        public class ObjectWithoutContract
        {
            [IgnoreDataMember]
            public Guid GuidIgnored { get; set; }

            public Guid Guid { get; set; }

            public int Int { get; set; }
        }

        [TestCaseSource(nameof(TestObjectWithContractCases))]
        public void TestObjectWithContract(ObjectWithContract value)
        {
            var result = (ObjectWithContract)ThereAndBack(value, false, false);

            result.GuidIgnored.Should().Be(Guid.Empty);
            result.Guid.Should().Be(value.Guid);
            result.IntIgnored.Should().Be(0);
            result.Int.Should().Be(value.Int);
        }

        static object[] TestObjectWithContractCases =
         {
            new []{ new ObjectWithContract {
                GuidIgnored = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                IntIgnored = 5,
                Int = 5,
            } },
        };

        [DataContract]
        public class ObjectWithContract
        {
            [IgnoreDataMember]
            public Guid GuidIgnored { get; set; }

            [DataMember(Name = "G")]
            public Guid Guid { get; set; }

            public int IntIgnored { get; set; }

            [DataMember]
            public int Int { get; set; }
        }

        [TestCaseSource(nameof(TestPlainObjectCases))]
        public void TestPlainObject(PlainObject value)
        {
            var result = (PlainObject)ThereAndBack(value, false, false);
        }

        static object[] TestPlainObjectCases =
         {
            new []{ new PlainObject {
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 5,
                Ints = new int[] { 5, 1 },
                PObject= new () { Int = 1 , Ints = new int[] {} },
                PObjects= new PlainObject[] { new (), null }
            } },
        };

        public class PlainObject
        {
            public Guid Guid { get; set; }

            public int Int { get; set; }

            public string String { get; set; }

            public int[] Ints { get; set; }

            public string[] Strings { get; set; }

            public PlainObject PObject { get; set; }

            public PlainObject[] PObjects { get; set; }
        }

        private static object ThereAndBack(object value, bool check = true, bool notyped = true)
        {
            object result = null;

            using var stream = new MemoryStream();
            NaiveSerializer.Serialize(value, stream);

            if (notyped)
            {
                stream.Position = 0;
                result = NaiveSerializer.Deserialize(stream);
            }

            stream.Position = 0;
            result = NaiveSerializer.Deserialize(stream, value?.GetType());

            if (check)
            {
                result.Should().BeEquivalentTo(value);
            }

            stream.Position.Should().Be(stream.Length);

            return result;
        }
    }
}