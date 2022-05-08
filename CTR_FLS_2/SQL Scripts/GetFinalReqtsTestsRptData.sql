
USE [CTR_FLS_Dev]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ===============================================================================
-- Author:		Tom Johnsson
-- Description:	Pulls data for the Required Tests sub-report of the Final
--				Requirements sub-report of the QA Job Log report. 
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----------		---			-------
--		2022-03-30		TCJ			Initial
--
-- ===============================================================================

CREATE OR ALTER PROCEDURE dbo.GetFinalReqtsTestsRptData (
	@Item varchar(50)
)
AS
BEGIN

SET NOCOUNT ON;

select Template, [Name], Frequency, CyclesSamples, SeatLoad, Requirements, TestBolt, PerformanceSpec,
       BoltSpec
from   dbo.Test
where  item = @Item
and    job  = '' 
order by Template

END

GRANT EXECUTE ON dbo.GetFinalReqtsTestsRptData TO PUBLIC
GO