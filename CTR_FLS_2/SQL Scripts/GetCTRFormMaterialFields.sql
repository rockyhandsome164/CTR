SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls info for material info. This will be used
--				to pull the materials list for the entire CTR or just
--				for one component.
--
--				The MaterialId argument is a delimited list of values in JSON
--				format. So it will return as many materials as are IDs in the list
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-02-18		JAM			Initial
--
-- =============================================

ALTER PROCEDURE [dbo].[GetCTRFormMaterialFields] (
	@MaterialLotList		varchar(1000)
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Create a temp table to import the JSON data into
	-- this is needed to have a column that will preserve
	-- the display order the components need to appear on the form
	DECLARE @MatListTable TABLE (DisplayOrder int, MatLot varchar(50))
	INSERT INTO @MatListTable
	SELECT * FROM OPENJSON (@MaterialLotList) WITH ( DispOrder int '$.Ord', Mat varchar(50) '$.Mat')

	SELECT LI.MaterialSpec, LI.MillNbr, LI.Lot, V.Name
	FROM LotInfo LI WITH (NOLOCK)
	INNER JOIN Vendor V WITH (NOLOCK) on TRIM(LI.VendorId) = V.VendorId 
	INNER JOIN @MatListTable MLT on LI.Lot = MLT.MatLot
	ORDER BY MLT.DisplayOrder

END

GRANT EXECUTE ON GetCTRFormMaterialFields TO PUBLIC
