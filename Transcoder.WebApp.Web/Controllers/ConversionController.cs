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
            return View();
        }


        public ActionResult AddConversion()
        {
            int userId = 0;
            int.TryParse(Request.Cookies["UserID"].Value, out userId);
            CreateTaskViewModel model = new TASK_Service().InitCreateTaskViewModel(userId);
            return View(model);
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
                var listFormatTypes = new FORMAT_TYPE_Service().GetSelectListFormatTypeByFormat((int)formatBase.FK_ID_FORMAT_TYPE);
                if (formatBase != null)
                {
                    return Json(new { success = "true", fileUrl = path, fileLength = file.ContentLength, fileFormatBase = formatBase.PK_ID_FORMAT,
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