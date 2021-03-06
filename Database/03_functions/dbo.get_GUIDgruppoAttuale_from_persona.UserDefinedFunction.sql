USE [dbEmendamenti]
GO
/****** Object:  UserDefinedFunction [dbo].[get_GUIDgruppoAttuale_from_persona]    Script Date: 12/09/2020 18:38:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[get_GUIDgruppoAttuale_from_persona](@UID_persona uniqueidentifier)
RETURNS uniqueidentifier
AS 

BEGIN

	Declare @UID_Gruppo uniqueidentifier;
 
select @UID_Gruppo =
(
	SELECT     JOIN_GRUPPO_AD.UID_Gruppo
)
FROM       (join_persona_gruppi_politici INNER JOIN join_persona_AD on join_persona_gruppi_politici.id_persona=join_persona_AD.id_persona) LEFT JOIN JOIN_GRUPPO_AD ON JOIN_GRUPPO_AD.id_gruppo = join_persona_gruppi_politici.id_gruppo
WHERE 
UID_persona=@UID_persona AND    
((join_persona_gruppi_politici.deleted = 0 AND join_persona_gruppi_politici.data_inizio <= GETDATE() AND join_persona_gruppi_politici.data_fine >= GETDATE()) 
OR
  (join_persona_gruppi_politici.deleted = 0 AND join_persona_gruppi_politici.data_inizio <= GETDATE() AND join_persona_gruppi_politici.data_fine IS NULL)
)		   
    RETURN @UID_Gruppo;
END;
GO
