using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("images", client =>
{
    client.Timeout = TimeSpan.FromSeconds(20);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MemoryZipImagesApi/1.0");
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    message = "Use GET /images.zip?count=5 para baixar um ZIP com imagens aleatorias.",
    example = "/images.zip?count=5"
}));

app.MapGet("/images.zip", async (
    int? count,
    IHttpClientFactory httpClientFactory,
    CancellationToken cancellationToken) =>
{
    var imageCount = Math.Clamp(count ?? 5, 1, 10);
    var httpClient = httpClientFactory.CreateClient("images");

    var downloads = Enumerable
        .Range(1, imageCount)
        .Select(index => DownloadRandomImageAsync(httpClient, index, cancellationToken));

    var images = await Task.WhenAll(downloads);

    await using var zipBuffer = new MemoryStream();

    using (var archive = new ZipArchive(zipBuffer, ZipArchiveMode.Create, leaveOpen: true))
    {
        foreach (var image in images)
        {
            var entry = archive.CreateEntry(image.FileName, CompressionLevel.Fastest);

            await using var entryStream = entry.Open();

            // Aqui esta o ponto principal: escrevemos a memoria diretamente,
            // sem precisar transformar a fatia em outro byte[].
            await entryStream.WriteAsync(image.Content, cancellationToken);
        }
    }

    var zipBytes = zipBuffer.ToArray();
    var fileName = $"random-images-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.zip";

    return Results.File(
        zipBytes,
        contentType: "application/zip",
        fileDownloadName: fileName);
});

app.Run();

static async Task<DownloadedImage> DownloadRandomImageAsync(
    HttpClient httpClient,
    int index,
    CancellationToken cancellationToken)
{
    var seed = $"{Guid.NewGuid():N}-{index}";
    var url = $"https://picsum.photos/seed/{seed}/800/600";

    await using var remoteStream = await httpClient.GetStreamAsync(url, cancellationToken);
    await using var imageBuffer = new MemoryStream();

    await remoteStream.CopyToAsync(imageBuffer, cancellationToken);

    // ToArray cria o array final com o tamanho exato dos bytes baixados.
    // Depois disso, ReadOnlyMemory<byte> representa esse conteudo para o restante do fluxo.
    ReadOnlyMemory<byte> content = imageBuffer.ToArray();

    return new DownloadedImage($"image-{index:D2}.jpg", content);
}

file sealed record DownloadedImage(string FileName, ReadOnlyMemory<byte> Content);
