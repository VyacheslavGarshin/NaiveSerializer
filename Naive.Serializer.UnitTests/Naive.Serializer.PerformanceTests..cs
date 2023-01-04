using FluentAssertions;
using Newtonsoft.Json;
using Salar.Bois;
using System.Diagnostics;
using System.Text;

namespace Naive.Serializer.UnitTests
{
    public class NaiveSerializerPerformanceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(TestPerformanceCases))]
        public void TestPerformance(int count, string name, object obj)
        {
            var boisSerializer = new BoisSerializer();

            Console.WriteLine($"Count: {count}, object: {obj}");

            var sw = Stopwatch.StartNew();
            byte[] bytes = null;
            for (var i = 0; i < count; i++)
            {
                bytes = NaiveSerializer.Serialize(obj);
            }
            sw.Stop();
            Console.WriteLine($"Naive serialize time: {sw.Elapsed.TotalMilliseconds}, bytes: {bytes.Length}");

            sw.Restart();
            byte[] jBytes = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    jBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, new JsonSerializerSettings()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Json failed serialize {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Json serialize time: {sw.Elapsed.TotalMilliseconds}, bytes: {jBytes?.Length}");

            sw.Restart();
            byte[] bBytes = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    using var ms = new MemoryStream();
                    boisSerializer.Serialize(obj, obj.GetType(), ms);
                    bBytes = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bois failed serialize {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Bois serialize time: {sw.Elapsed.TotalMilliseconds}, bytes: {bBytes?.Length}");

            sw.Restart();
            object objD = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    objD = NaiveSerializer.Deserialize(bytes, obj?.GetType());
                }

                try
                {
                    objD.Should().BeEquivalentTo(obj);
                }
                catch
                {
                    Console.WriteLine($"Naive failed check");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Naive failed deserialize {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Naive deserialize time: {sw.Elapsed.TotalMilliseconds}");

            sw.Restart();
            object jObjD = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    jObjD = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(jBytes), obj?.GetType());
                }

                try
                {
                    jObjD.Should().BeEquivalentTo(obj);
                }
                catch
                {
                    Console.WriteLine($"Json failed check");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Json failed deserialize {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Json deserialize time: {sw.Elapsed.TotalMilliseconds}");

            sw.Restart();
            object bObjD = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    using var ms = new MemoryStream(bBytes);
                    bObjD = boisSerializer.Deserialize(ms, obj.GetType());
                }
                try
                {
                    bObjD.Should().BeEquivalentTo(obj);
                }
                catch
                {
                    Console.WriteLine($"Bois failed check");
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bois failed deserialize {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Bois deserialize time: {sw.Elapsed.TotalMilliseconds}");

            sw.Restart();
            object objDRom = null;
            try
            {
                for (var i = 0; i < count; i++)
                {
                    objDRom = NaiveSerializer.Deserialize(new ReadOnlyMemory<byte>(bytes), obj?.GetType());
                }

                try
                {
                    objDRom.Should().BeEquivalentTo(obj);
                }
                catch
                {
                    Console.WriteLine($"Naive Rom failed check");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Naive Rom failed deserialize {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Naive Rom deserialize time: {sw.Elapsed.TotalMilliseconds}");

            Console.WriteLine("Naive bytes: " + string.Join(",", bytes.Select(x => x.ToString())));
            Console.WriteLine("Json bytes: " + Encoding.UTF8.GetString(jBytes?.ToArray() ?? Array.Empty<byte>()));
            Console.WriteLine("Bois bytes: " + string.Join(",", (bBytes ?? Array.Empty<byte>()).Select(x => x.ToString())));
        }

        static object[] TestPerformanceCases =
        {
            new []{ 10000, "null", (object)null },
            new []{ 10000, "1", (object)1 },
            new []{ 10000, "string", (object)"**********" },
            new []{ 10000, "datetime", (object)new DateTime(1000, 1, 1) },
            new []{ 10000, "timespan", (object)TimeSpan.FromMinutes(1) },
            new []{ 10000, "guid", (object)Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}") },
            new []{ 10000, "list string", (object)new List<string> { "*", null, "*", "*", "*", "*", "*", "*", "*", "*" } },
            new []{ 10000, "array string", (object)new string[] { "*", null, "*", "*", "*", "*", "*", "*", "*", "*" } },
            new []{ 10000, "array byte?", (object)new byte?[] { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new []{ 10000, "array byte", (object)new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new []{ 10000, "read only memory byte", (object)new ReadOnlyMemory<byte>(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }) },
            new []{ 10000, "array objects", (object)new object[] { 0, "*", true, 1L, new DateTime(1, 1, 1) } },
            new []{ 10000, "dictionary", (object)new Dictionary<int, string> { { 1, "*" }, { 2, null }, { 3, "*" }, { 4, "*" }, { 5, "*" }, { 6, "*" }, { 7, "*" }, { 8, "*" }, { 9, "*" }, { 10, "*" } } },
            new []{ 10000, "ienumerable", (object)new int?[] { null, 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Select(x => x) },
            new []{ 10000, "plain object", (object)new PlainObject { PObject = new(), PObjects = new PlainObject []{ new() } } },
            new []{ 10000, "plain struct", (object)new PlainStruct { PlainStructs = new PlainStruct[] { new() } }, },
            new []{ 10000, "plain object[10]", (object)Enumerable.Range(0, 10).Select(x => new PlainObject()).ToArray() },
            new []{ 10, "plain object[10000]", (object)Enumerable.Range(0, 10000).Select(x => new PlainObject()).ToArray() }
        };

        public class PlainObject
        {
            public Guid Guid { get; set; } = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}");

            public int Int { get; set; } = 1;

            public string String { get; set; } = "String";

            public string[] Strings { get; set; } = Enumerable.Range(0, 10).Select(x => "*").ToArray();

            public PlainObject PObject { get; set; }

            public PlainObject[] PObjects { get; set; }
        }

        public struct PlainStruct
        {
            public Guid Guid { get; set; } = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}");

            public int Int { get; set; } = 1;

            public string String { get; set; } = "String";

            public string[] Strings { get; set; } = Enumerable.Range(0, 10).Select(x => "*").ToArray();

            public PlainStruct[] PlainStructs { get; set; }

            public PlainStruct()
            {
            }
        }
    }
}