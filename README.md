NaiveSerializer
===============

.NET Standard binary serialization library.

Features:
+ Doesn't requere changing of serialized class or adding anything.
+ Resilient to changes in serialized/deserialized classes.
+ Deserialize unknown payload.
+ Respects `DataContract` attributes.

Plans:
+ Speed improvements on large object lists.

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

Supported Types
===============

+ All simple types and structs from `System`. 
+ `IDictionary`, `IList`, any `IEnumerable`. 
+ Classes and structs with parameterless constructor.
+ If deserializing class property type is an interface `IEnumerable` then deserialize it as `Array<T>`.
+ If deserializing unknown payload then deserialize objects as `Dictionary<string, object>` and enumerables as arrays.

Installation
============

NuGet package is [here](https://www.nuget.org/packages/Naive.Serializer/).
```
> dotnet add package Naive.Serializer
```

Performance
===========

Couple of times better than NewtonJson, a bit slower than [Bois](https://github.com/salarcode/Bois).

Benchmark results are [here](https://github.com/VyacheslavGarshin/NaiveSerializer/tree/main/Naive.Serializer.Benchmark/Results).