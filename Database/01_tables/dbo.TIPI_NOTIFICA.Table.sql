USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[TIPI_NOTIFICA]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TIPI_NOTIFICA](
	[IDTipo] [int] NOT NULL,
	[Tipo] [varchar](100) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
