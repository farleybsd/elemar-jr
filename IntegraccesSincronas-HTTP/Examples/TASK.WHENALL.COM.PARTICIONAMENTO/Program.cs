using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// PROBLEMA QUE ESSE CÓDIGO RESOLVE:
//
// Este código resolve o problema de processar muitos arquivos de forma paralela,
// mas com controle de quantidade de tarefas ao mesmo tempo.
//
// Sem esse controle, a aplicação poderia tentar processar milhares de arquivos
// simultaneamente, consumindo muita memória, disco, CPU e ThreadPool.
//
// COMO ELE RESOLVE:
//
// Ele divide a lista de arquivos em partições usando Partitioner.
// Cada partição funciona como um "worker", processando uma parte dos arquivos.
// No final, Task.WhenAll espera todos os workers terminarem.

app.MapPost("/process-files", async() =>
{
    // Cria partições da lista de arquivos.
    // files: lista/caminhos dos arquivos que serão processados.
    // loadBalance: true permite balanceamento dinâmico.
    // Se uma partição terminar antes, ela pode pegar mais trabalho.
    // batchSize: quantidade de partições/workers paralelos.

    string folderPath = @"C:\Temp\Files";

    if (!Directory.Exists(folderPath))
        return Results.BadRequest("Pasta não encontrada");

    var files = Directory.GetFiles(folderPath);
    int batchSize = Environment.ProcessorCount;
    var results = new ConcurrentBag<string>();

    /*
     * A classe Partitioner serve para dividir uma coleção em partes menores para processamento paralelo,
     * normalmente usada junto com Parallel.ForEach, Task, PLINQ ou cenários de alta concorrência.
     */

    var batches = Partitioner
       .Create(files, loadBalance: true)
       .GetPartitions(batchSize);

    // Cria uma coleção de Tasks assíncronas.
    // Cada item de "batches" representa uma partição de arquivos.
    // O Select percorre cada partição e cria uma Task para ela.
    //
    // Ou seja:
    //
    // Se batchSize = 4:
    //
    // Será criado algo parecido com:
    //
    // Worker 1 -> processa parte dos arquivos
    // Worker 2 -> processa outra parte
    // Worker 3 -> processa outra parte
    // Worker 4 -> processa outra parte
    //
    // Tudo isso executando em paralelo.


    var partitionTasks = batches.Select(async partition =>
    {
        // using garante que a partição será liberada da memória
        // corretamente ao final do processamento.
        // Isso é importante porque GetPartitions()
        // retorna objetos que implementam IDisposable.
        using (partition)
        {
            // Enquanto existir arquivo dentro dessa partição
            // o loop continua executando.
            while (partition.MoveNext())
            {
                // Pega o arquivo atual da partição.
                var file = partition.Current;

                try
                {
                    // Lê o conteúdo do arquivo de forma assíncrona.
                    //
                    // ReadAllTextAsync evita bloquear a thread
                    // enquanto o disco realiza a leitura.
                    string content = await File.ReadAllTextAsync(file);

                    // Processa o conteúdo do arquivo.
                    await ProcessFileContentAsync(content);

                    results.Add($"Arquivo processado: {Path.GetFileName(file)}");
                }
                catch(Exception ex)
                {
                    results.Add($"Erro em {file}: {ex.Message}");
                }
            }
        }
    });

    await Task.WhenAll(partitionTasks);
    return Results.Ok("Arquivos processados com sucesso");
});

static async Task ProcessFileContentAsync(string content)
{
    // Processa o conteúdo do arquivo.
    //
    // Aqui poderia existir:
    //
    // - Validação
    // - Importação CSV
    // - Salvar no banco
    // - Enviar para API
    // - Processamento de imagens
    // - ETL
    // - Machine Learning
    // etc.

    // Simula processamento pesado
    await Task.Delay(500);

    Console.WriteLine($"Conteúdo processado: {content.Length} caracteres");
}
app.Run();


