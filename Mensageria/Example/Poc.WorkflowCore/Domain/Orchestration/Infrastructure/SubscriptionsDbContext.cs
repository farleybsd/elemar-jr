using Microsoft.EntityFrameworkCore;

namespace Poc.WorkflowCore.Domain.Orchestration.Infrastructure;

public class SubscriptionsDbContext : DbContext
{
    public SubscriptionsDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Enrollment> Enrollments { get; set; }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Aqui você pode adicionar lógica extra (Domain Events, auditoria, etc)
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result > 0;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Enrollment>(builder =>
        {
            builder.ToTable("Enrollments");
            builder.HasKey(e => e.Id);
            builder.OwnsOne(e => e.Client, client =>
            {
                client.Property(c => c.Cpf).HasColumnName("ClientCpf");
                client.Property(c => c.Name).HasColumnName("ClientName");
                client.Property(c => c.Email).HasColumnName("ClientEmail");
                client.Property(c => c.Phone).HasColumnName("ClientPhone");
                client.Property(c => c.Address).HasColumnName("ClientAddress");
            });
            builder.Property(e => e.RequestDate).HasColumnName("RequestDate");
            builder.Property(e => e.State).HasColumnName("State");
            builder.Property(e => e.SuspensionStartDate).HasColumnName("SuspensionStartDate");
            builder.Property(e => e.SuspensionEndDate).HasColumnName("SuspensionEndDate");
        });
    }

}
