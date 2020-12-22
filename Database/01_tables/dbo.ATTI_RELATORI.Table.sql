USE [dbEmendamenti]
GO
/****** Object:  Table [dbo].[ATTI_RELATORI]    Script Date: 12/09/2020 18:38:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ATTI_RELATORI](
	[UIDAtto] [uniqueidentifier] NOT NULL,
	[UIDPersona] [uniqueidentifier] NOT NULL,
	[sycReplica] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_ATTI_RELATORI] PRIMARY KEY CLUSTERED 
(
	[sycReplica] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ATTI_RELATORI] ADD  CONSTRAINT [DF_ATTI_RELATORI_sycReplica]  DEFAULT (newsequentialid()) FOR [sycReplica]
GO
