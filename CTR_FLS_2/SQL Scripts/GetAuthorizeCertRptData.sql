SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Malvey
-- Description:	Gets relevant data points for the authorize release cert report
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-03-25		JAM			Initial
--
-- =============================================
CREATE PROCEDURE [dbo].[GetAuthorizeCertRptData] (
	@PackingSlip		int
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT PKI.QtyPacked, I.Description, II.Description, O.PO, PKIE.Lot, IC.InterchangePartNbr, IC.Revision
	FROM		PackItem PKI 
	INNER JOIN	Item I on PKI.Item = I.Item
	INNER JOIN	ItemInfo II on PKI.Item = II.item
	INNER JOIN	[Order] O on PKI.OrderNbr = O.OrderNbr
	INNER JOIN PackItemExtended PKIE ON  PKI.OrderNbr = PKIE.OrderNbr AND PKI.OrderLine = PKIE.OrderLine AND PKI.Release = PKIE.Release AND PKI.PackingSlip = PKIE.PackingSlip
	INNER JOIN CustomerOrderItem COI ON PKI.OrderNbr = COI.OrderNbr AND PKI.OrderLine = COI.LineNbr AND PKI.Release = COI.Release 
	LEFT OUTER JOIN Interchange IC on PKI.Item = IC.SupplierItemNbr AND COI.CI = IC.InterchangePartNbr AND IC.Status = 'A'
	WHERE	PKI.Packingslip = @PackingSlip

END

GRANT EXECUTE ON GetAuthorizeCertRptData TO PUBLIC


