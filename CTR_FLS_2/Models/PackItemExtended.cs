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
    
    public partial class PackItemExtended
    {
        public int Id { get; set; }
        public Nullable<int> OrderLine { get; set; }
        public string OrderNbr { get; set; }
        public Nullable<int> Release { get; set; }
        public string InvoicedFlag { get; set; }
        public string Location { get; set; }
        public string Lot { get; set; }
        public Nullable<int> PackingSlip { get; set; }
        public Nullable<int> QtyInvoiced { get; set; }
        public Nullable<int> QtyPacked { get; set; }
        public Nullable<int> QtyShipped { get; set; }
        public string ShippedFlag { get; set; }
    }
}
