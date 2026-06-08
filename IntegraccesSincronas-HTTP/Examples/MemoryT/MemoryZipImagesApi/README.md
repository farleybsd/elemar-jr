# MemoryZipImagesApi

Exemplo de Minimal API usando `ReadOnlyMemory<byte>` em um fluxo de I/O:

1. O endpoint baixa imagens aleatorias da internet.
2. Cada imagem e mantida como `ReadOnlyMemory<byte>`.
3. O conteudo e escrito em um `ZipArchive` com `WriteAsync(ReadOnlyMemory<byte>)`.
4. A API devolve o arquivo `.zip` para download.

## Rodar

```powershell
dotnet run --project . --urls http://127.0.0.1:5088
```

## Testar

Abra no navegador:

```text
http://127.0.0.1:5088/images.zip?count=5
```

Ou via PowerShell:

```powershell
Invoke-WebRequest -Uri http://127.0.0.1:5088/images.zip?count=5 -OutFile images.zip
```
