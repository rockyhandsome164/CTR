//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CTR_FLS_2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class JobLot
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ComponentLot { get; set; }
        public string MaterialLot { get; set; }
        public string ParentLot { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> Operation { get; set; }
        public Nullable<int> QtyPer { get; set; }
        public Nullable<int> Sequence { get; set; }
    }
}
