using Microsoft.EntityFrameworkCore;
using PicPayManager.Transference;
using PicPayManager.UserAccounts;

namespace PicPayManager.Crosscutting.Database;

public class PicPaySimplificadoContext : DbContext, IUnitOfWork
{
    public PicPaySimplificadoContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Transactions> Transactions { get; set; }
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Aqui você pode adicionar lógica extra (Domain Events, auditoria, etc)
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result > 0;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PicPaySimplificadoContext).Assembly);
    }
}