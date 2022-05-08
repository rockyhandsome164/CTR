using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Item1 { get; set; }
        public string Description { get; set; }
        public string NominalThreadSize { get; set; }
        public string MaterialStatus { get; set; }
        public string Type { get; set; }
        public string ProductFamily { get; set; }
        public string Status { get; set; }

    }
}