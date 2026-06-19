using Microsoft.EntityFrameworkCore;
using OutBox.Domain.Entities;
using System.Text.Json;

namespace OutBox.Data;

public class OutboxDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OutboxRecord> Outbox { get; set; }
    
    public OutboxDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder // Acessa o objeto usado para configurar as entidades no Entity Framework Core.
                .Entity<Order>() // Seleciona a entidade Order para realizar a configuração.
                .HasMany(e => e.Items) // Define que um Order possui vários itens na propriedade Items.
                .WithOne() //Define que cada OrderItem pertence a um único Order,mas OrderItem não possui uma propriedade de navegação para Order.
                .IsRequired();  // Torna o relacionamento obrigatório: todo OrderItem deve pertencer a um Order.

        modelBuilder // Acessa novamente o configurador de entidades do Entity Framework Core.
                .Entity<Order>()  // Seleciona a entidade Order.
                .Navigation(e => e.Items) // Seleciona a propriedade de navegação Items.
                .AutoInclude(); // Carrega Items automaticamente sempre que um Order for consultado.

        modelBuilder.Entity<OutboxRecord>() // Seleciona a entidade OutboxRecord para realizar a configuração.
                .Property(e => e.Event) // Seleciona a propriedade Event da entidade OutboxRecord.
                .HasConversion(
                    @event => JsonSerializer.Serialize(@event, new JsonSerializerOptions()), // Ao salvar, transforma o objeto Event em uma string no formato JSON.
                    serialized => JsonSerializer.Deserialize<object>(serialized, new JsonSerializerOptions())!   // Ao buscar, transforma a string JSON armazenada no banco novamente em um objeto.
                );
    }
}
