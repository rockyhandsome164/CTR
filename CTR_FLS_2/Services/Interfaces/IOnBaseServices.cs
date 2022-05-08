using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System.Collections.Generic;

namespace CTR_FLS_2.Services
{
    public interface IOnBaseServices
    {
        string CTRPDFExport(string FileName, string LotNbr);
        OnBaseViewModel GetOnBaseDocInfo(string DocType, List<OnBaseQueryCriteria> QueryCriteria, string DocOutputFileName, string DocOutputType);
        OnBaseViewModel SaveCTRDocToOnBase(string FileName, string DocType, string LotNbr);
        OnBaseViewModel SaveCertPackageToOnBase(string LotNbr, string ShipperNbr);
    }
}