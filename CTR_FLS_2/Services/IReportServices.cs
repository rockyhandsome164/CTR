namespace CTR_FLS_2.Services
{
    public interface IReportServices
    {
        string GetCTRReportFileName(string LotNbr);
        string DetermineLockingTorqueRptNeeded(string LotNbr);
        string ValidateJobItemRelationship(string LotNbr, string Item);
    }
}