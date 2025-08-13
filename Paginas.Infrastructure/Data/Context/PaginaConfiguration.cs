using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paginas.Domain.Entities;

namespace Paginas.Infrastructure.Data.Configurations
{
    public class PaginaConfiguration : IEntityTypeConfiguration<Pagina>
    {
        public void Configure(EntityTypeBuilder<Pagina> builder)
        {
            builder.ToTable("tbPaginaIntrodutoria");

            builder.HasKey(p => p.Codigo);

            builder.Property(p => p.Codigo)
                .HasColumnName("cd_pagina_introdutoria")
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd(); // <-- importante

            builder.Property(p => p.Titulo)
                .HasColumnName("nm_titulo_pagina_introdutoria")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.Property(p => p.Conteudo)
                .HasColumnName("ds_conteudo_pagina_introdutoria")
                .HasColumnType("text");

            builder.Property(p => p.Url)
                .HasColumnName("nm_url_amigavel_pagina_introdutoria")
                .HasColumnType("varchar(150)");

            builder.Property(p => p.Tipo)
                .HasColumnName("cd_tipo_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(p => p.CdPai)
                .HasColumnName("cd_pai_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(p => p.Criacao)
                .HasColumnName("dt_criacao_pagina_introdutoria")
                .HasColumnType("datetime");

            builder.Property(p => p.Atualizacao)
                .HasColumnName("dt_atualizacao_pagina_introdutoria")
                .HasColumnType("datetime");

            builder.Property(p => p.Publicacao)
                .HasColumnName("ck_publicacao_pagina_introdutoria")
                .HasColumnType("bit");

            builder.Property(p => p.Status)
                .HasColumnName("ck_status_pagina_introdutoria")
                .HasColumnType("bit");

            builder.Property(p => p.Ordem)
                .HasColumnName("cd_ordem_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(p => p.Versao)
                .HasColumnName("nr_versao")
                .HasColumnType("int");

            builder.Property(p => p.Banner)
                .HasColumnName("nm_banner_pagina_introdutoria")
                .HasColumnType("varchar(200)");

            builder.Property(p => p.CdVersao)
                .HasColumnName("cd_versao_pagina_introdutoria")
                .HasColumnType("int");

            // Relacionamento: Pagina -> Botoes (1:N) com cascade para limpeza em testes
            builder.HasMany(p => p.Botoes)
                .WithOne(b => b.Pagina)
                .HasForeignKey(b => b.CdPaginaIntrodutoria)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento páginas pai-filho (auto-relacionamento)
            builder.HasMany(p => p.PaginaFilhos)
                .WithOne(p => p.PaginaPai)
                .HasForeignKey(p => p.CdPai)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
