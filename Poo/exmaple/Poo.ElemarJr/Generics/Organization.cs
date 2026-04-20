namespace Generics;

internal class Organization<TEmployee,TWorker>
     where TEmployee : Employee , new() //Obriga que a classe tenha um construtor vazio
     where TWorker : IWork
{
    // Codigo da Classe
}

internal class Organization2<TEmployee, TWorker>
     where TEmployee : Employee, notnull //Obriga que o tipo informado não seja nullable
     where TWorker : IWork
{
    // Codigo da Classe
}


public class UltilizarEstoque
{
    // TOrigem: O tipo específico (Ex: Notebook)
    // TDestino: O tipo genérico da lista principal (Ex: Produto)

    // A Constraint "where TOrigem : TDestino" garante o polimorfismo
    // Ela diz: "Só aceito copiar se TOrigem for filho de TDestino"

    public void MoverItens<TOrigem, TDestino>(
        List<TOrigem> itensNovos,
        List<TDestino> estoqueGeral)
        where TOrigem : TDestino
    {
        foreach (var item in itensNovos)
        {
            // O compilador só permite essa linha
            // porque garantiu que TOrigem HERDA de TDestino.
            estoqueGeral.Add(item);
        }
        Console.WriteLine($"{itensNovos.Count} itens movidos com sucesso.");
    }
        
}