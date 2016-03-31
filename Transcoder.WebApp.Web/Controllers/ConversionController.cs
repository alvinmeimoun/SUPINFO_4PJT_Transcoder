using Core.Transcoder.DataAccess.ViewModels;
using Core.Transcoder.FFmpegWrapper;
using Core.Transcoder.Service;
using Core.Transcoder.Service.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Core.Transcoder.Service.Services;
using Core.Transcoder.PayPalMvc;
using Core.Transcoder.PayPalMvc.Enums;
using Core.Transcoder.DataAccess;

namespace Transcoder.WebApp.Web.Controllers
{
    public class ConversionController : Controller
    {
        PaypalService paypalService = new PaypalService();
        TASK_Service taskService = new TASK_Service();

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

        public ActionResult MesConversions()
        {
            int UserId = CookieUtil.GetUserId(this);
            if (UserId == 0)
                return RedirectToAction("Index", "Home");

            var commandes = new TASK_Service().GetListCommandesViewModel(UserId);

            return View("ListCommandes", commandes);
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
                    return RedirectToAction("Panier");
            }
            return RedirectToAction("Panier");
        }


        [HttpPost]
        public ActionResult ValiderPanier(PanierViewModel model)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (!ModelState.IsValid)
                return View("Panier", model);

            Session["Panier"] = model;

            string serverURL = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/");
            SetExpressCheckoutResponse transactionResponse = paypalService.SendPayPalSetExpressCheckoutRequest(model, serverURL);
            
            if (transactionResponse == null || transactionResponse.ResponseStatus != Core.Transcoder.PayPalMvc.Enums.ResponseType.Success)
            {
                string errorMessage = (transactionResponse == null) ? "Null Paypal Transaction Response" : transactionResponse.ErrorToString;
                Debug.WriteLine("Error initiating PayPal SetExpressCheckout transaction. Error: " + errorMessage);
                return RedirectToAction("Panier", model);
            }
            return Redirect(string.Format(Configuration.Current.PayPalRedirectUrl, transactionResponse.TOKEN));
        }

        [HttpPost]
        public ActionResult UploadFromUrl(string url)
        {
            var client = new System.Net.WebClient();
            try
            {
                string path = Server.MapPath("~/UploadedFiles/") + url.Substring(url.LastIndexOf('/') + 1);
                client.DownloadFile(url, path);
                FileInfo fileInfo = new FileInfo(path);
                VideoFile videoFile = new VideoFile(path);

                double VideoDuration = videoFile.Duration.TotalMinutes;
                // On génére le prix en fonction de la durée
                double price = PriceGeneratorUtil.GetPriceByDuration(VideoDuration);
                // On recupere le format de base de la video
                string format = fileInfo.Extension.Substring(fileInfo.Extension.LastIndexOf('.') + 1);
                var formatBase = new FORMAT_Service().GetFormatByName(format);


                // Si le format a été trouvé dans nos bases on va proposer les conversions liées possibles.
                if (formatBase != null)
                {
                    var listFormatTypes = new FORMAT_TYPE_Service().GetSelectListFormatTypeByFormat((int)formatBase.FK_ID_FORMAT_TYPE);

                    return Json(new
                    {
                        success = "true",
                        fileUrl = path,
                        fileLength = fileInfo.Length,
                        fileName = fileInfo.Name,
                        fileFormatBase = formatBase.PK_ID_FORMAT,
                        listFormatType = new SelectList(listFormatTypes, "Value", "Text"),
                        fileDuration = VideoDuration.ToString(),
                        filePrice = price.ToString()
                    });
                }
                else
                {
                    return Json(new { success = "false" });
                }
            }
            catch(Exception e)
            {
                return Json(new { success = "false" });
            }
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

        [HttpGet]
        public ActionResult Telecharger(int id)
        {
            TASK task = new TASK_Service().GetTaskById(id);
            int index = task.FILE_URL_DESTINATION.LastIndexOf('\\') + 1;
            string file = task.FILE_URL_DESTINATION.Substring(index);
            DownloadFile(file, task.FILE_URL_DESTINATION);

            return RedirectToAction("MesConversions");
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
        
        public ActionResult OrderPaypalAuthorized(string token, string PayerID)
        {
            PanierViewModel panier = (PanierViewModel)Session["Panier"];

            //Récupération des détails de l'appel Express Checkout
            GetExpressCheckoutDetailsResponse getDetailsResponse = paypalService.SendPayPalGetExpressCheckoutDetailsRequest(token);
            if (getDetailsResponse == null || getDetailsResponse.ResponseStatus != Core.Transcoder.PayPalMvc.Enums.ResponseType.Success)
            {
                string errorMessage = (getDetailsResponse == null) ? "Null Transaction Response" : getDetailsResponse.ErrorToString;
                Debug.WriteLine("Error initiating PayPal GetExpressCheckoutDetails transaction. Error: " + errorMessage);
                return RedirectToAction("Panier", panier);
            }

            //Paiement de la commande
            DoExpressCheckoutPaymentResponse doCheckoutRepsonse = paypalService.SendPayPalDoExpressCheckoutPaymentRequest(panier, token, PayerID);

            if (doCheckoutRepsonse == null || doCheckoutRepsonse.ResponseStatus != Core.Transcoder.PayPalMvc.Enums.ResponseType.Success)
            {
                if (doCheckoutRepsonse != null && doCheckoutRepsonse.L_ERRORCODE0 == "10486")
                {
                    Debug.WriteLine("10486 error (bad funding method - typically an invalid credit card)");
                    return Redirect(string.Format(Configuration.Current.PayPalRedirectUrl, token));
                }
                string errorMessage = (doCheckoutRepsonse == null) ? "Null Transaction Response" : doCheckoutRepsonse.ErrorToString;
                Debug.WriteLine("Error initiating PayPal DoExpressCheckoutPayment transaction. Error: " + errorMessage);
                return RedirectToAction("Panier", panier);
            }

            if (doCheckoutRepsonse.PaymentStatus == PaymentStatus.Completed)
            {
                taskService.SetAllTasksPaidForTransaction(panier.TransactionId);
                MailUtil.SendMail(Core.Transcoder.Service.Enums.StringManager.PAIEMENT_ACCEPTE, null, panier);

                return RedirectToAction("OrderPaidConfirm");
            }
            else
            {
                Debug.WriteLine($"Error taking PayPal payment. Error: " + doCheckoutRepsonse.ErrorToString +
                                " - Payment Error: " + doCheckoutRepsonse.PaymentErrorToString);
                TempData["TransactionResult"] = doCheckoutRepsonse.PAYMENTREQUEST_0_LONGMESSAGE;
                return RedirectToAction("Panier", panier);
            }
        }

        public ActionResult OrderPaidConfirm()
        {
            PanierViewModel cart = (PanierViewModel)Session["Panier"];
            ViewBag.TrackingReference = cart.TransactionId;
            ViewBag.Description = "Transcoder";
            ViewBag.TotalCost = cart.GlobalPrice;
            ViewBag.Currency = "EUR";

            Session["Panier"] = null;

            return RedirectToAction("Index", "Home");
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

        private void DownloadFile(string fileName, string fileUrl)
        {
            // **************************************************
            string strFileName =
                string.Format("{0}", fileName);

            if (System.IO.File.Exists(fileUrl) == false)
            {
                return ;
            }
            // **************************************************

            System.IO.Stream oStream = null;

            try
            {
                // Open the file
                oStream =
                    new System.IO.FileStream
                        (path: fileUrl,
                        mode: System.IO.FileMode.Open,
                        share: System.IO.FileShare.Read,
                        access: System.IO.FileAccess.Read);

                // **************************************************
                Response.Buffer = false;

                // Setting the unknown [ContentType]
                // will display the saving dialog for the user
                Response.ContentType = "application/octet-stream";

                // With setting the file name,
                // in the saving dialog, user will see
                // the [strFileName] name instead of [download]!
                Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName);

                long lngFileLength = oStream.Length;

                // Notify user (client) the total file length
                Response.AddHeader("Content-Length", lngFileLength.ToString());
                // **************************************************

                // Total bytes that should be read
                long lngDataToRead = lngFileLength;

                // Read the bytes of file
                while (lngDataToRead > 0)
                {
                    // The below code is just for testing! So we commented it!
                    //System.Threading.Thread.Sleep(200);

                    // Verify that the client is connected or not?
                    if (Response.IsClientConnected)
                    {
                        // 8KB
                        int intBufferSize = 8 * 1024;

                        // Create buffer for reading [intBufferSize] bytes from file
                        byte[] bytBuffers =
                            new System.Byte[intBufferSize];

                        // Read the data and put it in the buffer.
                        int intTheBytesThatReallyHasBeenReadFromTheStream =
                            oStream.Read(buffer: bytBuffers, offset: 0, count: intBufferSize);

                        // Write the data from buffer to the current output stream.
                        Response.OutputStream.Write
                            (buffer: bytBuffers, offset: 0,
                            count: intTheBytesThatReallyHasBeenReadFromTheStream);

                        // Flush (Send) the data to output
                        // (Don't buffer in server's RAM!)
                        Response.Flush();

                        lngDataToRead =
                            lngDataToRead - intTheBytesThatReallyHasBeenReadFromTheStream;
                    }
                    else
                    {
                        // Prevent infinite loop if user disconnected!
                        lngDataToRead = -1;
                    }
                }
            }
            catch { }
            finally
            {
                if (oStream != null)
                {
                    //Close the file.
                    oStream.Close();
                    oStream.Dispose();
                    oStream = null;
                }
                Response.Close();
            }
        }
    }
}