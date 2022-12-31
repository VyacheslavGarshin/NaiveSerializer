using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace NaiveSerializer.UnitTests
{
    public class NaiveSerializerPerformanceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(TestPerformanceCases))]
        public void TestPerformance(int count, object obj)
        {
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
                jBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            }
            sw.Stop();
            Console.WriteLine($"Json serialize time: {sw.Elapsed.TotalMilliseconds}, bytes: {jBytes.Length}");

            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                var objD = NaiveSerializer.Deserialize(bytes, obj?.GetType());
            }
            sw.Stop();            
            Console.WriteLine($"Naive deserialize time: {sw.Elapsed.TotalMilliseconds}");

            sw.Restart();
            for (var i = 0; i < count; i++)
            {
                var objD = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(jBytes), obj?.GetType());
            }
            sw.Stop();
            Console.WriteLine($"Json deserialize time: {sw.Elapsed.TotalMilliseconds}");
        }

        static object[] TestPerformanceCases =
        {
            new []{ 100000, (object)null },
            new []{ 100000, (object)1 },
            new []{ 100000, (object)"small string" },
            new []{ 100000, (object)new DateTime(1000, 1, 1) },
            new []{ 100000, (object)TimeSpan.MinValue },
            new []{ 100000, (object)Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}") },
            new []{ 100000, (object)new List<string> { null, "AAA", "BBB", string.Empty } },
            new []{ 100000, (object)new string[] { null, "AAA", "BBB", string.Empty } },
            new []{ 100000, (object)new byte?[] { null, 1, 2, 3, 4, 5, 6 } },
        };
    }
}