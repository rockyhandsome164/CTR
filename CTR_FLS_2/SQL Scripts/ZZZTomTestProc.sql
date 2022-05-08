
use CTR_FLS_Dev
go
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE dbo.ZZZTomTestProc
AS
BEGIN
	SET NOCOUNT ON;

	select Id, item, [Description], ThreadSize, ProductFamily from ItemInfo where item = 'DAT595'
END

GRANT EXECUTE ON ZZZTomTestProc TO PUBLIC