using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Mappings
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.ExternalId)
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            builder.Property(p => p.DataCriacao)
                .HasColumnName("DataCriacao")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(u => u.Email)
                .HasColumnType("NVARCHAR(200)")
                .IsRequired();

            builder.Property(u => u.Nome)
                .HasColumnType("NVARCHAR(200)")
                .IsRequired();

            builder.Property(u => u.Senha)
                .HasColumnType("NVARCHAR(500)")
                .IsRequired();

            builder.Property(u => u.Tipo)
                .HasColumnType("INT")
                .IsRequired();

            builder.HasMany(u => u.Biblioteca)
                   .WithOne(ub => ub.Usuario)
                   .HasForeignKey(ub => ub.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
