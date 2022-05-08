using CTR_FLS_2.Models;
using CTR_FLS_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CTR_FLS_2.Controllers
{
    public class OnBaseController : BaseController
    {
        private readonly IOnBaseServices OnBaseServ;

        public OnBaseController() : this(new OnBaseServices()) { }

        public OnBaseController(IOnBaseServices OnBaseSerivceObj)
        {
            OnBaseServ = OnBaseSerivceObj;
        }
        [HttpGet]
        [Route("onbase/view")]
        public ActionResult OnBasePortal()
        {
            return View("OnBase");
        }

        [HttpGet]
        [Route("onbase/certpkg")]
        public ActionResult CreateCertPackage()
        {
            return View("CertPackage");
        }



        [HttpGet]
        [Route("onbase/query")]
        public JsonResult GetDocumentFromOnBase(string DocType, string LotNbr)
        {
            List<OnBaseQueryCriteria> QryCriteria = new List<OnBaseQueryCriteria>();

            // For testing
            string KWT = DocType.Equals(Constants.ONBASE_DOC_TYPE_MATERIAL_CERT) ? "Material Lot Number" : "Lot Number";
            QryCriteria.Add(new OnBaseQueryCriteria { KeywordType = KWT, KeywordOperator = "=", KeywordValue = LotNbr });

            // Create the JsonResult separately so we can set the MaxJsonLength.
            // The VM will have streams of the PDFs so need to increase the default length
            // Note this not work if you set this in the web.config. Could be due to this
            // being an Ajax call
            var JsonData = Json(OnBaseServ.GetOnBaseDocInfo(DocType, QryCriteria, "", Constants.ONBASE_DOC_OUTPUT_TYPE_FILE));
            JsonData.MaxJsonLength = Int32.MaxValue;
            JsonData.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return JsonData;
        }

        [HttpGet]
        [Route("onbase/ctrpdfexport")]
        public JsonResult ExportReportToPDF(string FileName, string LotNbr)
        {
            return Json(OnBaseServ.CTRPDFExport(FileName, LotNbr), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("onbase/savedoctoonbase")]
        public JsonResult SaveCTRDocToOnBase(string FileName, string DocType, string LotNbr)
        {

            var OnBaseDocDataInJSONFormat = Json(OnBaseServ.SaveCTRDocToOnBase(FileName, DocType, LotNbr));
            OnBaseDocDataInJSONFormat.MaxJsonLength = int.MaxValue;
            OnBaseDocDataInJSONFormat.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return Json(OnBaseDocDataInJSONFormat, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("onbase/savepkgtoonbase")]
        public JsonResult SaveCertPackageToOnBase(string LotNbr, string ShipperNbr)
        {
            var OnBasePkgDataInJSONFormat = Json(OnBaseServ.SaveCertPackageToOnBase(LotNbr, ShipperNbr));
            OnBasePkgDataInJSONFormat.MaxJsonLength = int.MaxValue;
            OnBasePkgDataInJSONFormat.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return Json(OnBasePkgDataInJSONFormat, JsonRequestBehavior.AllowGet);


        }

    }
}
