using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PicPayManager.Crosscutting.Enum;
using PicPayManager.Crosscutting.ValueObjects.Transference;
using PicPayManager.Transference;
using PicPayManager.UserAccounts;


namespace PicPayManager.Crosscutting.Database.Configurations;

public class TransactionMap : IEntityTypeConfiguration<Transactions>
{
    public void Configure(EntityTypeBuilder<Transactions> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);

        builder.ComplexProperty(t => t.TransactionOrigin, _transactionBuilder =>
        {
            _transactionBuilder.Property( t => t._value)
                                .IsRequired()
                                .HasColumnType("varchar(100)")
                                .HasColumnName("TransactionOrigin");
        });

        builder.ComplexProperty(t => t.TransactionDestination, _transactionBuilder =>
        {
            _transactionBuilder.Property(t => t._value)
                                .IsRequired()
                                .HasColumnType("varchar(100)")
                                .HasColumnName("TransactionDestination");
        });

        builder.Property(s => s.TipoPagamento)
               .HasConversion(new EnumToStringConverter<FormasPagamento>())
               .HasColumnType("varchar(30)")
               .IsRequired();
        builder.ComplexProperty(s => s.TransferBalance, _wallerbuilder =>
        {
            _wallerbuilder.Property(c => c._balance)
                          .IsRequired()
                          .HasColumnType("decimal(18,2)")
                          .HasColumnName("TransferBalance")
                          .HasDefaultValue(0m);
        });
    }
}
