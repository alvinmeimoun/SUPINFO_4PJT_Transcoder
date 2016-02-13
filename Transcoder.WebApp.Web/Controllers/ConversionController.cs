using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.Service;
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
            int UserId = CookieUtil.GetUserId(this);
            if (UserId == 0)
                return RedirectToAction("Index", "Home");
            
            var listOfConversions = new TASK_Service().GetListTaskViewModelByUserId(UserId);

            return View("ListConversions",listOfConversions);
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


        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                string path = "";
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);
                    file.SaveAs(path);
                }
                int indexOf = file.FileName.IndexOf('.') + 1;
                string format = file.FileName.Substring(indexOf);
                var formatBase = new FORMAT_Service().GetFormatByName(format);
                if (formatBase != null)
                {
                    var listFormatTypes = new FORMAT_TYPE_Service().GetSelectListFormatTypeByFormat((int)formatBase.FK_ID_FORMAT_TYPE);

                    return Json(new { success = "true", fileUrl = path, fileLength = file.ContentLength, fileName = file.FileName, fileFormatBase = formatBase.PK_ID_FORMAT,
                        listFormatType = new SelectList(listFormatTypes, "Value", "Text")
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

    }
}