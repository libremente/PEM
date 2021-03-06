USE [dbEmendamenti]
GO
/****** Object:  UserDefinedFunction [dbo].[consigliere_attivo]    Script Date: 12/09/2020 18:38:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[consigliere_attivo](@id_persona int)
RETURNS bit
AS 

BEGIN

	Declare @id_rec int;
	Declare @res bit;
 
select @id_rec =
(
	SELECT [id_rec]
)
  FROM [dbEmendamenti].[dbo].[join_persona_organo_carica]
  WHERE deleted=0 and id_persona=@id_persona and (id_carica=4 or id_carica=36 or id_carica=100) and ((GETDATE() between data_inizio and data_fine) or (data_inizio<=GETDATE() and data_fine is null))	   

if @id_rec is not null select @res=1 else select @res=0;

RETURN @res;

END;
GO
