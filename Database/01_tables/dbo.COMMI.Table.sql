USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[COMMI]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[COMMI](
	[UIDComma] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UIDAtto] [uniqueidentifier] NOT NULL,
	[UIDArticolo] [uniqueidentifier] NULL,
	[Comma] [varchar](50) NULL,
	[TestoComma] [varchar](max) NULL,
	[Ordine] [int] NULL,
 CONSTRAINT [PK_COMMI] PRIMARY KEY CLUSTERED 
(
	[UIDComma] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[COMMI] ADD  CONSTRAINT [DF_COMMI_UIDComma]  DEFAULT (newsequentialid()) FOR [UIDComma]
GO
ALTER TABLE [dbo].[COMMI] ADD  CONSTRAINT [DF_COMMI_Ordine]  DEFAULT ((0)) FOR [Ordine]
GO
