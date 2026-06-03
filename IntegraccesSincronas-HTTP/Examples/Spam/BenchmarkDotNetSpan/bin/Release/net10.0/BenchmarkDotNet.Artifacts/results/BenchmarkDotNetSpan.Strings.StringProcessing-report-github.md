```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8457/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i7-12700K 3.60GHz, 1 CPU, 20 logical and 12 physical cores
.NET SDK 10.0.300
  [Host]     : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3


```
| Method                 | Mean      | Error     | StdDev    | Gen0   | Allocated |
|----------------------- |----------:|----------:|----------:|-------:|----------:|
| ExtractNumbers         | 1.9022 ns | 0.0163 ns | 0.0136 ns | 0.0024 |      32 B |
| ExtractNumbersWithSpan | 0.0014 ns | 0.0011 ns | 0.0010 ns |      - |         - |
