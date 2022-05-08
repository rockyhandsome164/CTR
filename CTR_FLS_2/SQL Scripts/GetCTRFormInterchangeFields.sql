SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls info for the interchange fields
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-02-17		JAM			Initial
--
-- =============================================

ALTER PROCEDURE [dbo].[GetCTRFormInterchangeFields] (
	@SupplierItemNbr		varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RowsWithRowNbr TABLE (RowNbr int, PartNbr varchar(50))
	DECLARE @WorkTable TABLE (RowNbr int, SlotNbr int, PartNbr varchar(50))
	DECLARE @FinalResult TABLE (RowNbr int, PartNbrSlotOne varchar(50), PartNbrSlotTwo varchar(50), PartNbrSlotThree varchar(50))

	INSERT INTO @RowsWithRowNbr
	SELECT ROW_NUMBER() OVER (PARTITION BY SupplierItemNbr ORDER BY SupplierItemNbr), InterchangePartNbr + ' ' + CASE WHEN IsNull(Revision, '') = '' THEN '' ELSE 'Rev ' + Revision END
	FROM	Interchange
	WHERE	SupplierItemNbr = @SupplierItemNbr	

	-- load the work table
	INSERT INTO @WorkTable
	SELECT  CEILING(CAST( RowNbr / 3.0 AS decimal(3, 2))),
			-- this calcs the right slot on each row
			CASE
				WHEN 1 % (RowNbr - ((CEILING(CAST( RowNbr / 3.0 AS decimal(3, 2))) * 3) - 3)) = 0 THEN 1
				WHEN 2 % (RowNbr - ((CEILING(CAST( RowNbr / 3.0 AS decimal(3, 2))) * 3) - 3)) = 0 THEN 2
				WHEN 3 % (RowNbr - ((CEILING(CAST( RowNbr / 3.0 AS decimal(3, 2))) * 3) - 3)) = 0 THEN 3
			END,
			PartNbr
	FROM @RowsWithRowNbr 
	ORDER BY RowNbr

	-- now flatten out to a 3-up format to display properly on the report
	-- establish a flatten row based on every slot 1 part we have
	INSERT INTO @FinalResult (RowNbr, PartNbrSlotOne)
	SELECT	RowNbr, PartNbr
	FROM	@WorkTable
	WHERE	SlotNbr = 1

	UPDATE @FinalResult
	SET		PartNbrSlotTwo = WT.PartNbr
	FROM	@FinalResult FR
	INNER JOIN @WorkTable WT ON WT.RowNbr = FR.RowNbr
	WHERE	WT.SlotNbr = 2

	UPDATE @FinalResult
	SET		PartNbrSlotThree = WT.PartNbr
	FROM	@FinalResult FR
	INNER JOIN @WorkTable WT ON WT.RowNbr = FR.RowNbr
	WHERE	WT.SlotNbr = 3

	-- And finally pull all the data back
	SELECT	PartNbrSlotOne,
			PartNbrSlotTwo,
			PartNbrSlotThree
	FROM	@FinalResult
END

GRANT EXECUTE ON GetCTRFormInterchangeFields TO PUBLIC
