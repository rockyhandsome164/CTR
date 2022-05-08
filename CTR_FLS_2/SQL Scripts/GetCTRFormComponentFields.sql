SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls info for the component
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-02-25		JAM			Initial
--
-- =============================================

CREATE  PROCEDURE [dbo].[GetCTRFormComponentFields] (
	@LotNbr			varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT JI.Job + '-00' + CONVERT(VARCHAR(1), JI.Suffix) AS 'ComponentLot', JI.Item AS 'SupplierItemNbr', I.Description, (SELECT COUNT(*) FROM TestResult WHERE JOB = @JobPortion AND Suffix = @Suffix) AS 'TestCount'
	FROM JOBINFO JI WITH (NOLOCK) 
	INNER JOIN ITEM I WITH (NOLOCK) ON JI.Item = I.Item
	WHERE JI.job = @JobPortion
	AND JI.Suffix = @Suffix
END

GRANT EXECUTE ON GetCTRFormComponentFields TO PUBLIC
