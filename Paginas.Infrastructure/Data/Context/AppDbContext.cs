using Microsoft.EntityFrameworkCore;
using Paginas.Domain.Entities;

namespace Paginas.Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pagina> Paginas { get; set; }
        public DbSet<Botao> Botoes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da entidade Pagina
            modelBuilder.Entity<Pagina>(entity =>
            {
                entity.ToTable("tbPaginaIntrodutoria");

                entity.HasKey(e => e.Codigo);
                entity.Property(e => e.Codigo).HasColumnName("cd_pagina_introdutoria");
                entity.Property(e => e.Titulo).HasColumnName("nm_titulo_pagina_introdutoria");
                entity.Property(e => e.Conteudo).HasColumnName("ds_conteudo_pagina_introdutoria");
                entity.Property(e => e.Url).HasColumnName("nm_url_amigavel_pagina_introdutoria");
                entity.Property(e => e.Tipo).HasColumnName("cd_tipo_pagina_introdutoria");
                entity.Property(e => e.CdPai).HasColumnName("cd_pai_pagina_introdutoria");
                entity.Property(e => e.Criacao).HasColumnName("dt_criacao_pagina_introdutoria");
                entity.Property(e => e.Atualizacao).HasColumnName("dt_atualizacao_pagina_introdutoria");
                entity.Property(e => e.Publicacao).HasColumnName("ck_publicacao_pagina_introdutoria");
                entity.Property(e => e.Status).HasColumnName("ck_status_pagina_introdutoria");
                entity.Property(e => e.Ordem).HasColumnName("cd_ordem_pagina_introdutoria");
                entity.Property(e => e.Versao).HasColumnName("nr_versao");
                entity.Property(e => e.Banner).HasColumnName("nm_banner_pagina_introdutoria");
                entity.Property(e => e.CdVersao).HasColumnName("cd_versao_pagina_introdutoria");

                // Relacionamento Pagina -> Botao
                entity.HasMany(e => e.Botoes)
                      .WithOne(b => b.Pagina)
                      .HasForeignKey(b => b.CdPaginaIntrodutoria);
            });

            // Configuração da entidade Botao
            modelBuilder.Entity<Botao>(entity =>
            {
                entity.ToTable("tbBotaoPaginaIntrodutoria");

                entity.HasKey(e => e.Codigo);
                entity.Property(e => e.Codigo).HasColumnName("cd_botao_pagina_introdutoria");
                entity.Property(e => e.Nome).HasColumnName("nm_botao_pagina_introdutoria");
                entity.Property(e => e.Link).HasColumnName("ds_link_botao_pagina_introdutoria");
                entity.Property(e => e.Tipo).HasColumnName("cd_tipo_botao_pagina_introdutoria");
                entity.Property(e => e.CdPaginaIntrodutoria).HasColumnName("cd_pagina_introdutoria");
                entity.Property(e => e.Linha).HasColumnName("cd_ordem_linha_botao_pagina_introdutoria");
                entity.Property(e => e.Coluna).HasColumnName("cd_ordem_coluna_botao_pagina_introdutoria");
                entity.Property(e => e.Criacao).HasColumnName("dt_criacao_botao_pagina_introdutoria");
                entity.Property(e => e.Atualizacao).HasColumnName("dt_atualizacao_botao_pagina_introdutoria");
                entity.Property(e => e.Status).HasColumnName("ck_status_botao_pagina_introdutoria");
                entity.Property(e => e.Versao).HasColumnName("cd_versao_botao_pagina_introdutoria");
            });

            // Relacionamento de páginas pai e filhos
            modelBuilder.Entity<Pagina>()
                .HasMany(p => p.PaginaFilhos)
                .WithOne(p => p.PaginaPai)
                .HasForeignKey(p => p.CdPai);
        }
    }
}
