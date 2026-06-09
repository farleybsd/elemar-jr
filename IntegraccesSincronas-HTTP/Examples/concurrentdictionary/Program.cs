

using System.Collections.Concurrent;
using System.Numerics;


///Em C#, um ` ConcurrentDictionaryA` é uma coleção thread-safe que armazena
///pares de chave-valor, projetada para cenários onde múltiplas threads precisam
///adicionar, remover ou modificar itens simultaneamente. Diferentemente de um
///`Dictionary` comum, ele oferece operações atômicas que são seguras para acesso
///concorrente sem bloqueio explícito, tornando-o ideal para padrões produtor-consumidor
///ou cenários de cache onde é necessário garantir a consistência dos dados entre threads.


AdicionarELerItensEmParalelo();
DemonstrarOperacoesComConcurrentDictionary();

void AdicionarELerItensEmParalelo()
{
    // Criar um Dicionario Concorrente
    ConcurrentDictionary<int,string> dict = new ConcurrentDictionary<int,string>();

    //Adicionar ou atualizar itens no ConcurrentDictionary em paralelo

    Parallel.For(0, 10, i =>
    {
        dict.AddOrUpdate(i, $"Value {i}", (key, oldValue) => $"Value {i}");
        Console.WriteLine($"Added or updated key {i}");
    });

    //Leia os itens do ConcurrentDictionary

    Parallel.For(0, 10, i =>
    {
        string result;

        if( dict.TryGetValue(i, out result))
        {
            Console.WriteLine($"Key {i} has value {result}");
        }
    });

    //Exibir o conteúdo do ConcurrentDictionary
    foreach (var kvp in dict)
    {
        Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
    }
}

/// Em resumo, o código demonstra como armazenar, atualizar, contar e remover dados
/// compartilhados entre threads sem precisar controlar lock manualmente.
void DemonstrarOperacoesComConcurrentDictionary()
{
    var cache = new ConcurrentDictionary<string, ExpensiveResult>();

        // GetOrAdd - só calculará o valor se a chave não existir.
    var result = cache.GetOrAdd("key1", key => {
        // Esta operação dispendiosa só é executada se a chave1 não existir.
        return PerformExpensiveOperation(key);
    });

    // Atualizações seguras para threads usando AddOrUpdate
    var userVisits = new ConcurrentDictionary<string, int>();
    userVisits.AddOrUpdate(
        "user123",
        // Adiciona um valor se a chave não existir
        key => 1,
        // Atualizar valor se a chave existir
        (key, oldValue) => oldValue + 1
    );


    // Remoção segura com TryRemove

    if (cache.TryRemove("key1", out var removedValue))
    {
        Console.WriteLine($"Successfully removed: {removedValue}");
    }

    // Atualizações atômicas usando GetOrAdd com uma fábrica
    var counter = new ConcurrentDictionary<string, int>();
    var items = new List<Item>
                {
                    new Item { Name = "Notebook", Category = "Eletrônicos" },
                    new Item { Name = "Celular", Category = "Eletrônicos" },
                    new Item { Name = "Camiseta", Category = "Roupas" },
                    new Item { Name = "Calça", Category = "Roupas" },
                    new Item { Name = "Livro C#", Category = "Livros" }
                };

    Parallel.ForEach(items, item => {
        counter.AddOrUpdate(
            item.Category,
            // Initial value if key doesn't exist
            category => 1,
            // Update function if key exists
            (category, existingCount) => existingCount + 1
        );
    });
    // Inicialização thread - safe de objetos complexos
    var connections = new ConcurrentDictionary<string, DatabaseConnection>();
    var connection = connections.GetOrAdd("db1", serverName => {
        return new DatabaseConnection(serverName, timeout: 30);
    });
}

static ExpensiveResult PerformExpensiveOperation(string key)
{
    Thread.Sleep(2000); // Simula uma demora de 2 segundos

    return new ExpensiveResult
    {
        Data = $"Resultado processado para {key}",
        CalculatedAt = DateTime.Now
    };
}
public class ExpensiveResult
{
    public string Data { get; set; }
    public DateTime CalculatedAt { get; set; }
}

public class Item
{
    public string Name { get; set; }
    public string Category { get; set; }
}

public class DatabaseConnection
{
    public string ServerName { get; }
    public int Timeout { get; }

    public DatabaseConnection(string serverName, int timeout)
    {
        ServerName = serverName;
        Timeout = timeout;
    }

    public void Connect()
    {
        Console.WriteLine(
            $"Conectando ao servidor {ServerName}..."
        );
    }
}