USE [CTR_FLS_Dev]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Malvey
-- Description:	Pulls data for the item spec report. This is the driver proc
--				that will return data that the sub reports will use
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-03-26		JAM			Initial
--
-- =============================================

CREATE PROCEDURE [dbo].[GetItemSpecRptData] (
	@ItemStartPattern			varchar(50),
	@ItemEndPattern				varchar(50),
	@ItemStatusList				varchar(10)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ExactItemStart varchar(50),
			@ExactItemEnd	varchar(50)

	-- Because the user will be entering a starting pattern, determine the
	-- actual item the query will pull from. Do the same with the ending item
	SELECT @ExactItemStart = IsNull(MIN(Item), '') FROM Item WHERE Item LIKE (@ItemStartPattern + '%')
	SELECT @ExactItemEnd = IsNull(MAX(Item), '') FROM Item WHERE Item LIKE (@ItemEndPattern + '%')

	SELECT	I.Item, I.Description, II.Description, I.ProductCode, I.Type, I.Status, I.NominalThreadSize
	FROM	Item I WITH (NOLOCK)
	INNER JOIN ItemInfo II WITH (NOLOCK) ON I.Item = II.Item
	WHERE	I.Item BETWEEN @ExactItemStart AND @ExactItemEnd
	AND		I.Status IN (SELECT VALUE FROM OPENJSON(@ItemStatusList ))
	ORDER BY I.Item
END

GRANT EXECUTE ON GetItemSpecRptData TO PUBLIC
