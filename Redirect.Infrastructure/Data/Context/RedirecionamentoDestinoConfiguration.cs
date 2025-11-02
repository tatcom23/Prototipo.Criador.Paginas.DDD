using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Redirect.Domain.Entities;

namespace Redirect.Infrastructure.Data.Configurations
{
    public class RedirecionamentoDestinoConfiguration : IEntityTypeConfiguration<RedirecionamentoDestino>
    {
        public void Configure(EntityTypeBuilder<RedirecionamentoDestino> builder)
        {
            builder.ToTable("tbRedirecionamentoDestino");

            builder.HasKey(r => r.Codigo);

            builder.Property(r => r.Codigo)
                .HasColumnName("cd_redirecionamento_destino")
                .HasColumnType("int")
                .ValueGeneratedOnAdd();

            builder.Property(r => r.RedirecionamentoOrigemId)
                .HasColumnName("cd_redirecionamento_origem")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(r => r.UrlDestino)
                .HasColumnName("nm_redirecionamento_destino")
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(r => r.DtInicial)
                .HasColumnName("dt_inicial_redirecionamento_destino")
                .HasColumnType("datetime");

            builder.Property(r => r.DtFinal)
                .HasColumnName("dt_final_redirecionamento_destino")
                .HasColumnType("datetime");

            builder.Property(r => r.Ativo)
                .HasColumnName("ck_status_redirecionamento_destino")
                .HasColumnType("bit")
                .HasDefaultValue(true);

            // 🔹 Relacionamento com Origem
            builder.HasOne(r => r.RedirecionamentoOrigem)
                   .WithMany(o => o.Destinos)
                   .HasForeignKey(r => r.RedirecionamentoOrigemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
