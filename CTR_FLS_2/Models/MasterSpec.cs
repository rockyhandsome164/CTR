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
    
    public partial class MasterSpec
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public string Master { get; set; }
        public int DefaultTypeId { get; set; }
        public Nullable<System.DateTime> DateEntered { get; set; }
        public string EnteredBy { get; set; }
    
        public virtual DefaultType DefaultType { get; set; }
    }
}
