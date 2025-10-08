using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paginas.Domain.Entities;

namespace Paginas.Infrastructure.Data.Configurations
{
    public class CarrosselImagemConfiguration : IEntityTypeConfiguration<CarrosselImagem>
    {
        public void Configure(EntityTypeBuilder<CarrosselImagem> builder)
        {
            builder.ToTable("tbCarrosselImagem");

            builder.HasKey(i => i.Codigo);

            builder.Property(i => i.Codigo)
                .HasColumnName("cd_imagem")
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(i => i.CdCarrossel)
                .HasColumnName("cd_carrossel")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(i => i.Titulo)
                .HasColumnName("nm_titulo_imagem")
                .HasColumnType("nvarchar(255)");

            builder.Property(i => i.Descricao)
                .HasColumnName("ds_imagem")
                .HasColumnType("nvarchar(500)");

            builder.Property(i => i.UrlImagem)
                .HasColumnName("nm_url_imagem")
                .HasColumnType("nvarchar(500)")
                .IsRequired();

            builder.Property(i => i.Ordem)
                .HasColumnName("cd_ordem_imagem")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(i => i.Ativo)
                .HasColumnName("ck_ativo_imagem")
                .HasColumnType("bit")
                .IsRequired();

            builder.Property(i => i.Criacao)
                .HasColumnName("dt_criacao_imagem")
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(i => i.Alteracao)
                .HasColumnName("dt_alteracao_imagem")
                .HasColumnType("datetime");

            // Relacionamento: Imagem -> Carrossel
            builder.HasOne(i => i.Carrossel)
                .WithMany(c => c.Imagens)
                .HasForeignKey(i => i.CdCarrossel)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

