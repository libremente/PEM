USE [dbEmendamenti]
GO
/****** Object:  UserDefinedFunction [dbo].[get_PIN]    Script Date: 12/09/2020 18:38:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[get_PIN](@Data datetime)
RETURNS varchar(255)
AS 

BEGIN

	Declare @PIN varchar(255);
 
select @PIN =
(
	SELECT     PIN
)
FROM       PINS
WHERE 
(
  (Dal <= @Data AND Al >= @Data) 
OR
  (Dal <= @Data AND al IS NULL)
)		   
    RETURN @PIN;
END;
GO
