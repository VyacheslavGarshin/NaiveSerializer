NaiveSerializer
===============

.NET Standard binary serialization library.

Features:
+ Doesn't requere changing of serialized class or adding anything
+ Resilient to changes in serialized/deserialized classes
+ Deserialize unknown classes to dictionary
+ Respects `DataContract` attributes

Plans:
+ Speed improovments

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

Benchmark results are [here](https://github.com/VyacheslavGarshin/NaiveSerializer/tree/main/Naive.Serializer.Benchmark/Results).