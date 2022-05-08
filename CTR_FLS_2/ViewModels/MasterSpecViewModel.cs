using CTR_FLS_2.Models;
using System;

namespace CTR_FLS_2.ViewModels
{
    public class MasterSpecViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public string Master { get; set; }
        public int DefaultTypeId { get; set; }
        public Nullable<System.DateTime> DateEntered { get; set; }
        public string EnteredBy { get; set; }
        public  DefaultType DefaultType { get; set; }

    }

    public class MasterSpecSearch
    { 
        public string Specification { get; set; }
        public string Master { get; set; }
        public int? DefaultTypeId { get; set; } 
  
    }
}
