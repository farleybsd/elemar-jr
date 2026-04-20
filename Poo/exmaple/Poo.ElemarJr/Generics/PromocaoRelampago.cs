namespace Generics;

public class PromocaoRelampago<T> where T : IItemVendavel //Aplica as restrições ao parâmetro
{
    public void AplicarDesconto(T item)
    {
        Console.WriteLine($"Novo preço do {item.Nome}: R$ {item.Preco}");
    }
}
