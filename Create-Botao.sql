USE [PaginaIntrodutoria_v2]
GO

/****** Object:  Table [dbo].[tbBotaoPaginaIntrodutoria]    Script Date: 24/08/2025 16:27:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbBotaoPaginaIntrodutoria](
	[cd_botao_pagina_introdutoria] [int] IDENTITY(1,1) NOT NULL,
	[nm_botao_pagina_introdutoria] [nvarchar](255) NOT NULL,
	[ds_link_botao_pagina_introdutoria] [nvarchar](255) NOT NULL,
	[cd_tipo_botao_pagina_introdutoria] [int] NOT NULL,
	[cd_pagina_introdutoria] [int] NOT NULL,
	[ck_status_botao_pagina_introdutoria] [bit] NOT NULL,
	[dt_criacao_botao_pagina_introdutoria] [datetime] NOT NULL,
	[dt_atualizacao_botao_pagina_introdutoria] [datetime] NULL,
	[cd_ordem_botao_pagina_introdutoria] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[cd_botao_pagina_introdutoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbBotaoPaginaIntrodutoria] ADD  DEFAULT ((1)) FOR [ck_status_botao_pagina_introdutoria]
GO

ALTER TABLE [dbo].[tbBotaoPaginaIntrodutoria] ADD  DEFAULT (getdate()) FOR [dt_criacao_botao_pagina_introdutoria]
GO

ALTER TABLE [dbo].[tbBotaoPaginaIntrodutoria]  WITH CHECK ADD  CONSTRAINT [FK_tbBotaoPaginaIntrodutoria_cd_pagina_introdutoria] FOREIGN KEY([cd_pagina_introdutoria])
REFERENCES [dbo].[tbPaginaIntrodutoria] ([cd_pagina_introdutoria])
GO

ALTER TABLE [dbo].[tbBotaoPaginaIntrodutoria] CHECK CONSTRAINT [FK_tbBotaoPaginaIntrodutoria_cd_pagina_introdutoria]
GO


