NaiveSerializer
===============

.NET Standard binary serialization library.

Features:
+ Doesn't requere changing of serialized class or adding anything
+ Resilient to changes in serialized/deserialized classes
+ Deserialize unknown classes to dictionary
+ Respects `DataContract` attributes

Plans:
+ Speed improovments on large list of objects

Usage
=====

```csharp
using var stream = new MemoryStream();
NaiveSerializer.Serialize(value, stream);

stream.Position = 0;
result = NaiveSerializer.Deserialize(stream, value.GetType());

// or 
var bytes = NaiveSerializer.Serialize(value);
result = NaiveSerializer.Deserialize(bytes, value.GetType());

// or 
result = NaiveSerializer.Deserialize<ValueType>(bytes);

// or unknown payload to object/dictionary
result = NaiveSerializer.Deserialize(bytes);
```

Performance
===========

Couple of times better than NewtonJson, a bit slower than [Bois](https://github.com/salarcode/Bois).

[Benchmark report](https://github.com/VyacheslavGarshin/NaiveSerializer/blob/main/SmallBenchmark-report.html) for small data.

[Benchmark report](https://github.com/VyacheslavGarshin/NaiveSerializer/blob/main/BigBenchmark-report.html) for big data.