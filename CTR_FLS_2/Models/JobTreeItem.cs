using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTR_FLS_2.Models
{
    public class JobTreeItem
    {
        public string ItemCode { get; set; }
        public string ComponentLot { get; set; }
        public string MaterialLot { get; set; }
        public int ItemLevel { get; set; }
        public string MaterialSpec { get; set; }
        public string VendorMillNbr { get; set; }
        public string VendorNbr { get; set; }
        public string OSPAttachType { get; set; }
    }
}
