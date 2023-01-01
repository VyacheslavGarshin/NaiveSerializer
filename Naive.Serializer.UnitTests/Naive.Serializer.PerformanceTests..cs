using Newtonsoft.Json;
using Salar.Bois;
using System.Diagnostics;
using System.IO;
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
            for (var i = 0; i < count; i++)
            {
                jBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, new JsonSerializerSettings()));
            }
            sw.Stop();
            Console.WriteLine($"Json serialize time: {sw.Elapsed.TotalMilliseconds}, bytes: {jBytes.Length}");

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
                Console.WriteLine($"Bois failed {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Bois serialize time: {sw.Elapsed.TotalMilliseconds}, bytes: {bBytes?.Length}");

            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                NaiveSerializer.Deserialize(bytes, obj?.GetType());
            }
            sw.Stop();
            Console.WriteLine($"Naive deserialize time: {sw.Elapsed.TotalMilliseconds}");

            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                JsonConvert.DeserializeObject(Encoding.UTF8.GetString(jBytes), obj?.GetType());
            }
            sw.Stop();
            Console.WriteLine($"Json deserialize time: {sw.Elapsed.TotalMilliseconds}");

            sw.Restart();
            try
            {
                for (var i = 0; i < count; i++)
                {
                    using var ms = new MemoryStream(bBytes);
                    boisSerializer.Deserialize(ms, obj.GetType());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bois failed {ex.GetBaseException().Message}");
            }
            sw.Stop();
            Console.WriteLine($"Bois deserialize time: {sw.Elapsed.TotalMilliseconds}");

            Console.WriteLine("Naive bytes: " + Encoding.UTF8.GetString(bytes?.ToArray() ?? Array.Empty<byte>()));
            Console.WriteLine("Json bytes: " + Encoding.UTF8.GetString(jBytes?.ToArray() ?? Array.Empty<byte>()));
            Console.WriteLine("Bois bytes: " + Encoding.UTF8.GetString(bBytes?.ToArray() ?? Array.Empty<byte>()));
        }

        static object[] TestPerformanceCases =
        {
            new []{ 100000, "null", (object)null },
            new []{ 100000, "1", (object)1 },
            new []{ 100000, "string", (object)"small string" },
            new []{ 100000, "datetime", (object)new DateTime(1000, 1, 1) },
            new []{ 100000, "timespan", (object)TimeSpan.MinValue },
            new []{ 100000, "guid", (object)Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}") },
            new []{ 100000, "list string", (object)new List<string> { null, "AAA", "BBB", string.Empty } },
            new []{ 100000, "array string", (object)new string[] { null, "AAA", "BBB", string.Empty } },
            new []{ 100000, "array byte?", (object)new byte?[] { null, 1, 2, 3, 4, 5, 6 } },
            new []{ 100000, "array byte", (object)new byte[] { 0, 1, 2, 3, 4, 5, 6 } },
            new []{ 100000, "plain object", (object)new PlainObject {
                Guid = Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}"),
                Int = 10,
                String = "Some small string",
                Strings = new string[] { "1", "", null, "some small string a very good indeed" },
                PObject = new () { Int = 1 },
                PObjects = new PlainObject[] { new (), new() { Guid = Guid.NewGuid() } } // null - bois fails
            } },
        };

        public class PlainObject
        {
            public Guid Guid { get; set; }

            public int Int { get; set; }

            public string String { get; set; }

            public string[] Strings { get; set; }

            public PlainObject PObject { get; set; }

            public PlainObject[] PObjects { get; set; }
        }
    }
}