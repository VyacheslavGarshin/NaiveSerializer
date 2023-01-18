using FluentAssertions;
using System.Runtime.Serialization;

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
        [TestCase(int.MaxValue)]
        [TestCase(int.MaxValue >> 16)]
        [TestCase(int.MinValue)]
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
        [TestCase(long.MaxValue >> 32)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
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

        static readonly object[] TestTimeSpanCases =
        {
            new []{ (TimeSpan?)null },
            new []{ TimeSpan.FromMinutes(1) },
        };

        [TestCaseSource(nameof(TestDateTimeCases))]
        public void TestDateTime(DateTime? value)
        {
            ThereAndBack(value);
        }

        static readonly object[] TestDateTimeCases =
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

        static readonly object[] TestDateTimeOffsetCases =
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

        static readonly object[] TestGuidCases =
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

        [TestCaseSource(nameof(TestBytesCases))]
        public void TestBytes(string name, object value, bool check)
        {
            var result = ThereAndBack(value, true, check);
        }

        static readonly object[] TestBytesCases =
        {
            new object[]{ "null", null, true },
            new object[]{ "byte[]", new byte[] { byte.MinValue, 1, 2, 3, 4, 5, 6, 7, 8, byte.MaxValue }, true },
            new object[]{ "read only memory", new ReadOnlyMemory<byte>(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), false },
        };

        [TestCaseSource(nameof(TestCharsCases))]
        public void TestChars(string name, object value)
        {
            var result = ThereAndBack(value);
        }

        static readonly object[] TestCharsCases =
        {
            new object[]{ "null", null },
            new object[]{ "char[]", new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' } },
        };

        [TestCaseSource(nameof(TestIEnumerableCases))]
        public void TestIEnumerable(string name, object value)
        {
            var result = ThereAndBack(value, checkTypedOnly: true);
        }

        static readonly object[] TestIEnumerableCases =
        {
            new object[]{ "byte?[]", new byte?[] { null, byte.MinValue, 2, 3, 4, 5, 6, 7, 8, byte.MaxValue } },
            new object[]{ "list byte", new List<byte> { byte.MinValue, 1, 2, 3, 4, 5, 6, 7, 8, byte.MaxValue } },
            new object[]{ "list byte?", new List<byte?> { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new object[]{ "list string", new List<string> { null, string.Empty, "*", "*", "*", "*", "*", "*", "*", "*" } },
            new object[]{ "string[]", new string[] { null, string.Empty, "*", "*", "*", "*", "*", "*", "*", "*" } },
            new object[]{ "object[]", new object[] { null, 1, true, "AAA", 'c', 1L } },
            new object[]{ "hashset", new HashSet<int?> { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new object[]{ "ienumerable", new int?[] { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Select(x => x) },
            new object[]{ "plain object[]", Enumerable.Range(0, 10).Select(x => new PlainObject()).ToArray() },
        };

        [TestCaseSource(nameof(TestIDictionaryCases))]
        public void TestIDictionary(string name, object value)
        {
            ThereAndBack(value);
        }

        static readonly object[] TestIDictionaryCases =
        {
            new object[]{ "IDictionary[]", new Dictionary<int, string> { { 1, "*" }, { 2, null }, { 3, "A" } } },
            new object[]{ "IDictionary[]", new Dictionary<object, object> { { 1, "*" }, { "", null }, { 3, "A" } } },
        };

        [TestCase(ReferenceLoopHandling.Error)]
        [TestCase(ReferenceLoopHandling.Ignore)]
        // [TestCase(ReferenceLoopHandling.Serialize)]
        public void TestObjectReferenceLoop(ReferenceLoopHandling handling)
        {
            var value = new PlainObject();
            value.PObject = value;

            var result = () => NaiveSerializer.Serialize(value, new NaiveSerializerOptions { ReferenceLoopHandling = handling });

            if (handling != ReferenceLoopHandling.Error)
            {
                var bytes = result.Should().NotThrow().Which;
                var des = NaiveSerializer.Deserialize<PlainObject>(bytes);

                des.PObject.Should().BeNull();
            }
            else
            {
                result.Should().Throw<ArgumentException>();
            }
        }

        [TestCaseSource(nameof(TestObjectWithoutContractCases))]
        public void TestObjectWithoutContract(ObjectWithoutContract value)
        {
            var result = (ObjectWithoutContract)ThereAndBack(value, false, false);

            result.GuidIgnored.Should().Be(Guid.Empty);
            result.Guid.Should().Be(value.Guid);
            result.Int.Should().Be(value.Int);
        }

        static readonly object[] TestObjectWithoutContractCases =
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

        static readonly object[] TestObjectWithContractCases =
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

        static readonly object[] TestObjectCases =
         {
            new object[]{ "plain object", new PlainObject {
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 5,
                Ints = new int[] { 5, 1 },
                PObject = new () { Int = 1 , Ints = new int[] { 2 }, Strings = new [] { "A" } },
                PObjects = new PlainObject[] { new (), null },
                IntField = 10,
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

            public int IntField;
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

        private static object ThereAndBack(object value, bool? notyped = null, bool check = true, object alrernativeValue = null, Type deserializeType = null, bool checkTypedOnly = false)
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

                if (check && !checkTypedOnly)
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