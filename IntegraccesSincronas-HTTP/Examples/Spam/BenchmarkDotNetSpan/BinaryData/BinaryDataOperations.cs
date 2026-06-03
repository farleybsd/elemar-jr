using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BenchmarkDotNetSpan.BinaryData;

[MemoryDiagnoser]
public class BinaryDataOperations
{
    byte[] data = new byte[1000000];
    byte[] pattern = { 0x12, 0x34, 0x56 };
    int index = 0;

    [Benchmark]
    public void SearchPattern()
    {
        for (var i = 0; i < data.Length - pattern.Length; i++)
        {
            if (data[i] == pattern[0] && data[i + 1] == pattern[1] && data[i + 2] == pattern[2])
            {
                index = i;
                break;
            }
        }
    }

    [Benchmark]
    public void SearchPatternWithSpan()
    {
        var span = new Span<byte>(data);
        index = span.IndexOf(pattern);
    }

}