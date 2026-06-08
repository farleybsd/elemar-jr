using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace ArrayPool;

public static class StreamCopy
{
    private const int DefaultBufferSize = 64 * 1024; // 64KB

    public static async Task CopyAsync(Stream source, Stream destination,
                                       CancellationToken ct = default)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);

        try
        {
            int read;
            while ((read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, read), ct);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer, clearArray: false);
        }
    }
    public static async Task CopyAsyncSemArrayPool(
                                                    Stream source,
                                                    Stream destination,
                                                    CancellationToken ct = default)
    {
        byte[] buffer = new byte[DefaultBufferSize];

        int read;

        while ((read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, read), ct);
        }
    }
}
