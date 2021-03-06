USE [dbEmendamenti]
GO
/****** Object:  UserDefinedFunction [dbo].[get_ProgressivoEM]    Script Date: 12/09/2020 18:38:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[get_ProgressivoEM](@UIDAtto uniqueidentifier, @id_gruppo int)
RETURNS int
AS
BEGIN

	Declare @progressivo int;
	set @progressivo = 1;
 
select @progressivo =
(
	SELECT COUNT(*)+1
FROM EM e
WHERE e.UIDAtto=@UIDAtto AND id_gruppo=@id_gruppo AND e.SubProgressivo is null
) 
    RETURN @progressivo;
END;
GO
