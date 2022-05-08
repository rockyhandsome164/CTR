
USE [CTR_FLS_Dev]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ===============================================================================
-- Author:		Tom Johnsson
-- Description:	Pulls data for the Job History Tests sub-report of the Final
--				Requirements sub-report of the QA Job Log report. 
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----------		---			-------
--		2022-03-30		TCJ			Initial
--
-- ===============================================================================

CREATE OR ALTER PROCEDURE dbo.GetFinalReqtsJobHistRptData (
	@Item varchar(50)
)
AS
BEGIN

SET NOCOUNT ON;

select top(10) Occurrence, Revision, Completed, JobInfoEnd, job, NCMR, comments   -- , *
from   JobInfo 
where  Item = @Item
and    JobInfoEnd is not null
and    JobInfoEnd <> ''
and    JobInfoEnd <> '?'
order by convert(int, occurrence) desc

END

GRANT EXECUTE ON dbo.GetFinalReqtsJobHistRptData TO PUBLIC
GO