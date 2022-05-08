
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	Pulls data for QA Job Cards
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-03-31		JAM			Initial
--
-- =============================================

CREATE PROCEDURE [dbo].[GetQAJobCardData] (
	@LotNbr		varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @JobPortion varchar(25) = LEFT(@LotNbr, CHARINDEX('-', @LotNbr) - 1)
	DECLARE @Suffix varchar(3) = RIGHT(@LotNbr, 1)

	SELECT	T.Template, I.Description, T.PerformanceSpec, I.CharFld3, I.NominalThreadSize, T.TestBolt, II.Description, T.MinBrkway, T.UnitOfMeasure
	FROM    JobInfo J WITH (NOLOCK)
	INNER JOIN	Test T  WITH (NOLOCK) ON J.Item = T.Item
	INNER JOIN	Item I  WITH (NOLOCK) ON J.Item = I.Item
	INNER JOIN	ItemInfo II  WITH (NOLOCK) ON J.Item = II.item
	WHERE J.Job = @JobPortion
	AND J.Suffix = 	@Suffix	
	AND T.Template IN (20,21,22,60,61,71,80,81,82,100,140,150,151,170,171,172,190,191,202,207,209,211,214,215,227,228)
	ORDER BY T.Template

END

GRANT EXECUTE ON GetQAJobCardData TO PUBLIC