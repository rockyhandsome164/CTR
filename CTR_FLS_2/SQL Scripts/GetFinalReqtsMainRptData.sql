USE [CTR_FLS_Test]
GO
/****** Object:  StoredProcedure [dbo].[GetFinalReqtsMainRptData]    Script Date: 4/9/2022 11:15:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetFinalReqtsMainRptData] (
	@Item	varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT II.Item, II.ProductFamily, (SELECT TOP 1 Occurrence FROM JobInfo WHERE Item = @Item ORDER BY CONVERT(int, Occurrence)  DESC) AS 'Occurrence'
	FROM   ItemInfo II WITH (NOLOCK)
	WHERE  II.Item     = @Item

END