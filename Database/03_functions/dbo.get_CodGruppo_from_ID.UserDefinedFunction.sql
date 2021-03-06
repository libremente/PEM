USE [dbEmendamenti]
GO
/****** Object:  UserDefinedFunction [dbo].[get_CodGruppo_from_ID]    Script Date: 12/09/2020 18:38:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[get_CodGruppo_from_ID] 
(
	-- Add the parameters for the function here
	@ID_gruppo int
)
RETURNS varchar(255)
AS
BEGIN

	Declare @Cod_Gruppo varchar(255);
 
select @Cod_Gruppo =
(
	SELECT     View_gruppi_politici_con_giunta.codice_gruppo
)
FROM       dbo.View_gruppi_politici_con_giunta
WHERE 
View_gruppi_politici_con_giunta.id_gruppo=@ID_gruppo
    RETURN @Cod_Gruppo;
END;
GO
