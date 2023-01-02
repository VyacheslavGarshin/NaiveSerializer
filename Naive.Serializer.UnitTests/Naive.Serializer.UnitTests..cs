using FluentAssertions;
using System.Runtime.Serialization;
using System.Text;

namespace Naive.Serializer.UnitTests
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
            ThereAndBack(value, false);
            ThereAndBack(value, true, true, (int?)value);
        }

        [TestCaseSource(nameof(TestIListCases))]
        public void TestIList(string name, object value)
        {
            var result = ThereAndBack(value);
        }

        static object[] TestIListCases =
        {
            new object[]{ "byte[]", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new object[]{ "byte?[]", new byte?[] { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new object[]{ "list int?", new List<int?> { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new object[]{ "list string", new List<string> { null, string.Empty, "*", "*", "*", "*", "*", "*", "*", "*" } },
            new object[]{ "string[]", new string[] { null, string.Empty, "*", "*", "*", "*", "*", "*", "*", "*" } },
            new object[]{ "object[]", new object[] { null, 1, true, "AAA", 'c', 1L } },
            new object[]{ "hashset", new HashSet<int?> { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new object[]{ "ienumerable", new int?[] { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Select(x => x) },
        };

        [TestCaseSource(nameof(TestIDictionaryCases))]
        public void TestIDictionary(string name, object value)
        {
            ThereAndBack(value);
        }

        static object[] TestIDictionaryCases =
        {
            new object[]{ "IDictionary[]", new Dictionary<int, string> { { 1, "*" }, { 2, null }, { 3, "A" } } },
            new object[]{ "IDictionary[]", new Dictionary<object, object> { { 1, "*" }, { 2, null }, { 3, "A" } } },
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

        [TestCaseSource(nameof(TestObjectCases))]
        public void TestObject(string name, object value, bool check, Type deserializeType)
        {
            var result = ThereAndBack(value, true, false, null, deserializeType);
            result = ThereAndBack(value, false, check, null, deserializeType);
        }

        static object[] TestObjectCases =
         {
            new object[]{ "plain object", new PlainObject {
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 5,
                Ints = new int[] { 5, 1 },
                PObject = new () { Int = 1 , Ints = new int[] { 2 }, Strings = new [] { "A" } },
                PObjects = new PlainObject[] { new (), null }
            }, true, null },
            new object[]{ "plain struct", new PlainStruct { Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 5,
                Ints = new int[] { 5, 1 },
                PStructs = new PlainStruct?[] { new() { String = "*" }, null }
            }, false, null },
            new object[]{ "plain object to lesser one", new PlainObject {
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 5,
                Ints = new int[] { 5, 1 },
                PObject = new () { Int = 1 , Ints = new int[] { 2 }, Strings = new [] { "A" } },
                PObjects = new PlainObject[] { new (), null }
            }, false, typeof(PlainObjectLesser) },
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

        public class PlainObjectLesser
        {
            public Guid Guid { get; set; }

            public int Int { get; set; }

            public string String { get; set; }

            public PlainObject PObject { get; set; }
        }

        public struct PlainStruct
        {
            public Guid Guid { get; set; }

            public int Int { get; set; }

            public string String { get; set; }

            public int[] Ints { get; set; }

            public string[] Strings { get; set; }

            public PlainStruct?[] PStructs { get; set; }
        }

        private static object ThereAndBack(object value, bool? notyped = null, bool check = true, object alrernativeValue = null, Type deserializeType = null)
        {
            object result = null;

            using var stream = new MemoryStream();
            NaiveSerializer.Serialize(value, stream);

            Console.WriteLine($"Length: {stream.Length}");
            Console.WriteLine(string.Join(",", stream.ToArray().Select(x => x.ToString())));

            if (notyped ?? true)
            {
                stream.Position = 0;
                result = NaiveSerializer.Deserialize(stream);

                if (check)
                {
                    result.Should().BeEquivalentTo(alrernativeValue ?? value);
                }
            }

            if (notyped != true)
            {
                stream.Position = 0;
                result = NaiveSerializer.Deserialize(stream, deserializeType ?? value?.GetType());

                if (check)
                {
                    result.Should().BeEquivalentTo(alrernativeValue ?? value);
                }
            }

            stream.Position.Should().Be(stream.Length);

            return result;
        }
    }
}