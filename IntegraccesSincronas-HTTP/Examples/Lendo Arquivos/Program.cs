using Lendo_Arquivos;

string path = @"C:\Git\elemar-jr\IntegraccesSincronas-HTTP\Examples\Lendo Arquivos\example.txt";
string pathwrite = @"C:\Git\elemar-jr\IntegraccesSincronas-HTTP\Examples\Lendo Arquivos\examplewrite.txt";

//LendoArquivosMaisEficiente lendoArquivosMaisEficiente = new();

//await lendoArquivosMaisEficiente.LerArquivo(path);

//LendoArquivosComBuffer lendoArquivosComBuffer = new();
//await lendoArquivosComBuffer.LerArquivo(path);

//EscrevendoEmArquivosMaisEficientes escrevendoEmArquivosMaisEficientes = new();
//await escrevendoEmArquivosMaisEficientes.EscreverArquivo(pathwrite);

EscrevendoEmArquivoConBuffer escrevendoEmArquivoConBuffer = new();
await escrevendoEmArquivoConBuffer.EscrevendoEmArquivo(pathwrite);