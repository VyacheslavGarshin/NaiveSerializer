using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Naive.Serializer;
using Newtonsoft.Json;
using Salar.Bois;
using System.Text;

[SimpleJob(runStrategy: RunStrategy.Throughput, warmupCount: 1, launchCount: 1, invocationCount: 1000)]
public class Benchmark
{
    private readonly BoisSerializer _boisSerializer = new ();

    private List<object[]> _serialize = new();

    private List<object[]> _deserialize = new();

    public Benchmark()
    {
        _serialize = new List<object[]>
        {
            new object[] { "Serialize", "Null", null },
            new object[] { "Serialize", "Bool", true },
            new object[] { "Serialize", "Int(1)", 1 },
            new object[] { "Serialize", "Int(MaxValue)", int.MaxValue },
            new object[] { "Serialize", "Float(1)", 1f },
            new object[] { "Serialize", "Float(MaxValue)", float.MaxValue },
            new object[] { "Serialize", "String(10)", string.Join("", Enumerable.Range(0, 10).Select(x => "*")) },
            new object[] { "Serialize", "DateTime", new DateTime(1000, 1, 1) },
            new object[] { "Serialize", "TimeSpan", new TimeSpan(10, 10, 10) },
            new object[] { "Serialize", "Guid", Guid.Empty },
            new object[] { "Serialize", "Enum", DateTimeKind.Utc },
            new object[] { "Serialize", "Byte[](10)", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
            new object[] { "Serialize", "Byte?[](10)", new byte?[] { 0, null, 2, null, 4, null, 6, null, 8, null } },
            new object[] { "Serialize", "String[](10)", Enumerable.Range(0, 10).Select(x => "*").ToArray() },
            new object[] { "Serialize", "List<String>(10)", Enumerable.Range(0, 10).Select(x => "*").ToList() },
            new object[] { "Serialize", "PlainClass[](10)",  Enumerable.Range(0, 10).Select(x => new PlainClass()).ToArray() },
            new object[] { "Serialize", "Object[](10)", new object[] { 0, null, "*", new DateTime(1000, 1, 1), new PlainClass(), Guid.NewGuid(), true, 7f, (byte)8, DateTimeKind.Utc } },
            new object[] { "Serialize", "Dictionary<int,string>(10)", Enumerable.Range(0, 10).Select((x, i) => i).ToDictionary((x) => x, (x) => x.ToString()) },
        };
    }

    [Benchmark]
    [ArgumentsSource(nameof(SerializeArguments))]
    public byte[] Naive(string method, string name, object value) => NaiveSerializer.Serialize(value);

    [Benchmark]
    [ArgumentsSource(nameof(SerializeArguments))]
    public byte[] Json(string method, string name, object value) => JsonSerialize(value);

    [Benchmark]
    [ArgumentsSource(nameof(SerializeArguments))]
    public byte[] Bois(string method, string name, object value) => BoisSerialize(value);

    [Benchmark]
    [ArgumentsSource(nameof(NaiveDeserializeArguments))]
    public object Naive(string method, string name, byte[] value, Type type) => NaiveSerializer.Deserialize(value, type);

    [Benchmark]
    [ArgumentsSource(nameof(JsonDeserializeArguments))]
    public object Json(string method, string name, byte[] value, Type type) => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), type);

    [Benchmark]
    [ArgumentsSource(nameof(BoisDeserializeArguments))]
    public object Bois(string method, string name, byte[] value, Type type) => BoisDeserialize(value, type);

    public IEnumerable<object[]> SerializeArguments()
    {
        return _serialize;
    }

    public IEnumerable<object[]> NaiveDeserializeArguments()
    {
        var result = new List<object[]>();

        foreach (var item in _serialize)
        {
            try
            {
                var bytes = NaiveSerializer.Serialize(item[2]);
                result.Add(new object[] { "Deserialize", item[1], bytes, item[2]?.GetType() });
            }
            catch
            {
                result.Add(new object[] { "Deserialize", item[1], null, null });
            }
        }

        return result;
    }

    public IEnumerable<object[]> JsonDeserializeArguments()
    {
        var result = new List<object[]>();

        foreach (var item in _serialize)
        {
            try
            {
                var bytes = JsonSerialize(item[2]);
                result.Add(new object[] { "Deserialize", item[1], bytes, item[2]?.GetType() });
            }
            catch
            {
                result.Add(new object[] { "Deserialize", item[1], null, null });
            }
        }

        return result;
    }

    public IEnumerable<object[]> BoisDeserializeArguments()
    {
        var result = new List<object[]>();

        foreach (var item in _serialize)
        {
            try
            {
                var bytes = BoisSerialize(item[2]);
                result.Add(new object[] { "Deserialize", item[1], bytes, item[2]?.GetType() });
            }
            catch(Exception ex)
            {
                result.Add(new object[] { "Deserialize", item[1], null, null });
            }
        }

        return result;
    }

    private byte[] JsonSerialize(object obj)
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
    }

    private byte[] BoisSerialize(object obj)
    {
        using var ms = new MemoryStream();
        _boisSerializer.Serialize(obj, obj?.GetType(), ms);
        return ms.ToArray();
    }

    private object BoisDeserialize(byte[] bytes, Type type)
    {
        using var ms = new MemoryStream(bytes);
        return _boisSerializer.Deserialize(ms, type);
    }

    public class PlainClass
    {
        public int Id { get; set; } = 1;

        public string Name { get; set; } = "Name";

        public string Description { get; set; } = "Description";

        public DateTime DateTime { get; set; } = DateTime.Now;

        public byte[] Bytes { get; set; } = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchmark>();
    }
}
