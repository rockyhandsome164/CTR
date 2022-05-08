using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTR_FLS_2.ViewModels
{
    public class JobTreeMaterialViewModel
    {
        public int Id { get; set; }
        public string ControlNbr { get; set; }
        public string ItemCode { get; set; }
        public string VendorNbr { get; set; }
        public string MillNbr { get; set; }
        public string MaterialSpec { get; set; }
        public string Stage { get; set; }
        public string VendorName { get; set; }
        public string Description { get; set; }
        public string Ponbr { get; set; }
        public DateTime? RecvdDate { get; set; }
        public int? POLineNbr { get; set; }
        public int? POReleaseNbr { get; set; }
    }
}
