USE [PaginaIntrodutoria_v2]
GO

/****** Object:  Table [dbo].[tbPaginaIntrodutoria]    Script Date: 24/08/2025 16:28:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbPaginaIntrodutoria](
	[cd_pagina_introdutoria] [int] IDENTITY(1,1) NOT NULL,
	[nm_titulo_pagina_introdutoria] [nvarchar](255) NOT NULL,
	[ds_conteudo_pagina_introdutoria] [nvarchar](max) NULL,
	[nm_url_amigavel_pagina_introdutoria] [nvarchar](255) NULL,
	[cd_tipo_pagina_introdutoria] [int] NOT NULL,
	[cd_pai_pagina_introdutoria] [int] NULL,
	[dt_criacao_pagina_introdutoria] [datetime] NOT NULL,
	[dt_atualizacao_pagina_introdutoria] [datetime] NULL,
	[ck_publicacao_pagina_introdutoria] [bit] NOT NULL,
	[ck_status_pagina_introdutoria] [bit] NOT NULL,
	[cd_ordem_pagina_introdutoria] [int] NOT NULL,
	[nr_versao] [int] NOT NULL,
	[nm_banner_pagina_introdutoria] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_pagina_introdutoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbPaginaIntrodutoria] ADD  DEFAULT (getdate()) FOR [dt_criacao_pagina_introdutoria]
GO

ALTER TABLE [dbo].[tbPaginaIntrodutoria] ADD  DEFAULT ((0)) FOR [ck_publicacao_pagina_introdutoria]
GO

ALTER TABLE [dbo].[tbPaginaIntrodutoria] ADD  DEFAULT ((1)) FOR [ck_status_pagina_introdutoria]
GO

ALTER TABLE [dbo].[tbPaginaIntrodutoria] ADD  DEFAULT ((1)) FOR [nr_versao]
GO

ALTER TABLE [dbo].[tbPaginaIntrodutoria]  WITH CHECK ADD  CONSTRAINT [FK_tbPaginaIntrodutoria_cd_pai] FOREIGN KEY([cd_pai_pagina_introdutoria])
REFERENCES [dbo].[tbPaginaIntrodutoria] ([cd_pagina_introdutoria])
GO

ALTER TABLE [dbo].[tbPaginaIntrodutoria] CHECK CONSTRAINT [FK_tbPaginaIntrodutoria_cd_pai]
GO


