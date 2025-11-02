using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Redirect.Domain.Entities;

namespace Redirect.Infrastructure.Data.Configurations
{
    public class RedirecionamentoOrigemConfiguration : IEntityTypeConfiguration<RedirecionamentoOrigem>
    {
        public void Configure(EntityTypeBuilder<RedirecionamentoOrigem> builder)
        {
            builder.ToTable("tbRedirecionamentoOrigem");

            builder.HasKey(r => r.Codigo);

            builder.Property(r => r.Codigo)
                .HasColumnName("cd_redirecionamento_origem")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.UrlOrigem)
                .HasColumnName("nm_redirecionamento_origem")
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(r => r.Ativo)
                .HasColumnName("ck_status_redirecionamento_origem")
                .HasColumnType("bit")
                .HasDefaultValue(true);

            builder.Property(r => r.DtRedirecionamento)
                .HasColumnName("dt_redirecionamento_origem")
                .HasColumnType("datetime");

            builder.Property(r => r.DtAtualizacao)
                .HasColumnName("dt_atualizacao_redirecionamento_origem")
                .HasColumnType("datetime");

            builder.HasIndex(r => r.UrlOrigem).IsUnique();

            // 🔹 Relacionamento com os destinos
            builder.HasMany(r => r.Destinos)
                   .WithOne(d => d.RedirecionamentoOrigem)
                   .HasForeignKey(d => d.RedirecionamentoOrigemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
