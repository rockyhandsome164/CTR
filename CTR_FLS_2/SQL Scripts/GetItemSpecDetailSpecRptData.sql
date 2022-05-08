SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Malvey
-- Description:	Pulls additional data for the item spec report
--				for one of the sub reports contained in the main reprt
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-03-27		JAM			Initial
--
-- =============================================

CREATE PROCEDURE [dbo].[GetItemSpecDetailSpecRptData] (
	@Item			varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	ItemType, Spec, Description
	FROM	ItemSpec WITH (NOLOCK) 
	WHERE	Item = @Item
	ORDER BY ItemType
END

GRANT EXECUTE ON GetItemSpecDetailSpecRptData TO PUBLIC


