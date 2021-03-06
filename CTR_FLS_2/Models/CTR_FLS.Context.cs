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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CTR_FLS_Entities : DbContext
    {
        public CTR_FLS_Entities()
            : base("name=CTR_FLS_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Cert> Certs { get; set; }
        public virtual DbSet<CTRUser> CTRUsers { get; set; }
        public virtual DbSet<Interchange> Interchanges { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemInfo> ItemInfoes { get; set; }
        public virtual DbSet<JobLotParent> JobLotParents { get; set; }
        public virtual DbSet<JobLot> JobLots { get; set; }
        public virtual DbSet<LotInfo> LotInfoes { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<OSP> OSPs { get; set; }
        public virtual DbSet<Sample> Samples { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<TestResult> TestResults { get; set; }
        public virtual DbSet<TestSpec> TestSpecs { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<ItemSpec> ItemSpecs { get; set; }
        public virtual DbSet<JobInfo> JobInfoes { get; set; }
        public virtual DbSet<MaterialTran> MaterialTrans { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<PackItem> PackItems { get; set; }
        public virtual DbSet<ShipLot> ShipLots { get; set; }
        public virtual DbSet<CustomerOrderItem> CustomerOrderItems { get; set; }
        public virtual DbSet<PackItemExtended> PackItemExtendeds { get; set; }
        public virtual DbSet<DefaultType> DefaultTypes { get; set; }
        public virtual DbSet<MasterSpec> MasterSpecs { get; set; }
        public virtual DbSet<NominalThread> NominalThreads { get; set; }
        public virtual DbSet<TestTemplate> TestTemplate { get; set; }
        public virtual DbSet<ProductFamily> ProductFamily { get; set; }
    
        public virtual ObjectResult<GetCTRFormHeaderFooterFields_Result> GetCTRFormHeaderFooterFields(string lotNbr)
        {
            var lotNbrParameter = lotNbr != null ?
                new ObjectParameter("LotNbr", lotNbr) :
                new ObjectParameter("LotNbr", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetCTRFormHeaderFooterFields_Result>("GetCTRFormHeaderFooterFields", lotNbrParameter);
        }
    }
}
