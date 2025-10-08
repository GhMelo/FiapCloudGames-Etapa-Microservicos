using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Mappings
{
    public class PromocaoMapping : IEntityTypeConfiguration<Promocao>
    {
        public void Configure(EntityTypeBuilder<Promocao> builder)
        {
            builder.ToTable("Promocao");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.JogoId)
                .HasColumnName("JogoId")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(p => p.NomePromocao)
                .HasColumnName("NomePromocao")
                .HasColumnType("NVARCHAR(200)")
                .IsRequired();

            builder.Property(p => p.PorcentagemDesconto)
                .HasColumnName("PorcentagemDesconto")
                .HasColumnType("DECIMAL(5,2)")
                .IsRequired();

            builder.Property(p => p.DataInicio)
                .HasColumnName("DataInicio")
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(p => p.DataFim)
                .HasColumnName("DataFim")
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.Property(p => p.Status)
                .HasColumnName("Status")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(j => j.ExternalId)
                .HasColumnName("ExternalId")
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            builder.Property(p => p.JogoId)
                .HasColumnName("JogoId")
                .HasColumnType("INT")
                .IsRequired();

            builder.HasOne(p => p.JogoPromocao)
                .WithMany(p => p.PromocoesAderidas)
                .HasForeignKey(p => p.JogoId);
        }
    }
}
