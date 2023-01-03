NaiveSerializer
===============

.NET Standard binary serialization library.

Features:
+ Doesn't requere changing of serialized class or adding anything
+ Resilient to changes in serialized/deserialized classes
+ Deserialize unknown classes to dictionary
+ Respects `DataContract` attributes

Plans:
+ Remove payload redundancy on lists and dictionaries

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

``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 10 (10.0.19044.2364/21H2/November2021Update)
Intel Core i5-9400F CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2
  Job-NMULZF : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2

InvocationCount=1000  LaunchCount=1  RunStrategy=Throughput  
UnrollFactor=1  WarmupCount=1  

```
| Method |      method |                 name |                value |                 type |         Mean |      Error |     StdDev |       Median |
|------- |------------ |--------------------- |--------------------- |--------------------- |-------------:|-----------:|-----------:|-------------:|
|   **Bois** | **Deserialize** |                 **Bool** |              **Byte[1]** |       **System.Boolean** |    **107.75 ns** |   **1.881 ns** |   **1.571 ns** |    **108.30 ns** |
|  **Naive** | **Deserialize** |                 **Bool** |              **Byte[2]** |       **System.Boolean** |     **97.58 ns** |   **2.038 ns** |   **2.650 ns** |     **96.50 ns** |
|   **Json** | **Deserialize** |                 **Bool** |              **Byte[4]** |       **System.Boolean** |    **453.76 ns** |   **9.122 ns** |  **17.355 ns** |    **454.20 ns** |
|   **Bois** | **Deserialize** |          **Byte?[](10)** |             **Byte[11]** | **Syste(...)te][] [32]** |    **982.15 ns** |  **19.389 ns** |  **23.081 ns** |    **979.60 ns** |
|  **Naive** | **Deserialize** |          **Byte?[](10)** |             **Byte[20]** | **Syste(...)te][] [32]** |    **641.17 ns** |  **12.713 ns** |  **26.816 ns** |    **630.05 ns** |
|   **Json** | **Deserialize** |          **Byte?[](10)** |             **Byte[36]** | **Syste(...)te][] [32]** |  **2,623.02 ns** |  **40.074 ns** |  **35.525 ns** |  **2,619.90 ns** |
|   **Bois** | **Deserialize** |           **Byte[](10)** |             **Byte[11]** |        **System.Byte[]** |    **126.84 ns** |   **2.359 ns** |   **2.091 ns** |    **127.10 ns** |
|  **Naive** | **Deserialize** |           **Byte[](10)** |             **Byte[15]** |        **System.Byte[]** |    **145.98 ns** |   **2.924 ns** |   **4.286 ns** |    **145.60 ns** |
|   **Json** | **Deserialize** |           **Byte[](10)** |             **Byte[18]** |        **System.Byte[]** |    **584.39 ns** |  **11.778 ns** |  **25.852 ns** |    **581.40 ns** |
|   **Bois** | **Deserialize** |             **DateTime** |             **Byte[10]** |      **System.DateTime** |    **147.28 ns** |   **2.906 ns** |   **4.438 ns** |    **146.60 ns** |
|   **Json** | **Deserialize** |             **DateTime** |             **Byte[21]** |      **System.DateTime** |    **682.29 ns** |  **13.700 ns** |  **31.203 ns** |    **677.70 ns** |
|  **Naive** | **Deserialize** |             **DateTime** |              **Byte[9]** |      **System.DateTime** |    **108.65 ns** |   **2.258 ns** |   **2.936 ns** |    **108.30 ns** |
|   **Bois** | **Deserialize** | **Dicti(...)&gt;(10) [26]** |             **Byte[31]** | **Syste(...)ring] [67]** |  **2,401.61 ns** |  **45.505 ns** |  **42.565 ns** |  **2,390.30 ns** |
|   **Json** | **Deserialize** | **Dicti(...)&gt;(10) [26]** |             **Byte[81]** | **Syste(...)ring] [67]** |  **3,197.11 ns** |  **46.138 ns** |  **43.157 ns** |  **3,187.20 ns** |
|  **Naive** | **Deserialize** | **Dicti(...)&gt;(10) [26]** |             **Byte[85]** | **Syste(...)ring] [67]** |  **2,999.49 ns** |  **55.817 ns** |  **49.480 ns** |  **2,986.30 ns** |
|   **Json** | **Deserialize** |                 **Enum** |              **Byte[1]** |  **System.DateTimeKind** |    **935.34 ns** |  **18.501 ns** |  **31.416 ns** |    **945.60 ns** |
|   Bois | Deserialize |                 Enum |              Byte[1] |  System.DateTimeKind |    239.98 ns |   4.850 ns |   8.868 ns |    236.50 ns |
|  **Naive** | **Deserialize** |                 **Enum** |              **Byte[5]** |  **System.DateTimeKind** |    **202.64 ns** |   **3.500 ns** |   **3.274 ns** |    **202.60 ns** |
|   **Json** | **Deserialize** |             **Float(1)** |              **Byte[3]** |        **System.Single** |    **672.09 ns** |  **13.292 ns** |  **14.774 ns** |    **672.10 ns** |
|   Bois | Deserialize |             Float(1) |              Byte[3] |        System.Single |    141.34 ns |   2.639 ns |   2.469 ns |    142.00 ns |
|  **Naive** | **Deserialize** |             **Float(1)** |              **Byte[5]** |        **System.Single** |     **98.99 ns** |   **1.417 ns** |   **1.325 ns** |     **98.50 ns** |
|   **Json** | **Deserialize** |      **Float(MaxValue)** |             **Byte[13]** |        **System.Single** |    **785.58 ns** |  **15.258 ns** |  **18.738 ns** |    **784.40 ns** |
|  **Naive** | **Deserialize** |      **Float(MaxValue)** |              **Byte[5]** |        **System.Single** |    **101.08 ns** |   **2.058 ns** |   **2.817 ns** |    **101.05 ns** |
|   Bois | Deserialize |      Float(MaxValue) |              Byte[5] |        System.Single |    130.72 ns |   2.671 ns |   2.623 ns |    131.15 ns |
|  **Naive** | **Deserialize** |                 **Guid** |             **Byte[17]** |          **System.Guid** |    **120.13 ns** |   **2.456 ns** |   **3.194 ns** |    **119.95 ns** |
|   **Bois** | **Deserialize** |                 **Guid** |              **Byte[1]** |          **System.Guid** |    **111.85 ns** |   **2.068 ns** |   **1.935 ns** |    **112.30 ns** |
|   **Json** | **Deserialize** |                 **Guid** |             **Byte[38]** |          **System.Guid** |    **910.00 ns** |  **17.544 ns** |  **22.188 ns** |    **907.20 ns** |
|   **Json** | **Deserialize** |               **Int(1)** |              **Byte[1]** |         **System.Int32** |    **521.62 ns** |  **10.249 ns** |  **11.803 ns** |    **520.40 ns** |
|   Bois | Deserialize |               Int(1) |              Byte[1] |         System.Int32 |    106.29 ns |   2.204 ns |   2.450 ns |    106.70 ns |
|  **Naive** | **Deserialize** |               **Int(1)** |              **Byte[5]** |         **System.Int32** |    **101.41 ns** |   **2.092 ns** |   **3.552 ns** |    **101.30 ns** |
|   **Json** | **Deserialize** |        **Int(MaxValue)** |             **Byte[10]** |         **System.Int32** |    **544.92 ns** |  **10.902 ns** |  **13.388 ns** |    **542.55 ns** |
|  **Naive** | **Deserialize** |        **Int(MaxValue)** |              **Byte[5]** |         **System.Int32** |    **100.69 ns** |   **2.056 ns** |   **2.673 ns** |    **100.20 ns** |
|   Bois | Deserialize |        Int(MaxValue) |              Byte[5] |         System.Int32 |    123.80 ns |   2.563 ns |   4.420 ns |    123.20 ns |
|   **Bois** | **Deserialize** |     **List&lt;String&gt;(10)** |             **Byte[21]** | **Syste(...)ring] [48]** |  **2,131.50 ns** |  **17.671 ns** |  **15.665 ns** |  **2,133.35 ns** |
|  **Naive** | **Deserialize** |     **List&lt;String&gt;(10)** |             **Byte[35]** | **Syste(...)ring] [48]** |  **1,040.79 ns** |  **20.260 ns** |  **22.518 ns** |  **1,031.30 ns** |
|   **Json** | **Deserialize** |     **List&lt;String&gt;(10)** |             **Byte[41]** | **Syste(...)ring] [48]** |  **1,810.80 ns** |  **21.475 ns** |  **20.088 ns** |  **1,811.10 ns** |
|   **Bois** | **Deserialize** |                 **Null** |                    **?** |                    **?** |           **NA** |         **NA** |         **NA** |           **NA** |
|  **Naive** | **Deserialize** |                 **Null** |              **Byte[1]** |                    **?** |     **83.72 ns** |   **1.755 ns** |   **2.021 ns** |     **82.65 ns** |
|   **Json** | **Deserialize** |                 **Null** |              **Byte[4]** |                    **?** |    **403.14 ns** |   **8.125 ns** |   **9.672 ns** |    **401.60 ns** |
|  **Naive** | **Deserialize** |         **Object[](10)** |            **Byte[139]** |      **System.Object[]** |  **1,837.64 ns** |  **36.682 ns** |  **99.796 ns** |  **1,782.15 ns** |
|   **Bois** | **Deserialize** |         **Object[](10)** |              **Byte[1]** |      **System.Object[]** |  **1,643.42 ns** |  **24.159 ns** |  **21.416 ns** |  **1,638.65 ns** |
|   **Json** | **Deserialize** |         **Object[](10)** |            **Byte[211]** |      **System.Object[]** |  **8,146.86 ns** |  **97.861 ns** |  **91.539 ns** |  **8,136.40 ns** |
|   **Json** | **Deserialize** |     **PlainClass[](10)** |           **Byte[1250]** | **Bench(...)ass[] [22]** | **17,882.03 ns** | **163.703 ns** | **181.956 ns** | **17,853.50 ns** |
|   **Bois** | **Deserialize** |     **PlainClass[](10)** |            **Byte[401]** | **Bench(...)ass[] [22]** |  **6,671.06 ns** |  **60.744 ns** |  **56.820 ns** |  **6,658.20 ns** |
|  **Naive** | **Deserialize** |     **PlainClass[](10)** |            **Byte[855]** | **Bench(...)ass[] [22]** |  **7,799.05 ns** | **113.397 ns** | **218.478 ns** |  **7,718.95 ns** |
|   **Bois** | **Deserialize** |           **String(10)** |             **Byte[11]** |        **System.String** |    **162.52 ns** |   **2.597 ns** |   **2.429 ns** |    **162.60 ns** |
|  **Naive** | **Deserialize** |           **String(10)** |             **Byte[12]** |        **System.String** |    **170.42 ns** |   **3.445 ns** |   **3.686 ns** |    **170.05 ns** |
|   Json | Deserialize |           String(10) |             Byte[12] |        System.String |    523.00 ns |  10.432 ns |  28.026 ns |    520.55 ns |
|   **Bois** | **Deserialize** |         **String[](10)** |             **Byte[21]** |      **System.String[]** |    **944.23 ns** |  **13.066 ns** |  **12.222 ns** |    **938.20 ns** |
|  **Naive** | **Deserialize** |         **String[](10)** |             **Byte[35]** |      **System.String[]** |    **914.27 ns** |   **7.966 ns** |   **7.451 ns** |    **914.20 ns** |
|   **Json** | **Deserialize** |         **String[](10)** |             **Byte[41]** |      **System.String[]** |  **1,937.30 ns** |  **37.708 ns** |  **33.427 ns** |  **1,925.55 ns** |
|   **Json** | **Deserialize** |             **TimeSpan** |             **Byte[10]** |      **System.TimeSpan** |  **1,059.24 ns** |  **19.642 ns** |  **24.122 ns** |  **1,051.40 ns** |
|   **Bois** | **Deserialize** |             **TimeSpan** |              **Byte[6]** |      **System.TimeSpan** |    **131.86 ns** |   **2.113 ns** |   **2.170 ns** |    **131.60 ns** |
|  **Naive** | **Deserialize** |             **TimeSpan** |              **Byte[9]** |      **System.TimeSpan** |    **104.25 ns** |   **1.973 ns** |   **1.846 ns** |    **103.50 ns** |
|  **Naive** |   **Serialize** |                 **Bool** |                 **True** |                    **?** |    **124.02 ns** |   **2.537 ns** |   **3.874 ns** |    **123.50 ns** |
|   Json |   Serialize |                 Bool |                 True |                    ? |    397.12 ns |   5.217 ns |   4.357 ns |    395.70 ns |
|   Bois |   Serialize |                 Bool |                 True |                    ? |    125.75 ns |   2.476 ns |   3.041 ns |    125.35 ns |
|  **Naive** |   **Serialize** |          **Byte?[](10)** |   **Nullable&lt;Byte&gt;[10]** |                    **?** |    **650.53 ns** |  **12.983 ns** |  **22.396 ns** |    **639.05 ns** |
|   Json |   Serialize |          Byte?[](10) |   Nullable&lt;Byte&gt;[10] |                    ? |  1,970.02 ns |  38.198 ns |  39.227 ns |  1,954.70 ns |
|   Bois |   Serialize |          Byte?[](10) |   Nullable&lt;Byte&gt;[10] |                    ? |  1,116.47 ns |  13.583 ns |  12.705 ns |  1,112.50 ns |
|  **Naive** |   **Serialize** |           **Byte[](10)** |             **Byte[10]** |                    **?** |    **147.16 ns** |   **0.672 ns** |   **0.525 ns** |    **146.95 ns** |
|   Json |   Serialize |           Byte[](10) |             Byte[10] |                    ? |    533.09 ns |   9.672 ns |   9.047 ns |    529.20 ns |
|   Bois |   Serialize |           Byte[](10) |             Byte[10] |                    ? |    153.26 ns |   3.068 ns |   4.954 ns |    151.50 ns |
|  **Naive** |   **Serialize** |             **DateTime** |  **01/01/1000 00:00:00** |                    **?** |    **131.28 ns** |   **2.296 ns** |   **2.148 ns** |    **131.70 ns** |
|   Json |   Serialize |             DateTime |  01/01/1000 00:00:00 |                    ? |    481.74 ns |   5.930 ns |   5.257 ns |    482.15 ns |
|   Bois |   Serialize |             DateTime |  01/01/1000 00:00:00 |                    ? |    191.71 ns |   3.786 ns |   4.923 ns |    192.75 ns |
|  **Naive** |   **Serialize** | **Dicti(...)&gt;(10) [26]** | **Syste(...)ring] [67]** |                    **?** |    **967.79 ns** |  **10.210 ns** |   **9.550 ns** |    **963.10 ns** |
|   Json |   Serialize | Dicti(...)&gt;(10) [26] | Syste(...)ring] [67] |                    ? |  2,685.93 ns |  21.717 ns |  20.314 ns |  2,688.30 ns |
|   Bois |   Serialize | Dicti(...)&gt;(10) [26] | Syste(...)ring] [67] |                    ? |  2,104.38 ns |  15.716 ns |  13.124 ns |  2,105.70 ns |
|  **Naive** |   **Serialize** |                 **Enum** |                  **Utc** |                    **?** |    **136.21 ns** |   **2.788 ns** |   **3.816 ns** |    **135.45 ns** |
|   Json |   Serialize |                 Enum |                  Utc |                    ? |    410.53 ns |   5.165 ns |   4.831 ns |    408.60 ns |
|   Bois |   Serialize |                 Enum |                  Utc |                    ? |    171.66 ns |   1.442 ns |   1.603 ns |    172.40 ns |
|  **Naive** |   **Serialize** |             **Float(1)** |                    **1** |                    **?** |    **128.81 ns** |   **2.351 ns** |   **2.199 ns** |    **129.40 ns** |
|   Json |   Serialize |             Float(1) |                    1 |                    ? |    614.06 ns |  12.131 ns |  12.458 ns |    613.30 ns |
|   Bois |   Serialize |             Float(1) |                    1 |                    ? |    150.55 ns |   1.596 ns |   1.415 ns |    151.05 ns |
|  **Naive** |   **Serialize** |      **Float(MaxValue)** |        **3.4028235E+38** |                    **?** |    **126.71 ns** |   **2.514 ns** |   **2.992 ns** |    **127.70 ns** |
|   Json |   Serialize |      Float(MaxValue) |        3.4028235E+38 |                    ? |    675.89 ns |  12.011 ns |  10.647 ns |    676.00 ns |
|   Bois |   Serialize |      Float(MaxValue) |        3.4028235E+38 |                    ? |    155.88 ns |   3.068 ns |   3.534 ns |    155.40 ns |
|  **Naive** |   **Serialize** |                 **Guid** | **00000(...)00000 [36]** |                    **?** |    **138.15 ns** |   **2.834 ns** |   **3.374 ns** |    **137.90 ns** |
|   Json |   Serialize |                 Guid | 00000(...)00000 [36] |                    ? |    478.82 ns |   3.808 ns |   3.180 ns |    478.30 ns |
|   Bois |   Serialize |                 Guid | 00000(...)00000 [36] |                    ? |    137.85 ns |   0.659 ns |   0.550 ns |    137.70 ns |
|  **Naive** |   **Serialize** |               **Int(1)** |                    **1** |                    **?** |    **125.74 ns** |   **2.277 ns** |   **2.130 ns** |    **125.10 ns** |
|   Json |   Serialize |               Int(1) |                    1 |                    ? |    384.47 ns |   6.036 ns |   5.350 ns |    382.10 ns |
|   Bois |   Serialize |               Int(1) |                    1 |                    ? |    125.88 ns |   2.252 ns |   1.881 ns |    126.20 ns |
|  **Naive** |   **Serialize** |        **Int(MaxValue)** |           **2147483647** |                    **?** |    **129.64 ns** |   **2.652 ns** |   **5.236 ns** |    **127.80 ns** |
|   Json |   Serialize |        Int(MaxValue) |           2147483647 |                    ? |    430.53 ns |   7.406 ns |   6.184 ns |    429.10 ns |
|   Bois |   Serialize |        Int(MaxValue) |           2147483647 |                    ? |    151.81 ns |   3.056 ns |   3.519 ns |    150.50 ns |
|  **Naive** |   **Serialize** |     **List&lt;String&gt;(10)** | **Syste(...)ring] [48]** |                    **?** |    **622.21 ns** |   **5.753 ns** |   **5.100 ns** |    **621.80 ns** |
|   Json |   Serialize |     List&lt;String&gt;(10) | Syste(...)ring] [48] |                    ? |  1,834.89 ns |  21.586 ns |  20.192 ns |  1,835.50 ns |
|   Bois |   Serialize |     List&lt;String&gt;(10) | Syste(...)ring] [48] |                    ? |  2,013.17 ns |  29.579 ns |  27.668 ns |  2,021.90 ns |
|  **Naive** |   **Serialize** |                 **Null** |                    **?** |                    **?** |     **87.04 ns** |   **1.683 ns** |   **1.492 ns** |     **86.60 ns** |
|   Json |   Serialize |                 Null |                    ? |                    ? |    288.76 ns |   1.794 ns |   1.498 ns |    288.10 ns |
|   Bois |   Serialize |                 Null |                    ? |                    ? |           NA |         NA |         NA |           NA |
|  **Naive** |   **Serialize** |         **Object[](10)** |           **Object[10]** |                    **?** |  **1,235.86 ns** |  **28.118 ns** |  **82.907 ns** |  **1,191.75 ns** |
|   Json |   Serialize |         Object[](10) |           Object[10] |                    ? |  4,907.93 ns |  25.314 ns |  21.138 ns |  4,911.10 ns |
|   Bois |   Serialize |         Object[](10) |           Object[10] |                    ? |  1,271.79 ns |  15.364 ns |  13.620 ns |  1,268.00 ns |
|  **Naive** |   **Serialize** |     **PlainClass[](10)** |       **PlainClass[10]** |                    **?** |  **4,241.46 ns** |  **22.126 ns** |  **18.477 ns** |  **4,246.90 ns** |
|   Json |   Serialize |     PlainClass[](10) |       PlainClass[10] |                    ? |  9,917.97 ns | 168.283 ns | 347.534 ns |  9,880.30 ns |
|   Bois |   Serialize |     PlainClass[](10) |       PlainClass[10] |                    ? |  3,837.17 ns |  22.557 ns |  21.100 ns |  3,844.70 ns |
|  **Naive** |   **Serialize** |           **String(10)** |           ************** |                    **?** |    **153.06 ns** |   **2.294 ns** |   **2.146 ns** |    **153.00 ns** |
|   Json |   Serialize |           String(10) |           ********** |                    ? |    431.79 ns |   6.470 ns |   5.403 ns |    432.40 ns |
|   Bois |   Serialize |           String(10) |           ********** |                    ? |    192.08 ns |   3.640 ns |   3.405 ns |    191.20 ns |
|  **Naive** |   **Serialize** |         **String[](10)** |           **String[10]** |                    **?** |    **577.68 ns** |   **1.474 ns** |   **1.307 ns** |    **577.80 ns** |
|   Json |   Serialize |         String[](10) |           String[10] |                    ? |  1,765.95 ns |  11.287 ns |  10.558 ns |  1,762.10 ns |
|   Bois |   Serialize |         String[](10) |           String[10] |                    ? |  1,000.99 ns |  18.868 ns |  18.531 ns |    991.25 ns |
|  **Naive** |   **Serialize** |             **TimeSpan** |             **10:10:10** |                    **?** |    **133.72 ns** |   **2.747 ns** |   **4.027 ns** |    **132.50 ns** |
|   Json |   Serialize |             TimeSpan |             10:10:10 |                    ? |    440.12 ns |   2.505 ns |   1.956 ns |    440.70 ns |
|   Bois |   Serialize |             TimeSpan |             10:10:10 |                    ? |    162.53 ns |   3.327 ns |   3.267 ns |    161.90 ns |

Benchmarks with issues:
  Benchmark.Bois: Job-NMULZF(InvocationCount=1000, LaunchCount=1, RunStrategy=Throughput, UnrollFactor=1, WarmupCount=1) [method=Deserialize, name=Null, value=?, type=?]
  Benchmark.Bois: Job-NMULZF(InvocationCount=1000, LaunchCount=1, RunStrategy=Throughput, UnrollFactor=1, WarmupCount=1) [method=Serialize, name=Null, value=?]
