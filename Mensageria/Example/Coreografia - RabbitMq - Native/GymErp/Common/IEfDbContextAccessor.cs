using Microsoft.EntityFrameworkCore;

namespace GymErp.Common;

public interface IEfDbContextAccessor<T> : IDisposable where T : DbContext // Define um contrato para acessar um DbContext
{
    void Register(T context); // Registra uma instância de DbContext.
    T Get();  // Retorna o DbContext registrado.
    void Clear(); // Limpa o DbContext registrado.
}
