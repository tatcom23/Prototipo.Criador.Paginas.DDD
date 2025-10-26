using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Redirect.Domain.Entities;

namespace Redirect.Infrastructure.Data.Configurations
{
    public class RedirectURLConfiguration : IEntityTypeConfiguration<RedirectURL>
    {
        public void Configure(EntityTypeBuilder<RedirectURL> builder)
        {
            // 🔹 Nome da tabela
            builder.ToTable("tbRedirectUrl");

            // 🔹 Chave primária
            builder.HasKey(r => r.Codigo);

            builder.Property(r => r.Codigo)
                .HasColumnName("cd_redirect_url")
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(r => r.UrlAntiga)
                .HasColumnName("nm_antiga_redirect_url")
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(r => r.UrlNova)
                .HasColumnName("nm_nova_redirect_url")
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(r => r.Ativo)
                .HasColumnName("ck_status_redirect_url")
                .HasColumnType("bit")
                .HasDefaultValue(true);

            builder.Property(r => r.DtRedirectUrl)
                .HasColumnName("dt_redirect_url")
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd()
                .IsRequired(false);

            builder.Property(r => r.DtAtualizacao)
                .HasColumnName("dt_atualizacao_redirect_url")
                .HasColumnType("datetime")
                .IsRequired(false);

            // 🔹 Índice único na URL antiga
            builder.HasIndex(r => r.UrlAntiga)
                   .IsUnique();
        }
    }
}
