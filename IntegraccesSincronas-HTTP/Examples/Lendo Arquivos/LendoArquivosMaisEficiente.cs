using System;
using System.IO;
using System.Threading.Tasks;

namespace Lendo_Arquivos;

internal class LendoArquivosMaisEficiente
{
    public async Task LerArquivo(string path)
    {
        try
        {
            using var reader = new StreamReader(path);

            string? line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                Console.WriteLine(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}