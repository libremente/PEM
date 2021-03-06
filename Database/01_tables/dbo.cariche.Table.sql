USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[cariche]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[cariche](
	[id_carica] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[nome_carica] [varchar](250) NOT NULL,
	[ordine] [int] NOT NULL,
	[tipologia] [varchar](20) NOT NULL,
	[presidente_gruppo] [bit] NULL,
 CONSTRAINT [PK_cariche] PRIMARY KEY CLUSTERED 
(
	[id_carica] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
