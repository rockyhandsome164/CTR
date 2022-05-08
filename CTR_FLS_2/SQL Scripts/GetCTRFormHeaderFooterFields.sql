/****** Object:  StoredProcedure [dbo].[GetCTRFormHeaderFooterFields]    Script Date: 3/15/2022 1:35:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls info for the header and footer
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-02-17		JAM			Initial
--
-- =============================================

CREATE PROCEDURE [dbo].[GetCTRFormHeaderFooterFields] (
	@LotNbr		varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @JobPortion varchar(25) = LEFT(@LotNbr, CHARINDEX('-', @LotNbr) - 1)
	DECLARE @Suffix varchar(3) = RIGHT(@LotNbr, 1)

	SELECT JI.Job + '-00' + CONVERT(VARCHAR(1), JI.Suffix) AS 'ComponentLot', C.CertNbr, JI.Item AS 'SupplierItemNbr', II.ProductFamily, 'Captain Approver' 'ApproverName', 'Sr. Approver Person' 'ApproverTitle'
	FROM JOBINFO JI WITH (NOLOCK) 
	LEFT OUTER JOIN Cert C WITH (NOLOCK) ON JI.Job + '-00' + CONVERT(VARCHAR(1), JI.Suffix) = C.LotNbr AND C.Revision = (SELECT MAX(Revision) FROM Cert WHERE LotNbr = @LotNbr)
	INNER JOIN ITEMINFO II WITH (NOLOCK) ON JI.Item = II.Item
	WHERE JI.job = @JobPortion
	AND JI.Suffix = @Suffix
END

GRANT EXECUTE ON GetCTRFormHeaderFooterFields TO PUBLIC
