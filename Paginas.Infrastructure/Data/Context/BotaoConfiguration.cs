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
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(b => b.Nome)
                .HasColumnName("nm_botao_pagina_introdutoria")
                .HasColumnType("nvarchar(255)");

            builder.Property(b => b.Link)
                .HasColumnName("ds_link_botao_pagina_introdutoria")
                .HasColumnType("nvarchar(255)");

            builder.Property(b => b.Tipo)
                .HasColumnName("cd_tipo_botao_pagina_introdutoria")
                .HasColumnType("int");

            builder.Property(b => b.CdPaginaIntrodutoria)
                .HasColumnName("cd_pagina_introdutoria")
                .HasColumnType("int");

            // ✅ Mapeia Ordem para a nova coluna
            builder.Property(b => b.Ordem)
                .HasColumnName("cd_ordem_botao_pagina_introdutoria")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(b => b.Criacao)
                .HasColumnName("dt_criacao_botao_pagina_introdutoria")
                .HasColumnType("datetime");

            builder.Property(b => b.Atualizacao)
                .HasColumnName("dt_atualizacao_botao_pagina_introdutoria")
                .HasColumnType("datetime");

            builder.Property(b => b.Status)
                .HasColumnName("ck_status_botao_pagina_introdutoria")
                .HasColumnType("bit");

            // Relacionamento
            builder.HasOne(b => b.Pagina)
                   .WithMany(p => p.Botoes)
                   .HasForeignKey(b => b.CdPaginaIntrodutoria)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}