USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[PINS_NoCons]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PINS_NoCons](
	[UIDPIN] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[UID_persona] [uniqueidentifier] NOT NULL,
	[PIN] [nvarchar](255) NOT NULL,
	[Dal] [datetime] NOT NULL,
	[Al] [datetime] NULL,
	[FIRMA_e_DEPOSITO] [bit] NOT NULL,
	[RichiediModificaPIN] [bit] NOT NULL,
 CONSTRAINT [PK_PINS_NoCons] PRIMARY KEY CLUSTERED 
(
	[UIDPIN] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PINS_NoCons] ADD  CONSTRAINT [DF_PINS_NoCons_UIDPIN]  DEFAULT (newsequentialid()) FOR [UIDPIN]
GO
ALTER TABLE [dbo].[PINS_NoCons] ADD  CONSTRAINT [DF_PINS_NoCons_Dal]  DEFAULT (getdate()) FOR [Dal]
GO
ALTER TABLE [dbo].[PINS_NoCons] ADD  CONSTRAINT [DF_PINS_FIRMA_e_DEPOSITO_]  DEFAULT ((0)) FOR [FIRMA_e_DEPOSITO]
GO
ALTER TABLE [dbo].[PINS_NoCons] ADD  CONSTRAINT [DF_PINS_NoCons_RichiediModificaPIN]  DEFAULT ((0)) FOR [RichiediModificaPIN]
GO
