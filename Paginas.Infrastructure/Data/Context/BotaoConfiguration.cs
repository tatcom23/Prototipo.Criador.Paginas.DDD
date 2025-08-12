using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paginas.Domain.Entities;

namespace Paginas.Infrastructure.Data.Configurations
{
    public class BotaoConfiguration : IEntityTypeConfiguration<Botao>
    {
        public void Configure(EntityTypeBuilder<Botao> builder)
        {
            builder.ToTable("tbBotaoPaginaIntrodutoria");

            builder.HasKey(b => b.Codigo);
            builder.Property(b => b.Codigo)
                .HasColumnName("cd_botao_pagina_introdutoria")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(b => b.Nome)
                .HasColumnName("nm_botao_pagina_introdutoria")
                .HasColumnType("varchar(150)")
                .IsRequired();

            builder.Property(b => b.Link)
                .HasColumnName("ds_link_botao_pagina_introdutoria")
                .HasColumnType("varchar(500)");

            builder.Property(b => b.Tipo)
                .HasColumnName("cd_tipo_botao_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(b => b.CdPaginaIntrodutoria)
                .HasColumnName("cd_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(b => b.Linha)
                .HasColumnName("cd_ordem_linha_botao_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(b => b.Coluna)
                .HasColumnName("cd_ordem_coluna_botao_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(b => b.Criacao)
                .HasColumnName("dt_criacao_botao_pagina_introdutoria")
                .HasColumnType("datetime");

            builder.Property(b => b.Atualizacao)
                .HasColumnName("dt_atualizacao_botao_pagina_introdutoria")
                .HasColumnType("datetime");

            builder.Property(b => b.Status)
                .HasColumnName("ck_status_botao_pagina_introdutoria")
                .HasColumnType("bit");

            builder.Property(b => b.Versao)
                .HasColumnName("cd_versao_botao_pagina_introdutoria")
                .HasColumnType("int");
        }
    }
}
