﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QADEntities : DbContext
    {
        public QADEntities()
            : base("name=QADEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<abs_mstr> abs_mstr { get; set; }
        public virtual DbSet<prh_hist> prh_hist { get; set; }
        public virtual DbSet<pt_mstr> pt_mstr { get; set; }
        public virtual DbSet<sod_det> sod_det { get; set; }
        public virtual DbSet<tr_hist> tr_hist { get; set; }
        public virtual DbSet<wo_mstr> wo_mstr { get; set; }
        public virtual DbSet<vd_mstr> vd_mstr { get; set; }
    }
}