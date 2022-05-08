SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls info for the test spec fields
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-02-25		JAM			Initial
--
-- =============================================

ALTER PROCEDURE [dbo].[GetCTRFormTestSpecFields] (
	@ItemNbr		varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @JobPortion varchar(25) = LEFT(@LotNbr, CHARINDEX('-', @LotNbr) - 1)
	DECLARE @Suffix varchar(3) = RIGHT(@LotNbr, 1)

	DECLARE @RowsWithRowNbr TABLE (RowNbr int, SpecInfo varchar(350))
	DECLARE @WorkTable TABLE (RowNbr int, SlotNbr int, SpecInfo varchar(350))
	DECLARE @FinalResult TABLE (RowNbr int, SpecInfoSlotOne varchar(350), SpecInfoSlotTwo varchar(350))

	INSERT INTO @RowsWithRowNbr
	SELECT ROW_NUMBER() OVER (PARTITION BY  Item ORDER BY Item), SpecType + '|' + Specification
	FROM	TestSpec WITH (NOLOCK)
	WHERE	Item = @ItemNbr	
	AND		Job = @JobPortion
	AND		Suffix = @Suffix

	-- load the work table
	INSERT INTO @WorkTable
	SELECT  CEILING(CAST( RowNbr / 2.0 AS decimal(4, 2))),
			-- this calcs the right slot on each row
			CASE
				WHEN 1 % (RowNbr - ((CEILING(CAST( RowNbr / 2.0 AS decimal(4, 2))) * 2) - 2)) = 0 THEN 1
				WHEN 2 % (RowNbr - ((CEILING(CAST( RowNbr / 2.0 AS decimal(4, 2))) * 2) - 2)) = 0 THEN 2
			END,
			SpecInfo
	FROM @RowsWithRowNbr 
	ORDER BY RowNbr

	-- now flatten out to a 2-up format to display properly on the report
	-- establish a flatten row based on every slot 1 part we have
	INSERT INTO @FinalResult (RowNbr, SpecInfoSlotOne)
	SELECT	RowNbr, SpecInfo
	FROM	@WorkTable
	WHERE	SlotNbr = 1

	UPDATE @FinalResult
	SET		SpecInfoSlotTwo = WT.SpecInfo
	FROM	@FinalResult FR
	INNER JOIN @WorkTable WT ON WT.RowNbr = FR.RowNbr
	WHERE	WT.SlotNbr = 2

	-- And finally pull all the data back
	SELECT	SpecInfoSlotOne,
			IsNull(SpecInfoSlotTwo, '') AS SpecInfoSlotTwo
	FROM	@FinalResult

END

GRANT EXECUTE ON GetCTRFormTestSpecFields TO PUBLIC
