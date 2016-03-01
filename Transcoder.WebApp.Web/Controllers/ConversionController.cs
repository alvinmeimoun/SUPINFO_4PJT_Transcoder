using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.FFmpegWrapper;
using Core.Transcoder.Service;
using Core.Transcoder.Service.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Transcoder.WebApp.Web.Controllers
{
    public class ConversionController : Controller
    {
        // GET: Conversion
        public ActionResult Index()
        {
            return RedirectToAction("Panier");
        }


        public ActionResult Panier()
        {
            int UserId = CookieUtil.GetUserId(this);
            if (UserId == 0)
                return RedirectToAction("Index", "Home");

            var panier = new TASK_Service().GetPanierViewModel(UserId);

            return View("Panier", panier);
        }

        public ActionResult AddConversion()
        {
            int UserId = CookieUtil.GetUserId(this);
            if (UserId == 0)
                return RedirectToAction("Index", "Home");
            CreateTaskViewModel model = new TASK_Service().InitCreateTaskViewModel(UserId);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddConversion(CreateTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isEdited =  new TASK_Service().AddTaskByViewModel(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteConversion(int id)
        {
            if(id != 0)
            {
               bool isDeleted = new TASK_Service().DeleteTaskById(id);
                if (isDeleted)
                    return RedirectToAction("Index");
            }
            return null;
        }

        [HttpPost]
        public ActionResult ValiderPanier(PanierViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // Prevoir l'intervention de paypal

            // on set a true le is Paid
            //foreach(var task in model.ListOfConversions)
            //{
            //    task.IS_PAID = true;
            //}
            //bool isEdited = new TASK_Service().AddTaskByViewModel(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                string path = SaveFileInFolder(file);
                // On recupere le format de base de la video
                int indexOf = file.ContentType.LastIndexOf('/') + 1;
                string format = file.ContentType.Substring(indexOf);
                var formatBase = new FORMAT_Service().GetFormatByName(format);
                // On recupere les infos de la video
                VideoFile videoFile = new VideoFile(path);
                double VideoDuration = videoFile.Duration.TotalMinutes;
                // On génére le prix en fonction de la durée
                double price = PriceGeneratorUtil.GetPriceByDuration(VideoDuration);

                // Si le format a été trouvé dans nos bases on va proposer les conversions liées possibles.
                if (formatBase != null)
                {
                    var listFormatTypes = new FORMAT_TYPE_Service().GetSelectListFormatTypeByFormat((int)formatBase.FK_ID_FORMAT_TYPE);

                    return Json(new { success = "true", fileUrl = path, fileLength = file.ContentLength, fileName = file.FileName, fileFormatBase = formatBase.PK_ID_FORMAT,
                        listFormatType = new SelectList(listFormatTypes, "Value", "Text"), fileDuration =  VideoDuration.ToString(), filePrice = price.ToString()
                    });
                }
                else
                {
                    return Json(new { success = "false" });
                }
            }
            return Json(new { success = "false" });
        }

        [HttpPost]
        public JsonResult GetFormatByFormatTypeAndFormatBase(string formatTypeId, string fileFormatBase)
        {
            int FormatTypeId = 0;
            int FileFormat = 0;
            int.TryParse(formatTypeId, out FormatTypeId);
            int.TryParse(fileFormatBase, out FileFormat);
            List<SelectListItem> slFormat = new FORMAT_Service().GetSelectListFormatByFormatTypeIdAndFormatBase(FormatTypeId, FileFormat);
            return Json(new { listFormats = new SelectList(slFormat, "Value", "Text") }, JsonRequestBehavior.AllowGet);

        }


        private string SaveFileInFolder(HttpPostedFileBase file)
        {
            string path = "";
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                path = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);
                file.SaveAs(path);
                
            }
            return path;
        }
    }
}