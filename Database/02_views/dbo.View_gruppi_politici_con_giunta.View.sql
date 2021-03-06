USE [dbEmendamenti]
GO
/****** Object:  View [dbo].[View_gruppi_politici_con_giunta]    Script Date: 12/09/2020 18:38:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_gruppi_politici_con_giunta]
AS
(SELECT        id_gruppo, nome_gruppo, codice_gruppo
FROM            dbo.gruppi_politici)
UNION
(SELECT        id_gruppo, 'GIUNTA REGIONALE' AS nome_gruppo, 'GIUNTA REGIONALE' AS codice_gruppo
 FROM            JOIN_GRUPPO_AD
 WHERE        GiuntaRegionale = 1)
GO
