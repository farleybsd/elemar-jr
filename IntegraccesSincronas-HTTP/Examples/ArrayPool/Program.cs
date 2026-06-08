
using ArrayPool;

/*
 * reutilizar arrays já existentes/alocados.
 *  Evitar Pressao No GC.
 */
var origemPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "Downloads",
    "WhatsApp Image 2026-05-27 at 19.20.09.jpeg");

var destinoPath = @"C:\Git\elemar-jr\IntegraccesSincronas-HTTP\Examples\ArrayPool\copia.jpeg";

await using var origem = File.OpenRead(origemPath);
await using var destino = File.Create(destinoPath);

await StreamCopy.CopyAsync(origem, destino);