using CTR_FLS_2.Models;
using CTR_FLS_2.Services;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace CTR_FLS_2.Controllers
{
    public class NominalThreadController : BaseController
    {
        // GET: NominalThread
        private readonly INominalThreadServices nominalThreadServices;
        public ActionResult NominalThread()
        {
            return View();
        }
        public NominalThreadController() : this(new NominalThreadServices()) { }
        public NominalThreadController(INominalThreadServices _nominalThreadServices)
        {
            nominalThreadServices = _nominalThreadServices;
        }

        [HttpGet]
        [Route("nominalthread/getallnominalthreads")]
        public JsonResult GetAllNominalThreads(string searchParams)
        {
            try
            {
                JsonResult data = Json(nominalThreadServices.GetAllNominalThread(searchParams), JsonRequestBehavior.AllowGet);
                return data;

            }
            catch (Exception e)
            {
                throw new Exception("Unable to fetch the Record", e);
            }
        }

        [HttpGet]
        [Route("nominalthread/getById")]
        public JsonResult GetNominalThreadById(int Id)
        {
            try
            {
                JsonResult data = Json(nominalThreadServices.GetNominalThreadById(Id), JsonRequestBehavior.AllowGet);
                return data;

            }
            catch (Exception e)
            {
                throw new Exception("Unable to fetch the Record", e);
            }
        }

        [HttpPost]
        [Route("nominalthread/savenominalthread")]
        public ActionResult Savemasterpec(NominalThreadViewModel nominalThreadViewModel)
        {
            object result = null;

            try
            {
                if (nominalThreadViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ModelState.IsValid)
                {
                    NominalThread nominalThread = new NominalThread
                    {
                        Id = nominalThreadViewModel.Id,
                        DashNumber = nominalThreadViewModel.DashNumber,
                        NominalThreadSize = nominalThreadViewModel.NominalThreadSize
                    };
                    result = nominalThreadServices.SaveNominalThread(nominalThread);

                }
                ModelState.Clear();

                JsonResult data = Json(result, JsonRequestBehavior.AllowGet);
                return data;
            }
            catch (Exception e)
            {

                return Json(new { status = "Error", message = "Unable to Save the Record" });
            }
        }

        [HttpPost]
        [Route("nominalthread/delete")]
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
                    nominalThreadServices.DeleteNominalThread(Id);
                }

                return Json(new
                {
                    message = "NominalThread has been deleted Successfully."
                });
            }
            catch (Exception e)
            {
                throw new Exception("Unable to Delete the Record", e);
            }

        }


    }
}
