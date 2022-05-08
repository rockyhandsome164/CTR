using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.Models
{
    public class OnBaseDocMeta
    {
        public long DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentDisplayName { get; set; }
        public string DocumentType { get; set; }
        public DateTime DocumentDate { get; set;}
        public string DocumentImg { get; set; }

    }
}