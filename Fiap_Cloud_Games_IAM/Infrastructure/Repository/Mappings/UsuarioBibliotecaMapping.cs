using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Mappings
{
    public class UsuarioBibliotecaMapping : IEntityTypeConfiguration<UsuarioBiblioteca>
    {
        public void Configure(EntityTypeBuilder<UsuarioBiblioteca> builder)
        {
            builder.ToTable("UsuarioBiblioteca");

            builder.HasKey(ub => ub.Id);

            builder.Property(ub => ub.UsuarioId)
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(ub => ub.JogoExternalId)
                .HasColumnType("UNIQUEIDENTIFIER")
                .IsRequired();

            builder.Property(ub => ub.DataCriacao)
                .HasColumnType("DATETIME2")
                .IsRequired();

            builder.HasOne(ub => ub.Usuario)
                .WithMany(u => u.Biblioteca)
                .HasForeignKey(ub => ub.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
