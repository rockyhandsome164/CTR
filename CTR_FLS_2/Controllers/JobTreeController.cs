using CTR_FLS_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CTR_FLS_2.Controllers
{
    public class JobTreeController : BaseController
    {
        private readonly IJobTreeServices JobTreeServ;

        public JobTreeController() : this (new JobTreeServices()) { }
        public JobTreeController(IJobTreeServices JobTreeServicesObj)
        {
            JobTreeServ = JobTreeServicesObj;
        }
        [HttpGet]
        [Route("JobTree/View")]
        public ActionResult DisplayJobTree()
        {
            return View("JobTree");
        }

        [HttpPost]
        [Route("JobTree/Show")]
        public JsonResult GetJobTree(string ComponentNbr)
        {
            return Json(JobTreeServ.GetJobTreeVM(ComponentNbr)) ;
        }
    }
}
