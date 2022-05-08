SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		John Malvey
-- Description:	Pulls data for Locking Torque QA Job Card
--
-- Change log:
--	
--		WHEN			WHO			WHAT
--		----			---			----
--
--		2022-04-01		JAM			Initial
--
-- =============================================

CREATE PROCEDURE [dbo].[GetQAJobCardLockingTorqueData] (
	@LotNbr		varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @JobPortion varchar(25) = LEFT(@LotNbr, CHARINDEX('-', @LotNbr) - 1)
	DECLARE @Suffix varchar(3) = RIGHT(@LotNbr, 1)

	SELECT	T.Template, I.Description, T.PerformanceSpec, I.CharFld3, I.NominalThreadSize, T.TestBolt, II.Description, T.MinBrkway, T.UnitOfMeasure, T.MaxInstall, T.SeatLoad
	FROM    JobInfo J WITH (NOLOCK)
	INNER JOIN	Test T  WITH (NOLOCK) ON J.Item = I.Item
	INNER JOIN	Item I  WITH (NOLOCK) ON J.Item = I.Item
	INNER JOIN	ItemInfo II  WITH (NOLOCK) ON J.Item = II.item
	WHERE J.Job = @JobPortion
	AND J.Suffix = 	@Suffix	
	AND T.Template IN (9, 10, 11)
	ORDER BY T.Template

END

GRANT EXECUTE ON GetQAJobCardLockingTorqueData TO PUBLIC
