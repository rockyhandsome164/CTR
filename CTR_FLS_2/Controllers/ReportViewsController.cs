using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CTR_FLS_2.Models;
using System.Configuration;
using CTR_FLS_2.Services;
using CTR_FLS_2.Controllers;
using System.Web.Mvc;
using System.IO;
using System.Web;

namespace CTR_FLS.Controllers
{
    public class ReportViewsController : BaseController
    {
        CTR_FLS_Entities db = new CTR_FLS_Entities();
        private readonly IReportServices ReportServ;
        private readonly ITestTemplateService TestTemplateService;

        public ReportViewsController() : this(new ReportServices(), new TestTemplateService()) { }
        public ReportViewsController(IReportServices ReportServicesObj, ITestTemplateService testTemplateService)
        {
            ReportServ = ReportServicesObj;
            TestTemplateService = testTemplateService;
        }

        #region ReportContainerViews
        [HttpGet]
        public ActionResult CertifiedTestReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ItemSpecReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AuthRelCert()
        {
            return View("AuthorizedReleaseCert");
        }

        [HttpGet]
        public ActionResult QAJobLogReport()
        {
            return View();
        }

        #endregion

        #region ReportRendering
        [HttpGet]
        [Route("reportviews/getctr")]
        public ActionResult GetCertifiedTestReport(string LotNbr, string FileName)
        {
            TempData["RptType"] = "CertifiedTestReport";
            TempData["RptFileName"] = Path.Combine(HttpContext.Server.MapPath("~/Reports/ReportObjectsInTRDXFormat/"), FileName);
            TempData["LotNbr"] = LotNbr;

            return View("ReportViewer");
        }

        [HttpGet]
        [Route("reportviews/GetQAJobLog")]
        public ActionResult GetQAJobLogReport(string Item, string Job)
        {
            TempData["Item"] = Item;
            TempData["Job"] = Job;

            TempData["RptType"] = String.IsNullOrWhiteSpace(Job) ? "QAJobLogReport" : "QAJobCardContainer";

            return View("ReportViewer");
        }

        [HttpGet]
        [Route("reportviews/GetQAJobLogLockTorq")]
        public ActionResult GetQAJobLogLockTorq(string Job)
        {
            TempData["Job"] = Job;

            TempData["RptType"] = "QAJobCardLockingTorqueRpt";

            return View("ReportViewer");
        }

        [HttpGet]
        [Route("reportviews/getctrfilename")]
        public JsonResult GetCTRReportFileName(string LotNbr)
        {
            // Make a call to create the custom CTR TRDX file that is needed to render
            // Done this way because we need to dynamically add component sub reports
            // to the CTR report. We'll pass in the lot number and have it create the 
            // XML blocks of the subreport and inject those into the CTR TRDX template
            // and create a new unique report name that will get returned. Then
            // we'll assign that in the report source.
            return Json(ReportServ.GetCTRReportFileName(LotNbr), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("reportviews/getauthcert")]
        public ActionResult GetAuthorizationCertReport(string ShipNbr, string FormType, string AuthName, string AuthDate, string Rem1, string Rem2, string Rem3)
        {
            TempData["RptType"] = "AuthorizeCert";
            TempData["FormType"] = FormType;
            TempData["AuthName"] = AuthName;
            TempData["AuthDate"] = AuthDate;
            TempData["Remark1"] = HttpUtility.UrlDecode(Rem1);
            TempData["Remark2"] = HttpUtility.UrlDecode(Rem2);
            TempData["Remark3"] = HttpUtility.UrlDecode(Rem3);
            TempData["ShipperNbr"] = ShipNbr;

            return View("ReportViewer");
        }

        [HttpGet]
        [Route("reportviews/getitemspec")]
        public ActionResult GetItemSpecReport(string StartItem, string EndItem, string StatusList)
        {
            TempData["RptType"] = "ItemSpec";
            TempData["ItemStart"] = StartItem;
            TempData["ItemEnd"] = EndItem;
            TempData["ListOfStatuses"] = StatusList;

            return View("ReportViewer");
        }

        #endregion

        #region RptHelperMethods
        [HttpGet]
        [Route("reportviews/getlocktorqreq")]
        public JsonResult GetLockingTorqueRptNeeded(string Job)
        {
            return Json(ReportServ.DetermineLockingTorqueRptNeeded(Job), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("reportviews/validateitemjob")]
        public JsonResult ValidateItemAndJobRelationship(string Job, string Item)
        {
            return Json(ReportServ.ValidateJobItemRelationship(Job, Item), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Test Template
        public ActionResult TestTemplateDetails()
        {
            return RedirectToAction("TestTemplates", "ReportViews", new { id = 0 });

        }

        [HttpGet]
        public ActionResult TestTemplates(int id)
        {
            if (id > 0)
            {
                List<TestTemplateType> testTemplateTypes = TestTemplateService.GetTestTemplateTypes();
                List<TestTemplate> testTemplates = TestTemplateService.GetTestTemplateDetails();
                TestTemplate testTemplate = testTemplates.Where(x => x.Id == id).FirstOrDefault();
                string list = string.Join(",", testTemplateTypes.Select(x => "'" + x.Name + "'"));
                ViewBag.Name = new SelectList(testTemplateTypes, "Template", "Name");
                return View(testTemplate);
            }
            else
            {
                List<TestTemplateType> testTemplateTypes = TestTemplateService.GetTestTemplateTypes();
                List<TestTemplate> testTemplates = TestTemplateService.GetTestTemplateDetails();
                TestTemplate testTemplate = testTemplates.FirstOrDefault();
                string list = string.Join(",", testTemplateTypes.Select(x => "'" + x.Name + "'"));
                ViewBag.Name = new SelectList(testTemplateTypes, "Template", "Name");
                return View(testTemplate);
            }

        }

        [HttpPost]
        public ActionResult TestTemplates(TestTemplate testTemplate,string cpassfail,string PrintMinMax,string MinValueReq,string MaxValueReq,string submit)
        {
            if (submit == "Save")
            {
                var templateData = db.TestTemplate.Where(x => x.Id == testTemplate.Id).FirstOrDefault();
                if(templateData != null)
                {
                    templateData.Requirements = testTemplate.Requirements;
                    db.Entry(templateData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                string pass = "";
                string MinMax = "";
                string MinVal = "";
                string MaxVal = "";
                if (cpassfail == "on")
                { pass = "yes"; }
                else { pass = "no"; }
                if (PrintMinMax == "on")
                { MinMax = "yes"; }
                else { MinMax = "no"; }
                if (MinVal == "on")
                { MinVal = "yes"; }
                else { MinVal = "no"; }
                if (MaxVal == "on")
                { MaxVal = "yes"; }
                else { MaxVal = "no"; }
                testTemplate.PassFail = pass;
                testTemplate.PrintMinMax = MinMax;
                testTemplate.MinValueRequired = MinVal;
                testTemplate.MaxValueRequired = MaxVal;
                db.TestTemplate.Add(testTemplate);

                db.SaveChanges();
            }

            List<TestTemplateType> testTemplateTypes = TestTemplateService.GetTestTemplateTypes();
            List<TestTemplate> testTemplates = TestTemplateService.GetTestTemplateDetails();
            TestTemplate testTemp = testTemplates.FirstOrDefault();
            string list = string.Join(",", testTemplateTypes.Select(x => "'" + x.Name + "'"));
            ViewBag.Name = new SelectList(testTemplateTypes, "Template", "Name");
            return View(testTemp);

        }

        [HttpGet]
        public ActionResult GetTestTemplateDetails()
        {
            List<TestTemplate> testTemplates = TestTemplateService.GetTestTemplateDetails();
            return View("GetTestTemplateDetails", testTemplates);
        }
        
        
        public ActionResult DeleteTestTemplate(int id)
        {
            TestTemplateService.DeleteTestTemplate(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
