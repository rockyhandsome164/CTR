using CTR_FLS_2.Models;
using System.Collections.Generic;

namespace CTR_FLS_2.Services
{
    public interface ICommonServices
    {
        JobLot GetComponentItem(int ComponentItemId);
        int GetComponentItemId(string ComponentToSearch);
        Material GetMaterial(string ComponentLotNbr);
        List<int> GetItemChildren(int ComponentItemId);
        Cert GetCertForLot(string LotNbr);
        LotInfo GetLotInfo(string ItemNbr);
        Vendor GetVendorInfo(string VendorId);
        Item GetItem(string ItemCode);
        JobInfo GetJobInfoForLot(string LotNbr);
        List<OSP> GetOSP(string ComponentLotNbr);
    }
}