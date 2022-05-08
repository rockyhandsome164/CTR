using CTR_FLS_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTR_FLS_2.ViewModels
{
    public class JobTreeViewModel
    {
        public string TopLevelComponentItemCode { get; set; }
        public string TopLevelComponentItemDesc { get; set; }
        public string TopLevelComponentOSP { get; set; }
        public List<JobTreeItem> JobTreeItems { get; set; }
        public List<JobTreeMaterialViewModel> MaterialList { get; set; }

        public int? CertNumber { get; set; }
        public DateTime? CertDate { get; set; }
        public string Message { get; set; }
    }
}
