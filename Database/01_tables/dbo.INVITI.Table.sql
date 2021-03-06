USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[INVITI]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[INVITI](
	[UID_Invitante] [uniqueidentifier] NOT NULL,
	[UID_Invitato] [uniqueidentifier] NOT NULL,
	[UID_EM] [uniqueidentifier] NOT NULL,
	[Data_Invito] [datetime] NULL,
	[Data_Visto] [datetime] NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_INVITI] PRIMARY KEY CLUSTERED 
(
	[UID_Invitante] ASC,
	[UID_Invitato] ASC,
	[UID_EM] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[INVITI] ADD  CONSTRAINT [MSmerge_df_rowguid_E4942F0365A44648997AF649F4A96930]  DEFAULT (newsequentialid()) FOR [rowguid]
GO
