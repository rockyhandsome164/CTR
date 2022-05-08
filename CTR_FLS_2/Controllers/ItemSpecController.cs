using CTR_FLS_2.Models;
using CTR_FLS_2.Services;
using CTR_FLS_2.ViewModels;
using System;
using System.Net;
using System.Web.Mvc;

namespace CTR_FLS_2.Controllers
{
    public class ItemSpecController : BaseController
    {
        private readonly ItemSpecServices itemSpecServices;

        // GET: ItemSpec
        public ActionResult ItemSpec()
        {
            return View();
        }

        public ItemSpecController() : this(new ItemSpecServices()) { }
        public ItemSpecController(ItemSpecServices _itemSpecServices)
        {
            itemSpecServices = _itemSpecServices;
        }

        [HttpGet]
        [Route("itemspec/items")]
        public JsonResult GetItemsDetail(string searchParams)
        {
            try
            {
                JsonResult data = Json(itemSpecServices.GetAllItems(searchParams), JsonRequestBehavior.AllowGet);
                return data;

            }
            catch (Exception e)
            {
                throw new Exception("Unable to fetch the Record", e);
            }
        }


        [HttpGet]
        [Route("itemspec/getitemspecsbyitemnumber")]
        public JsonResult GetItemSpecsByItemNumber(string itemNumber)
        {
            try
            {
                JsonResult result = Json(itemSpecServices.GetItemSpecsByItemNumber(itemNumber), JsonRequestBehavior.AllowGet);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to find the Record", e);
            }
        }

        [HttpGet]
        [Route("itemspec/getnotebyitemnumber")]
        public JsonResult GetNoteByItemNumber(string itemNumber)
        {
            try
            {
                JsonResult data = Json(itemSpecServices.GetNoteByItemNumber(itemNumber), JsonRequestBehavior.AllowGet);
                return data;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("itemspec/savenote")]
        public ActionResult SaveNote(NoteViewModel note)
        {
            JsonResult data = null;

            try
            {
                if (note == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ModelState.IsValid)
                {

                    data = Json(itemSpecServices.SaveItemNote(note), JsonRequestBehavior.AllowGet);

                }
                ModelState.Clear();

                return data;
            }
            catch (Exception e)
            {

                return Json(new { status = "Error", message = "Unable to Save the Record" });
            }
        }

        [HttpPost]
        [Route("itemspec/saveitems")]
        public ActionResult Savemasterpec(ItemViewModel itemViewModel)
        {
            object result = null;

            try
            {
                if (itemViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ModelState.IsValid)
                {
                    Item item = new Item
                    {
                        Id = itemViewModel.Id,
                        Item1 = itemViewModel.Item1,
                        Description = itemViewModel.Description,
                        Type = itemViewModel.Type,
                        NominalThreadSize = itemViewModel.NominalThreadSize,
                        ProductFamily = itemViewModel.ProductFamily,
                        Status = itemViewModel.Status,
                        MaterialStatus = itemViewModel.MaterialStatus
                    };
                    result = itemSpecServices.SaveItems(item);

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
        [Route("itemspec/saveitemspec")]
        public ActionResult SaveItemSpec(ItemSpecsViewModel itemSpec)
        {
            ItemSpec result = null;

            try
            {
                if (itemSpec == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ModelState.IsValid)
                {

                    itemSpecServices.AddMasterSpecToItem(itemSpec);

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


        [HttpGet]
        [Route("itemspec/getproductfamily")]
        public JsonResult GetAllProductFamily(string searchParams)
        {
            try
            {
                JsonResult data = Json(itemSpecServices.GetAllProductFamily(searchParams), JsonRequestBehavior.AllowGet);
                return data;

            }
            catch (Exception e)
            {
                throw new Exception("Unable to fetch the Record", e);
            }
        }

        [HttpGet]
        [Route("itemspec/testtemplate")]
        public JsonResult GetTestDetail(string searchParams)
        {
            try
            {
                JsonResult data = Json(itemSpecServices.GetTestDetail(searchParams), JsonRequestBehavior.AllowGet);
                return data;

            }
            catch (Exception e)
            {
                throw new Exception("Unable to fetch the Record", e);
            }
        }

        [HttpPost]
        [Route("itemspec/savetests")]
        public ActionResult SaveTests(Test test)
        {
            Test result = null;

            try
            {
                if (test == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ModelState.IsValid)
                {

                    itemSpecServices.SaveTest(test);

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


    }
}