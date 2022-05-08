namespace CTR_FLS_2.Services
{
    public interface IQADServices
    {
        string GetPartDesc(string PartNbr);
        string GetVendorName(string VendorId);
        bool IsQADLotNbr(string LotNbr);
    }
}