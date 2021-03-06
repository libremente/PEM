USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[RUOLI_UTENTE]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RUOLI_UTENTE](
	[UID_ruolo_utente] [uniqueidentifier] NOT NULL,
	[UID_persona] [uniqueidentifier] NOT NULL,
	[IDruolo] [int] NOT NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_RUOLI_UTENTE] PRIMARY KEY CLUSTERED 
(
	[UID_ruolo_utente] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RUOLI_UTENTE] ADD  CONSTRAINT [MSmerge_df_rowguid_1E1036453C7D44B78DED15600325EDAA]  DEFAULT (newsequentialid()) FOR [rowguid]
GO
