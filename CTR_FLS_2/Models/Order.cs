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
    
    public partial class Order
    {
        public int Id { get; set; }
        public string OrderNbr { get; set; }
        public string Customer { get; set; }
        public string PO { get; set; }
        public Nullable<int> ShipTo { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
        public Nullable<int> NbrOfPackages { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
        public string Contact { get; set; }
    }
}
