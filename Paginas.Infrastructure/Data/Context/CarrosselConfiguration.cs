using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paginas.Domain.Entities;

namespace Paginas.Infrastructure.Data.Configurations
{
    public class CarrosselConfiguration : IEntityTypeConfiguration<Carrossel>
    {
        public void Configure(EntityTypeBuilder<Carrossel> builder)
        {
            builder.ToTable("tbCarrossel");

            builder.HasKey(c => c.Codigo);

            builder.Property(c => c.Codigo)
                .HasColumnName("cd_carrossel")
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(c => c.CdPagina)
                .HasColumnName("cd_pagina_introdutoria")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.Titulo)
                .HasColumnName("nm_titulo_carrossel")
                .HasColumnType("nvarchar(255)");

            builder.Property(c => c.Descricao)
                .HasColumnName("ds_carrossel")
                .HasColumnType("nvarchar(500)");

            builder.Property(c => c.Ativo)
                .HasColumnName("ck_ativo_carrossel")
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(c => c.Criacao)
                .HasColumnName("dt_criacao_carrossel")
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.Alteracao)
                .HasColumnName("dt_alteracao_carrossel")
                .HasColumnType("datetime");

            // Relacionamento: Carrossel -> Imagens (1:N)
            builder.HasMany(c => c.Imagens)
                .WithOne(i => i.Carrossel)
                .HasForeignKey(i => i.CdCarrossel)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento: Carrossel -> Página
            builder.HasOne(c => c.Pagina)
                .WithMany(p => p.Carrosseis)
                .HasForeignKey(c => c.CdPagina)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
