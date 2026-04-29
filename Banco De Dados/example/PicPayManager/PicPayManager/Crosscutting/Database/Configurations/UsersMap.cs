using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PicPayManager.Crosscutting.Enum;
using PicPayManager.Crosscutting.ValueObjects.UserAccounts;
using PicPayManager.UserAccounts;
using System.Security.Cryptography.Xml;

namespace PicPayManager.Crosscutting.Database.Configurations;

public class UsersMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.ComplexProperty(s => s.Name,_nameBuilder => {
               _nameBuilder.Property(n => n._value)
                           .IsRequired()
                           .HasColumnType("varchar(100)")
                            .HasColumnName("Name");
            ;
        });

        builder.Property(s => s.Cpf)
               .HasConversion(
                   cpf => cpf.ToString(),
                   value => Cpf.Parse(value))
               .IsRequired()
               .HasColumnType("char(11)")
               .HasColumnName("Cpf");

        builder.HasIndex(s => s.Cpf)
               .IsUnique()
               .HasDatabaseName("IX_Users_Cpf");


        builder.ComplexProperty(s => s.Email, _emailbuilder =>
        {
            _emailbuilder.Property(c => c._value)
                        .IsRequired()
                        .HasColumnType("varchar(254)")
                        .HasColumnName("Email");
        });

        builder.ComplexProperty(s => s.WalletBalance, _wallerbuilder =>
        {
            _wallerbuilder.Property(c => c.Amount)
                          .IsRequired()
                          .HasColumnType("decimal(18,2)")
                          .HasColumnName("WalletBalance")
                          .HasDefaultValue(0m);
        });

        builder.Property(s => s.UserType)
               .HasConversion(new EnumToStringConverter<UserType>())
               .HasColumnType("varchar(30)")
               .IsRequired();
        // 1:n
        builder
              .HasMany(u => u.Transactions)
              .WithOne()
              .HasForeignKey("UserId")
              .OnDelete(DeleteBehavior.Cascade)
              .IsRequired();

        builder
            .Navigation(u => u.Transactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
