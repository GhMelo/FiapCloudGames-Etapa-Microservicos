using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Mappings
{
    public class PagamentoMapping : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamento");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.CompraId)
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(p => p.ValorPago)
                .HasColumnType("DECIMAL(10,2)")
                .IsRequired();

            builder.Property(p => p.DataPagamento)
                .HasColumnType("DATETIME2")
                .HasDefaultValueSql("SYSDATETIME()")
                .IsRequired();

            builder.Property(p => p.Metodo)
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            builder.Property(p => p.Status)
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.HasOne(p => p.Compra)
                   .WithMany(c => c.Pagamentos)
                   .HasForeignKey(p => p.CompraId);
        }
    }
}
