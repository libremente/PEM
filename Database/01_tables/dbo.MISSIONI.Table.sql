USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[MISSIONI]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MISSIONI](
	[UIDMissione] [uniqueidentifier] NOT NULL,
	[NMissione] [int] NOT NULL,
	[DAL] [date] NOT NULL,
	[AL] [date] NULL,
	[Desccrizione] [varchar](250) NOT NULL,
	[Ordine] [int] NOT NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_MISSIONI_1] PRIMARY KEY CLUSTERED 
(
	[UIDMissione] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[MISSIONI] ADD  CONSTRAINT [DF_MISSIONI_UID_Missione]  DEFAULT (newid()) FOR [UIDMissione]
GO
ALTER TABLE [dbo].[MISSIONI] ADD  CONSTRAINT [MSmerge_df_rowguid_EFC478CAEE8D4BCB821CA4ABB4AED949]  DEFAULT (newsequentialid()) FOR [rowguid]
GO
