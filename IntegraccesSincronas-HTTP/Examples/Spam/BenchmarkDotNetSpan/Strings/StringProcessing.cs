using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BenchmarkDotNetSpan.Strings;

[MemoryDiagnoser]
public class StringProcessing
{
    private const string OriginalString = "Bem vindos ao meu canal do youtube!";

    [Benchmark]
    public void ExtractNumbers()
    {
        var substring = OriginalString.Substring(7, 5);
    }

    [Benchmark]
    public void ExtractNumbersWithSpan()
    {
        var span = OriginalString.AsSpan().Slice(7, 5);
    }
}
