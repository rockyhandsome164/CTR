SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		John Malvey
-- Description:	Pulls item test data for the item spec report
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

CREATE PROCEDURE [dbo].[GetItemTestRptData] (
	@Item			varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	Test, Name, TestBolt, BoltSpec, PerformanceSpec, UnitOfMeasure, PassFail, PrintMinMax, SeatLoad, MaxInstall, MinBrkway, CyclesSamples, ResultCycles, Frequency
	FROM	Test WITH (NOLOCK) 
	WHERE	Item = @Item
	ORDER BY Test
END

GRANT EXECUTE ON GetItemTestRptData TO PUBLIC


