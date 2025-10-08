using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Mappings
{
    public class JogoMapping : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.ToTable("Jogo");

            builder
                .HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnType("INT");

            builder
                .Property(p => p.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(j => j.ExternalId)
                .HasColumnName("ExternalId")
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            builder.Property(j => j.Nome)
                .HasColumnName("Nome")
                .HasColumnType("NVARCHAR(200)")
                .IsRequired();

            builder.Property(j => j.Produtora)
                .HasColumnName("Produtora")
                .HasColumnType("NVARCHAR(200)")
                .IsRequired();

            builder.Property(j => j.Genero)
                .HasColumnName("genero")
                .HasColumnType("NVARCHAR(200)");


            builder.Property(j => j.Valor)
                .HasColumnName("Valor")
                .HasColumnType("DECIMAL(10,2)")
                .IsRequired();
        }
    }
}
