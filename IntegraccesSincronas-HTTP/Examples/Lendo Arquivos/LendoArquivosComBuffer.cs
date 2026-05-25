using System;
using System.Collections.Generic;
using System.Text;

namespace Lendo_Arquivos;

internal class LendoArquivosComBuffer
{
    public async Task LerArquivo(string path)
    {
        byte[] buffer = new byte[1024]; // Buffer de 1 KB

        using ( var fs = new FileStream(path,FileMode.Open,FileAccess.Read))
        {
            int bytesRead;

            while ((bytesRead = await fs.ReadAsync(buffer,0,buffer.Length)) > 0)
            {
                Console.Write(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
        }
    }
}
