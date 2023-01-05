using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using MessagePack;
using Naive.Serializer;
using Newtonsoft.Json;
using Salar.Bois;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var primitivesSummary = BenchmarkRunner.Run<PrimitivesBenchmark>();
        var smallSummary = BenchmarkRunner.Run<SmallBenchmark>();
        var bigSummary = BenchmarkRunner.Run<BigBenchmark>();
    }
}

[SimpleJob(runStrategy: RunStrategy.Throughput, warmupCount: 1, launchCount: 1, invocationCount: 1000)]
public class PrimitivesBenchmark : Benchmark
{
    public PrimitivesBenchmark()
    {
        ToSerialize = new List<object[]>
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
            new object[] { "Serialize", "Guid",  Guid.Parse("{6F9619FF-8B86-D011-B42D-00CF4FC964FF}") },
            new object[] { "Serialize", "Enum", DateTimeKind.Utc },
            new object[] { "Serialize", "Byte[](10)", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 } },
        };
    }
}

[SimpleJob(runStrategy: RunStrategy.Throughput, warmupCount: 1, launchCount: 1, invocationCount: 1000)]
public class SmallBenchmark : Benchmark
{
    public SmallBenchmark()
    {
        ToSerialize = new List<object[]>
        {
            new object[] { "Serialize", "Byte?[](10)", new byte?[] { 0, null, 2, null, 4, null, 6, null, 8, null } },
            new object[] { "Serialize", "String[](20)", Enumerable.Range(0, 20).Select(x => "*").ToArray() },
            new object[] { "Serialize", "List<String>(20)", Enumerable.Range(0, 20).Select(x => "*").ToList() },
            new object[] { "Serialize", "List<Byte[]>(10^2)", Enumerable.Range(0, 10).Select(x => Enumerable.Range(0, 10).Select(x => (byte)x).ToArray()).ToList() },
            new object[] { "Serialize", "PlainClass",  new PlainClass() },
            new object[] { "Serialize", "PlainClass[](5)", Enumerable.Range(0, 5).Select(x => new PlainClass()).ToArray() },
            new object[] { "Serialize", "Object[](10)", new object[] { 0, null, "*", new DateTime(1000, 1, 1), new PlainClass(), Guid.NewGuid(), true, 7f, (byte)8, DateTimeKind.Utc } },
            new object[] { "Serialize", "Dic<int,string>(10)", Enumerable.Range(0, 10).Select((x, i) => i).ToDictionary((x) => x, (x) => x.ToString()) },
        };
    }
}

[SimpleJob(runStrategy: RunStrategy.Throughput, warmupCount: 1, launchCount: 1, invocationCount: 10)]
public class BigBenchmark : Benchmark
{
    public BigBenchmark()
    {
        ToSerialize = new List<object[]>
        {
            new object[] { "Serialize", "PlainClass[](10000)", Enumerable.Range(0, 10000).Select(x => new PlainClass()).ToArray() },
            new object[] { "Serialize", "List<Byte[]>(1000^2)", Enumerable.Range(0, 1000).Select(x => Enumerable.Range(0, 1000).Select(x => (byte)x).ToArray()).ToList() },
        };
    }
}

public class Benchmark
{
    private readonly BoisSerializer _boisSerializer = new ();

    protected List<object[]> ToSerialize = new();

    private List<object[]> _toDeserialize = new();

    public Benchmark()
    {       
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
    [ArgumentsSource(nameof(SerializeArguments))]
    public byte[] MsgPack(string method, string name, object value) => MessagePackSerializer.Serialize(value);
    
    [Benchmark]
    [ArgumentsSource(nameof(NaiveDeserializeArguments))]
    public object Naive(string method, string name, byte[] value, Type type) => NaiveSerializer.Deserialize(value, type);

    [Benchmark]
    [ArgumentsSource(nameof(JsonDeserializeArguments))]
    public object Json(string method, string name, byte[] value, Type type) => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), type);

    [Benchmark]
    [ArgumentsSource(nameof(BoisDeserializeArguments))]
    public object Bois(string method, string name, byte[] value, Type type) => BoisDeserialize(value, type);

    [Benchmark]
    [ArgumentsSource(nameof(MsgPackDeserializeArguments))]
    public object MsgPack(string method, string name, byte[] value, Type type) => MsgPackDeserialize(value, type);
    
    public IEnumerable<object[]> SerializeArguments()
    {
        return ToSerialize;
    }

    public IEnumerable<object[]> NaiveDeserializeArguments()
    {
        return DeserializeArguments((obj, type) => NaiveSerializer.Serialize(obj));       
    }

    public IEnumerable<object[]> JsonDeserializeArguments()
    {
        return DeserializeArguments((obj, type) => JsonSerialize(obj));        
    }

    public IEnumerable<object[]> BoisDeserializeArguments()
    {
        return DeserializeArguments((obj, type) => BoisSerialize(obj));
    }

    public IEnumerable<object[]> MsgPackDeserializeArguments()
    {
        return DeserializeArguments((obj, type) => MessagePackSerializer.Serialize(type, obj));
    }

    public IEnumerable<object[]> DeserializeArguments(Func<object, Type, byte[]> func)
    {
        var result = new List<object[]>();

        foreach (var item in ToSerialize)
        {
            try
            {
                var bytes = func(item[2], item[2]?.GetType());
                result.Add(new object[] { "Deserialize", item[1], bytes, item[2]?.GetType() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Serialization '{item[1]}' error: {ex.Message}");
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

    private object MsgPackDeserialize(byte[] bytes, Type type)
    {
        using var ms = new MemoryStream(bytes);
        return MessagePackSerializer.Deserialize(type, ms);
    }

    [MessagePackObject]
    public class PlainClass
    {
        [Key("Id")]
        public int Id { get; set; } = 1;

        [Key("Name")]
        public string Name { get; set; } = "Name";

        [Key("Description")]
        public string Description { get; set; } = "Description";

        [Key("DateTime")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Key("Bytes")]
        public byte[] Bytes { get; set; } = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }
}
