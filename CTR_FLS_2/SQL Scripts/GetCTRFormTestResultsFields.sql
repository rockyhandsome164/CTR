SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	One of multiple procs used to generate the CTR FLS form
--				This proc pulls test results and sample data
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-02-27		JAM			Initial
--
-- =============================================

CREATE PROCEDURE [dbo].[GetCTRFormTestResultsFields] (
	@LotNbr			varchar(25),
	@LotSuffix		varchar(5)
)
AS
BEGIN
	SET NOCOUNT ON;

DECLARE @TestResultsInfo TABLE	(RowNbr int,
								 TestNbr varchar(3),
								 TestInfo varchar(255),
								 UOM varchar(25),
								 SampleNbr int,
								 ResultCycle int,
								 MinVal decimal(15,2),
								 MaxVal decimal(15,2),
								 PassFail varchar(25),
								 MinReq decimal(15,2),
								 MaxReq decimal(15,2),
								 Requirements varchar(255),
								 MaxDisplayFlag varchar(50),
								 MinDisplayFlag varchar(50),
								 PassFailDisplayFlag varchar(25))

-- This table is used to indentify test that have at least two rows.
-- Necessary because we may have requirements data points that need 
-- to be added to the second row of the test results. This table
-- helps in determining if we need to manually add that second row
-- in case the test only has one row of data
DECLARE @TestsWithAtLeastTwoRows TABLE (TestNbr varchar(3), TestInfo varchar(255))

-- For certain tests that involve torque, additional info may be required
-- Store that info in this interim table and update testresultinfo table if needed
DECLARE @TorqueInfo TABLE (TestNbr varchar(3), BoltSpec varchar(50))

-- For certain tests that require reporting of stat calcs
-- Store that info in this interim table and update testresultinfo table if needed
DECLARE @StatCalcInfo TABLE (TestNbr varchar(3), TestInfoRow int, StatCalc varchar(100))

-- First pull the sample and test results data in	
-- Row number is important to determine where each tests starts in the list
-- Partition values provide the break so each unique test starts with a row nbr of 1
INSERT INTO @TestResultsInfo
SELECT ROW_NUMBER() OVER (PARTITION BY TR.Test ORDER BY TR.Test, CONVERT(int, S.Sample), CONVERT(int, S.ResultCycle)), 
		TR.Test, 
		(TR.Test + ' - ' + TR.Template + ' ' + TR.Name),
		TR.UnitOfMeasure,
		S.Sample,
		S.ResultCycle,
		S.Minimum,
		S.Maximum,
		S.PassFail,
		TR.MinBrkway,
		TR.MaxInstall,
		TR.Requirements,
		CASE  WHEN UPPER(TR.Name) = 'HARDNESS' THEN 'no' ELSE TR.MaxValueRequired END,
		CASE  WHEN UPPER(TR.Name) = 'HARDNESS' THEN 'no' ELSE TR.MinValueRequired END,
		TR.PassFail_A
FROM TestResult TR WITH (NOLOCK)
INNER JOIN Sample S  WITH (NOLOCK) ON TR.Cert = S.Cert AND TR.Test = S.Test
WHERE TR.Job = @LotNbr
AND TR.Suffix = CONVERT(int, @LotSuffix)
ORDER BY TR.Test, S.Sample, S.ResultCycle

-- Make sure all tests have two rows
-- First grab all tests that HAVE two rows
INSERT INTO @TestsWithAtLeastTwoRows
SELECT DISTINCT TestNbr, TestInfo
	 FROM @TestResultsInfo
	 WHERE RowNbr = 2

-- Now look for any tests not in the above list and insert rows
-- Yeah, this could be consolidated into a single insert but
-- wanted to keep things separated for now
INSERT INTO @TestResultsInfo
SELECT DISTINCT 2, TestNbr, TestInfo, '', 0, 0, 0, 0, '', 0, 0, Requirements, '', '', '' 
	 FROM @TestResultsInfo
	 WHERE RowNbr = 1
	 AND TestInfo NOT IN (SELECT TestInfo FROM @TestsWithAtLeastTwoRows)

-- Blank out data in the info column for all but the first row
-- The other rows may contain additional info
-- This assists in how the data is displayed on the CTR report.
UPDATE @TestResultsInfo
SET		TestInfo = ''
WHERE	RowNbr <> 1

-- Now put any requirements data into the 2nd row in the info col for each test
-- Als used for display purposes
UPDATE	@TestResultsInfo
SET		TestInfo = Requirements
FROM	@TestResultsInfo TI
WHERE	TI.RowNbr = 2	-- this data appears on the second row

-- Check for any special messaging needed for torque tests
-- First see if this lot has the requirements to report these messages
INSERT INTO @TorqueInfo
SELECT	Test, BoltSpec FROM test WHERE job = @LotNbr AND suffix = @LotSuffix AND UPPER(NAME) = 'TORQUE TEST' AND PerformanceSpec LIKE '%BPS-N-70%'

-- Check if we got any rows from the previous query
IF (SELECT COUNT(*) FROM @TorqueInfo) > 0
BEGIN
	-- Since we have data, insert the additional text on lines 3 & 4
	-- Match on test number
	UPDATE	@TestResultsInfo
	SET		TestInfo = 'TORQUE TEST BOLT CHAR:'
	FROM	@TestResultsInfo TI
	INNER JOIN @TorqueInfo TQI ON TI.TestNbr = TQI.TestNbr
	WHERE	TI.RowNbr = 3

	UPDATE	@TestResultsInfo
	SET		TestInfo = TQI.BoltSpec
	FROM	@TestResultsInfo TI
	INNER JOIN @TorqueInfo TQI ON TI.TestNbr = TQI.TestNbr
	WHERE	TI.RowNbr = 4
END

/* SPECIAL HANDLING FOR STAT CALCS */
DECLARE @TestNbrForStatCalc int
SELECT @TestNbrForStatCalc = Test
	FROM JobLots JL WITH (NOLOCK)
	INNER JOIN Test T WITH (NOLOCK) ON T.Item = JL.ItemCode
	WHERE JL.ComponentLot = @LotNbr + '-' + @LotSuffix 
	AND (JL.ItemCode = 'C1F062' OR JL.ItemCode = 'C1F067') 
	AND T.Template in (10, 11)
IF @@ROWCOUNT > 0
BEGIN
	-- Calc the average breakaway value
	DECLARE @AvgBrkWay decimal(9,2)
	SELECT	@AvgBrkWay = IsNull(AVG(MinVal), 0)
	FROM	@TestResultsInfo
	WHERE	ResultCycle = 1
	AND		TestNbr = @TestNbrForStatCalc
	GROUP BY TestNbr

	-- Calculate the kval - pretty straightforward  :-)
	-- There is a spreadsheet attached to CR23662 that contains examples of how this is calculated step-by-step
	-- The calc was consolidated into a single statement rather than breaking each calc separately
	DECLARE @KVal decimal(6,3)
	SELECT	@KVal = (@AvgBrkWay - 2)/(SQRT(SUM((TI.MinVal - @AvgBrkWay) * (TI.MinVal - @AvgBrkWay)) / (MAX(SampleNbr) - 1)))
	FROM	@TestResultsInfo TI
	WHERE	ResultCycle = 1
	AND		TI.TestNbr = @TestNbrForStatCalc

	INSERT INTO @StatCalcInfo VALUES (@TestNbrForStatCalc, 4, 'Avg 1st Cycle Breakaway = ' + CONVERT(varchar(10), @AvgBrkWay))
	INSERT INTO @StatCalcInfo VALUES (@TestNbrForStatCalc, 5, '1st Cycle Breakaway k = ' + CONVERT(varchar(10), @KVal))
	INSERT INTO @StatCalcInfo VALUES (@TestNbrForStatCalc, 6, CASE WHEN @KVal <= 2.079 THEN '** UNACCEPTABLE K VALUE **' ELSE '(k must be greater than 2.079)' END)
	INSERT INTO @StatCalcInfo VALUES (@TestNbrForStatCalc, 7, CASE WHEN @KVal <= 2.079 THEN '(k must be greater than 2.079)' ELSE '' END)

	-- Since we have data, insert the additional text on lines 4, 5, 6 & 7
	-- Match on test number
	UPDATE	@TestResultsInfo
	SET		TestInfo = SCI.StatCalc
	FROM	@TestResultsInfo TI
	INNER JOIN @StatCalcInfo SCI ON TI.TestNbr = SCI.TestNbr
	WHERE TI.RowNbr = SCI.TestInfoRow
END

/*  END SPECIAL HANDLINE FOR STAT CALCS */

-- Final select back to the consumer
SELECT	CASE WHEN UOM = '-' AND RowNbr = 2 THEN '(' + TestInfo + ')' ELSE TestInfo END AS 'TestInfo',
		UOM,
		SampleNbr,
		ResultCycle,
		CASE WHEN UOM = '-' THEN '(' + CONVERT(Varchar(25), MinVal) + ')' ELSE CONVERT(varchar(25), MinVal) END AS 'MinVal',
		CASE WHEN UOM = '-' THEN '(' + CONVERT(Varchar(25), MaxVal) + ')' ELSE CONVERT(varchar(25), MaxVal) END AS 'MaxVal',
		CASE WHEN PassFailDisplayFlag = 'yes' THEN CASE WHEN PassFail = 'yes' THEN 'Pass' ELSE 'Fail' END ELSE '' END AS 'PassFail',
		CASE WHEN MinDisplayFlag = 'yes' AND UPPER(TestInfo) NOT LIKE '%HARDNESS%' THEN  CONVERT(varchar(25), MinReq) ELSE '' END AS 'MinReq',
		CASE WHEN MaxDisplayFlag = 'yes' AND UPPER(TestInfo) NOT LIKE '%HARDNESS%'  THEN  CONVERT(varchar(25), MaxReq) ELSE '' END AS 'MaxReq'
FROM	@TestResultsInfo
CONVERT(int, TestNbr), RowNbr

END

GRANT EXECUTE ON GetCTRFormTestResultsFields TO PUBLIC
