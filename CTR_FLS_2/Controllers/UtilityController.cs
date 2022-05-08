using CTR_FLS_2.Models;
using CTR_FLS_2.Services;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace CTR_FLS_2.Controllers
{
    public class UtilityController : BaseController
    {
        private readonly IUtilityServices utilityServices;
        public ActionResult Index()
        {
            return View();
        }

        public UtilityController() : this(new UtilityServices()) { }
        public UtilityController(IUtilityServices _utilityServices)
        {
            utilityServices = _utilityServices;
        }


        #region MasterSpec
        [HttpGet]
        public ActionResult MasterSpec()
        {
            return View();
        }

        [HttpGet]
        [Route("utilities/gettypes")]
        public JsonResult GetTypes()
        {
            try
            {
                JsonResult data = Json(utilityServices.GetTypes(), JsonRequestBehavior.AllowGet);
                return data;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to fetch the Record", e);
            }
        }

        [HttpGet]
        [Route("utilities/search")]
        public JsonResult Search(MasterSpecSearch masterSpec)
        {            
            try
            {
                JsonResult result = Json(utilityServices.GetMasterSpecs(masterSpec), JsonRequestBehavior.AllowGet);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to find the Record", e);
            }
        }

        [HttpPost]
        [Route("utilities/savemasterspec")]
        public ActionResult Savemasterpec(MasterSpec masterSpec)
        {
           
            try
            {
                if (masterSpec == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ModelState.IsValid)
                {
                    MasterSpec masterSpec1 = new MasterSpec
                    {
                        Id=masterSpec.Id ,
                        Specification = masterSpec.Specification,
                        Master = masterSpec.Master,
                        Description = masterSpec.Description,
                        DefaultTypeId = masterSpec.DefaultTypeId
                    };
                    utilityServices.SaveMasterSpecs(masterSpec1);
                }
                ModelState.Clear();
                var masterSpecId = masterSpec.Id;
                return Json(new {
                    message = $"Master Spec {((masterSpecId>0) ? "updated" : "added")} successfully."
                });
            }
            catch (Exception e)
            {

                throw new Exception("Unable to Save the Record", e);
            }
        }

        [HttpPost]
        [Route("utilities/delete")]
        public ActionResult Delete(int Id)
        {
            try
            {
                if (!(Id > 0))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    utilityServices.DeleteMasterSpec(Id);
                }

                return Json(new
                {
                    message = "Selected Master has been deleted Successfully."
                });
            }
            catch (Exception e)
            {
                throw new Exception("Unable to Delete the Record", e);
            }

        }

        #endregion

    }
}
