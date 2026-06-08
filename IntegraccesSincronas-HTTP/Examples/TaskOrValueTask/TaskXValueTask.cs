Resumo:

Task<T> é o padrão para métodos assíncronos.Use quando a operação normalmente precisa esperar algo:
banco -> HTTP ->arquivo ->fila -> serviço externo

ValueTask<T> é uma otimização.

Use quando o método precisa ter assinatura async, mas muitas vezes o resultado já está pronto em memória, como cache.Se é só cálculo local ou regra em memória, não use nenhum dos dois. Retorne o valor direto.
bool -> int -> decimal -> Usuario


public sealed class PermissionService
{
    private readonly Dictionary<int, bool> _cache = new();

    //Se é só cálculo local ou regra em memória, não use nenhum dos dois. Retorne o valor direto. bool -> int -> decimal -> Usuario
    public bool TemPermissaoLocal(Usuario usuario)
    {
        return usuario.Perfil == "Admin";
    }

    //Task<T> é o padrão para métodos assíncronos.Use quando a operação normalmente precisa esperar algo
    public async Task<bool> TemPermissaoNoBancoAsync(int usuarioId)
    {
        await Task.Delay(500); // simula banco/rede

        return usuarioId == 1;
    }

  // se quando o método precisa ter assinatura async, mas muitas vezes o resultado já está pronto em memória, como cache
    public ValueTask<bool> TemPermissaoAsync(int usuarioId)
    {
        if (_cache.TryGetValue(usuarioId, out bool permitido))
        {
            // Resultado ja esta pronto em memoria.
            // Aqui ValueTask evita criar uma Task<bool>.
            return ValueTask.FromResult(permitido);
        }

        // Nao estava em memoria.
        // Agora cai no fluxo async normal.
        return new ValueTask<bool>(BuscarSalvarNoCacheAsync(usuarioId));
    }

    private async Task<bool> BuscarSalvarNoCacheAsync(int usuarioId)
    {
        bool permitido = await TemPermissaoNoBancoAsync(usuarioId);

        _cache[usuarioId] = permitido;

        return permitido;
    }
}

public sealed class Usuario
{
    public int Id { get; set; }
    public string Perfil { get; set; } = "";
}