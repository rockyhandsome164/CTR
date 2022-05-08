using CTR_FLS_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.Services
{
    public class QADServices : IQADServices
    {
        private readonly ILoggingServices _logger;

        public QADServices() : this (new LoggingServices()) { }

        public QADServices(ILoggingServices LoggingServicesObj)
        {
            _logger = LoggingServicesObj;
        }

        /// <summary>
        /// Checks to see if the lot nbr passed in should query legacy table (in the CTR database) or
        /// from QAD
        /// </summary>
        /// <param name="LotNbr"></param>
        /// <returns></returns>
        public bool IsQADLotNbr(string LotNbr)
        {
            return LotNbr.IndexOf('-') == -1 ? true : false;
        }

        public string GetPartDesc(string PartNbr)
        {
            string PartDesc = "";

            try
            {
                using (QADEntities QAD = new QADEntities())
                {
                    pt_mstr PartMasterRec = QAD.pt_mstr.Where(w => w.pt_part == PartNbr).FirstOrDefault();
                    if (PartMasterRec != null)
                    {
                        PartDesc = (String.IsNullOrEmpty(PartMasterRec.pt_desc1) ? "" : PartMasterRec.pt_desc1) + " " +
                                   (String.IsNullOrEmpty(PartMasterRec.pt_desc2) ? "" : PartMasterRec.pt_desc2);
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }

            return PartDesc;
        }

        public string GetVendorName(string VendorId)
        {
            string VendorName = "";

            try
            {
                using (QADEntities QAD = new QADEntities())
                {
                    vd_mstr VendorRec = QAD.vd_mstr.Where(w => w.vd_addr == VendorId).FirstOrDefault();
                    if (VendorRec != null)
                    {
                        VendorName = String.IsNullOrEmpty(VendorRec.vd_sort) ? "" : VendorRec.vd_sort;
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex);
            }
            return VendorName;

        }
    }
}