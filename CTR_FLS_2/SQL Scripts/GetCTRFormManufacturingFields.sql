/****** Object:  StoredProcedure [dbo].[GetCTRFormManufacturingFields]    Script Date: 3/15/2022 3:04:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls manufacturing info
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-03-03		JAM			Initial
--
-- =============================================

CREATE PROCEDURE [dbo].[GetCTRFormManufacturingFields] (
	@LotNbr				varchar(50),
	@SupplierItem		varchar(50)

)
AS
BEGIN
	SET NOCOUNT ON;

	-- Grab just the job  out of the lot #
	DECLARE @JobPortion varchar(25) = LEFT(@LotNbr, CHARINDEX('-', @LotNbr) - 1)

	-- Get the manufacturing date
	DECLARE @MfgDate datetime
	SELECT	@MfgDate = MIN(MT.TransDate)
	FROM	MaterialTrans MT
	INNER JOIN JobLots JL ON MT.Item = JL.ItemCode AND MT.RefRelease = JL.Operation AND MT.RefNbr = @JobPortion
	WHERE	JL.ParentLot = @LotNbr
	AND		JL.MaterialLot = MT.Lot
	AND		MT.RefType = 'J'
	AND		MT.TransType = 'I'
	AND		MT.RefLineSuffix = 0

	SELECT LI.QtyReceived AS MfgQty, @MfgDate AS MfgDate, II.Description, II.ThreadSize
	FROM ItemInfo II WITH (NOLOCK)
	INNER JOIN LotInfo LI WITH (NOLOCK) ON II.Item = LI.Item 
	WHERE II.Item = @SupplierItem
	AND LI.Lot = @LotNbr

END

GRANT EXECUTE ON GetCTRFormManufacturingFields TO PUBLIC
