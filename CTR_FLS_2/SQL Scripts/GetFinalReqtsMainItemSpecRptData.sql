SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetFinalReqtsMainItemSpecRptData] (
	@Item	varchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Spec, ItemType
	FROM   ItemSpec ISP WITH (NOLOCK)
	WHERE  Item     = @Item
	AND    ItemType IN  ('PERFORMANCE', 'EMBRITTELMENT', 'VIBRATION', 'Tensile', 'Torque')

END

GRANT EXECUTE ON GetFinalReqtsMainItemSpecRptData TO PUBLIC
