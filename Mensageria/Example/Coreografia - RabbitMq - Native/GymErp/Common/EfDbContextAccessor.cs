namespace GymErp.Common;

using Microsoft.EntityFrameworkCore;

public sealed class EfDbContextAccessor<T> : IEfDbContextAccessor<T> where T : DbContext
{
    private T? _context;

    public void Register(T context)
    {
        _context = context;
    }

    public T Get()
    {
        return _context ?? throw new InvalidOperationException($"DbContext {typeof(T).Name} não registrado.");
    }

    public void Clear()
    {
        _context = null;
    }

    public void Dispose()
    {
        _context?.Dispose();
        _context = null;
    }
}
