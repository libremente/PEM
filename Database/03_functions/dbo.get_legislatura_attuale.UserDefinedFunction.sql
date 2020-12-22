USE [dbEmendamenti]
GO
/****** Object:  UserDefinedFunction [dbo].[get_legislatura_attuale]    Script Date: 12/09/2020 18:38:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[get_legislatura_attuale]()
RETURNS int
AS 

BEGIN

	Declare @idleg int;
 
	select @idleg =
	(
		SELECT id_legislatura from legislature where attiva=1
	) 
	RETURN @idleg
END;
GO
