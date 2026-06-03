using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BenchmarkDotNetSpan.Array;

[MemoryDiagnoser]
public class ArrayOperations
{
    private static readonly int[] Array = new int[1000000];

    
    public void FillArray()
    {
        for (var i = 0; i < Array.Length; i++)
        {
            Array[i] += 10;
        }
    }

    [Benchmark]
    public void FillArrayWithSpan()
    {
        var span = new Span<int>(Array);
        span.Fill(10);
    }
}
