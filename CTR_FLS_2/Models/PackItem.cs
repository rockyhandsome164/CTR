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
    
    public partial class PackItem
    {
        public int Id { get; set; }
        public int PackingSlip { get; set; }
        public string OrderNbr { get; set; }
        public Nullable<int> OrderLine { get; set; }
        public Nullable<int> Release { get; set; }
        public string Item { get; set; }
        public Nullable<int> QtyOrdered { get; set; }
        public Nullable<int> QtyPacked { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}
