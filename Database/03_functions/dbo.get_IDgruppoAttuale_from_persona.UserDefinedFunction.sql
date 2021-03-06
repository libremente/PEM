USE [dbEmendamenti]
GO
/****** Object:  UserDefinedFunction [dbo].[get_IDgruppoAttuale_from_persona]    Script Date: 12/09/2020 18:38:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[get_IDgruppoAttuale_from_persona](@UID_persona uniqueidentifier, @Giunta bit = 0)
RETURNS int
AS 

BEGIN

	Declare @id_Gruppo int;
 
if (@Giunta = 1)
  BEGIN
	select @id_Gruppo =
	(
		SELECT    id_gruppo
	)
	FROM   JOIN_GRUPPO_AD left join legislature on JOIN_GRUPPO_AD.id_legislatura = legislature.id_legislatura
	WHERE attiva = 1 AND GiuntaRegionale = 1
  END
IF (@Giunta = 0 or @Giunta is null)
  BEGIN
	select @id_Gruppo =
	(
		SELECT     join_persona_gruppi_politici.id_gruppo
	)
	FROM       (join_persona_gruppi_politici INNER JOIN join_persona_AD on join_persona_gruppi_politici.id_persona=join_persona_AD.id_persona)
	WHERE 
	UID_persona=@UID_persona AND    
	((join_persona_gruppi_politici.deleted = 0 AND join_persona_gruppi_politici.data_inizio <= GETDATE() AND join_persona_gruppi_politici.data_fine >= GETDATE()) 
	OR
	  (join_persona_gruppi_politici.deleted = 0 AND join_persona_gruppi_politici.data_inizio <= GETDATE() AND join_persona_gruppi_politici.data_fine IS NULL)
	)
  END
RETURN @id_gruppo;
END;
GO
