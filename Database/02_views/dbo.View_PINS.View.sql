USE [dbEmendamenti]
GO
/****** Object:  View [dbo].[View_PINS]    Script Date: 12/09/2020 18:38:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_PINS]
AS
(SELECT     dbo.PINS.*
FROM         dbo.PINS)
UNION
(SELECT     dbo.PINS_NoCons.*
 FROM         dbo.PINS_NoCons)
GO
