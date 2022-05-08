using CTR_FLS_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTR_FLS_2.Services
{
    public class CommonServices : ICommonServices
    {
        private readonly ILoggingServices _logger;

        public CommonServices() : this (new LoggingServices())  {  }
        public CommonServices(ILoggingServices loggingServices)
        {
            _logger = loggingServices;
        }

        public int GetComponentItemId(string ComponentToSearch)
        {
            int ItemId = -1;

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    ItemId = DBContext.JobLots.Where(w => w.ComponentLot == ComponentToSearch).Select(s => s.Id).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return ItemId;
        }

        public JobLot GetComponentItem(int ComponentItemId)
        {
            JobLot SelectedItem = new JobLot();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    SelectedItem = DBContext.JobLots.Where(w => w.Id == ComponentItemId).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return SelectedItem;
        }

        public Material GetMaterial(string ComponentLotNbr)
        {
            Material MatRec = new Material();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    MatRec = DBContext.Materials.Where(w => w.ControlNbr == ComponentLotNbr).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return MatRec;
        }

        public List<OSP> GetOSP(string ComponentLotNbr)
        {
            List<OSP> OspRecs = new List<OSP>();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    OspRecs = DBContext.OSPs.Where(w => w.LotNbr == ComponentLotNbr).ToList();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return OspRecs;
        }

        public LotInfo GetLotInfo(string LotNbr)
        {
            LotInfo LotInfoRec = new LotInfo();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    LotInfoRec = DBContext.LotInfoes.Where(w => w.Lot == LotNbr).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return LotInfoRec;

        }

        public Item GetItem(string ItemCode)
        {
            Item ItemRec = new Item();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    ItemRec = DBContext.Items.Where(w => w.Item1 == ItemCode).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return ItemRec;

        }

        public Vendor GetVendorInfo(string VendorId)
        {
            Vendor VendorRec = new Vendor();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    VendorId = VendorId.Replace(" ", String.Empty);
                    VendorRec = DBContext.Vendors.Where(w => w.VendorId == VendorId).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return VendorRec;


        }

        public List<int> GetItemChildren(int ComponentItemId)
        {
            List<int> ChildIds = new List<int>();

            using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
            {
                ChildIds = DBContext.JobLotParents.Where(w => w.ParentJobLotId == ComponentItemId).Select(s => s.JobLotId).ToList();
            }

            return ChildIds;

        }

        public Cert GetCertForLot(string LotNbr)
        {
            Cert Cert = new Cert();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    Cert = DBContext.Certs.Where(w => w.LotNbr == LotNbr).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return Cert;
        }

        public JobInfo GetJobInfoForLot(string LotNbr)
        {
            JobInfo JobInfoRec = new JobInfo();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    string JobNbrPortion = LotNbr.Substring(0, LotNbr.IndexOf("-"));
                    int JobSuffix = Int32.Parse(LotNbr.Substring(LotNbr.IndexOf("-")));
                    JobInfoRec = DBContext.JobInfoes.Where(w => w.Job == JobNbrPortion && w.Suffix == JobSuffix).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return JobInfoRec;
        }
    }
}
