SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls info for the notes data (special proc req)
--				for the CTR form
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-02-21		JAM			Initial
--
-- =============================================

ALTER PROCEDURE [dbo].[GetCTRFormNotesData] (
	@LotNbr		varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @JobPortion varchar(25) = LEFT(@LotNbr, CHARINDEX('-', @LotNbr) - 1)
	DECLARE @Suffix varchar(3) = RIGHT(@LotNbr, 1)

	SELECT	Notes
	FROM	JobInfo JI WITH (NOLOCK)
	INNER JOIN	Note N WITH (NOLOCK) ON JI.CertTextKey = N.TextKey
	WHERE	JI.Job = @JobPortion

END
GRANT EXECUTE ON GetCTRFormNotesData TO PUBLIC
