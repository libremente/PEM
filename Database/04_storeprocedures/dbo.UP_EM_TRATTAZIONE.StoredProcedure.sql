USE [dbEmendamenti]
GO
/****** Object:  StoredProcedure [dbo].[UP_EM_TRATTAZIONE]    Script Date: 12/09/2020 18:38:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UP_EM_TRATTAZIONE]
   @UIDEM uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @OrdineVotazione int;
    DECLARE @UIDAtto uniqueidentifier;

/*Seleziono e memorizzo l'UID dell'atto a cui appartiene l'EM*/
select @UIDAtto =
(
	SELECT EM.UIDAtto
)
  FROM [EM]
	WHERE EM.UIDEM=@UIDEM

/*Seleziono e memorizzo l'ordine di Votazione attuale dell'EM*/
select @OrdineVotazione =
(
	SELECT EM.OrdineVotazione
)
  FROM [EM]
	WHERE EM.UIDEM=@UIDEM

if (@OrdineVotazione <= 1) return;

/*Aggiorno l'ordine di votazione dell'EM sottraendo 1 a OrdineVotazione e quindi lo porto in alto di una posizione*/
UPDATE EM SET EM.OrdineVotazione=@OrdineVotazione-1 where EM.UIDEM=@UIDEM

/*Dopo l'aggiornamento avrò 2 EM con la stessa numerazione: 1 EM è quello che ho appena spostato mentre l'altro EM è quello che deve essere spostato in basso di una posizione*/
/*Quindi sposto l'altro EM in basso di una posizione sommando 1 a Ordine Votazione*/

UPDATE EM SET EM.OrdineVotazione=@OrdineVotazione where (EM.OrdineVotazione=@OrdineVotazione-1 AND EM.UIDEM<>@UIDEM) AND EM.UIDAtto=@UIDAtto	
	
END
GO
