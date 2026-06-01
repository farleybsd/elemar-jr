/*
 * Este documento demonstra um pipeline de fluxo de dados que baixa o livro A Ilíada de Homero de um site e pesquisa o texto para corresponder palavras
 * individuais com palavras que invertem a ordem dos caracteres da primeira palavra. A formação do pipeline de fluxo de dados neste documento consiste nas seguintes etapas:
 * Crie os blocos de fluxo de dados que participam do pipeline.
 * 1.Crie os blocos de fluxo de dados que participam do pipeline.
 * 2. Conecte cada bloco de fluxo de dados ao próximo bloco na cadeia. Cada bloco recebe como entrada a saída do bloco anterior no pipeline.
 * 3. Para cada bloco de fluxo de dados, crie uma tarefa de continuação que define o próximo bloco para o estado concluído após a conclusão do bloco anterior.
 * 4. Poste dados no cabeçalho do pipeline.
 * 5. Defina a cabeça do pipeline como concluída.
 * 6. Aguarde até que o pipeline conclua todo o trabalho.

 */
//Conecte os blocos de fluxo de dados para formar um pipeline.

using Dataflow.Block.Livro;

using var downloader = new HttpTextDownloader();

var outputWriter = new ConsoleOutputWriter();

var pipeline = new Pipelines(
    new DownloadStringPipe(downloader, outputWriter),
    new CreateWordListPipe(outputWriter),
    new FilterWordListPipe(outputWriter),
    new FindReversedWordsPipe(outputWriter),
    new PrintReversedWordsPipe(outputWriter));

await pipeline.ExecuteAsync("https://www.gutenberg.org/files/6130/6130-0.txt");
