using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Mappings
{
    public class CompraMapping : IEntityTypeConfiguration<Compra>
    {
        public void Configure(EntityTypeBuilder<Compra> builder)
        {
            builder.ToTable("Compra");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.UsuarioExternalId)
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            builder.Property(c => c.JogoExternalId)
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            builder.Property(c => c.PromocaoExternalId)
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired(false);

            builder.Property(c => c.ValorCompra)
                .HasColumnType("DECIMAL(10,2)")
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType("NVARCHAR(50)")
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.HasMany(c => c.Pagamentos)
                   .WithOne(p => p.Compra)
                   .HasForeignKey(p => p.CompraId);
        }
    }
}
