using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Core.Transcoder.DataAccess.ViewModels;
using PayPalMvc;

namespace Core.Transcoder.Service.Services
{
    public interface IPaypalService
    {
        SetExpressCheckoutResponse SendPayPalSetExpressCheckoutRequest(PanierViewModel cart, string serverURL, string userEmail = null);
        GetExpressCheckoutDetailsResponse SendPayPalGetExpressCheckoutDetailsRequest(string token);
        DoExpressCheckoutPaymentResponse SendPayPalDoExpressCheckoutPaymentRequest(PanierViewModel cart, string token, string payerId);
    }

    public class PaypalService : IPaypalService
    {

        private PayPalMvc.ITransactionRegistrar _payPalTransactionRegistrar = new PayPalMvc.TransactionRegistrar();

        public SetExpressCheckoutResponse SendPayPalSetExpressCheckoutRequest(PanierViewModel cart, string serverURL, string userEmail = null)
        {
            try
            {
                Debug.WriteLine("SendPayPalSetExpressCheckoutRequest");

                // Optional handling of cart items: If there is only a single item being sold we don't need a list of expressCheckoutItems
                // However if you're selling a single item as a sale consider also adding it as an ExpressCheckoutItem as it looks better once you get to PayPal's site
                // Note: ExpressCheckoutItems are currently NOT stored by PayPal against the sale in the users order history so you need to keep your own records of what items were in a cart
                List<ExpressCheckoutItem> expressCheckoutItems = null;
                if (cart.ListOfConversions != null)
                {
                    expressCheckoutItems = new List<ExpressCheckoutItem>();
                    foreach (ListTaskViewModel item in cart.ListOfConversions)
                        expressCheckoutItems.Add(new ExpressCheckoutItem(1, new decimal(item.PRICE), item.FILE_URL_ACCESS, ""));
                }

                SetExpressCheckoutResponse response = _payPalTransactionRegistrar.SendSetExpressCheckout("EUR", new decimal(cart.GlobalPrice), "Transcoder", cart.TransactionId, serverURL, expressCheckoutItems, userEmail);

                // Add a PayPal transaction record
                PaypalModels.PayPalTransaction transaction = new PaypalModels.PayPalTransaction
                {
                    RequestId = response.RequestId,
                    TrackingReference = cart.TransactionId.ToString(),
                    RequestTime = DateTime.Now,
                    RequestStatus = response.ResponseStatus.ToString(),
                    TimeStamp = response.TIMESTAMP,
                    RequestError = response.ErrorToString,
                    Token = response.TOKEN,
                };

                // Store this transaction in your Database

                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex);
            }
            return null;
        }

        public GetExpressCheckoutDetailsResponse SendPayPalGetExpressCheckoutDetailsRequest(string token)
        {
            try
            {
                Debug.WriteLine("SendPayPalGetExpressCheckoutDetailsRequest");
                GetExpressCheckoutDetailsResponse response = _payPalTransactionRegistrar.SendGetExpressCheckoutDetails(token);

                // Add a PayPal transaction record
                PaypalModels.PayPalTransaction transaction = new PaypalModels.PayPalTransaction
                {
                    RequestId = response.RequestId,
                    TrackingReference = response.TrackingReference,
                    RequestTime = DateTime.Now,
                    RequestStatus = response.ResponseStatus.ToString(),
                    TimeStamp = response.TIMESTAMP,
                    RequestError = response.ErrorToString,
                    Token = response.TOKEN,
                    PayerId = response.PAYERID,
                    RequestData = response.ToString,
                };

                // Store this transaction in your Database

                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex);
            }
            return null;
        }

        public DoExpressCheckoutPaymentResponse SendPayPalDoExpressCheckoutPaymentRequest(PanierViewModel cart, string token, string payerId)
        {
            try
            {
                Debug.WriteLine("SendPayPalDoExpressCheckoutPaymentRequest");
                DoExpressCheckoutPaymentResponse response = _payPalTransactionRegistrar.SendDoExpressCheckoutPayment(token, payerId, "EUR", new decimal(cart.GlobalPrice));

                // Add a PayPal transaction record
                PaypalModels.PayPalTransaction transaction = new PaypalModels.PayPalTransaction
                {
                    RequestId = response.RequestId,
                    TrackingReference = cart.TransactionId,
                    RequestTime = DateTime.Now,
                    RequestStatus = response.ResponseStatus.ToString(),
                    TimeStamp = response.TIMESTAMP,
                    RequestError = response.ErrorToString,
                    Token = response.TOKEN,
                    RequestData = response.ToString,
                    PaymentTransactionId = response.PaymentTransactionId,
                    PaymentError = response.PaymentErrorToString,
                };

                // Store this transaction in your Database

                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex);
            }
            return null;
        }

        //public PaypalModels.PayPalRedirect ExpressCheckout(PaypalModels.PayPalOrder order)
        //{
        //    NameValueCollection values = new NameValueCollection();

        //    values["METHOD"] = "SetExpressCheckout";
        //    values["RETURNURL"] = payPalSettings.ReturnUrl;
        //    values["CANCELURL"] = payPalSettings.CancelUrl;
        //    values["AMT"] = "";
        //    values["PAYMENTACTION"] = "Sale";
        //    values["CURRENCYCODE"] = "GBP";
        //    values["BUTTONSOURCE"] = "PP-ECWizard";
        //    values["USER"] = payPalSettings.Username;
        //    values["PWD"] = payPalSettings.Password;
        //    values["SIGNATURE"] = payPalSettings.Signature;
        //    values["SUBJECT"] = "";
        //    values["VERSION"] = "2.3";
        //    values["AMT"] = order.Amount.ToString(CultureInfo.InvariantCulture);

        //    values = Submit(values);

        //    string ack = values["ACK"].ToLower();

        //    if (ack == "success" || ack == "successwithwarning")
        //    {
        //        return new PaypalModels.PayPalRedirect
        //        {
        //            Token = values["TOKEN"],
        //            Url = string.Format("https://{0}/cgi-bin/webscr?cmd=_express-checkout&token={1}",
        //                payPalSettings.CgiDomain, values["TOKEN"])
        //        };
        //    }
        //    else
        //    {
        //        throw new Exception(values["L_LONGMESSAGE0"]);
        //    }
        //}

        //private NameValueCollection Submit(NameValueCollection values)
        //{
        //    ServicePointManager.Expect100Continue = true;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    string data = string.Join("&", values.Cast<string>()
        //        .Select(key => string.Format("{0}={1}", key, HttpUtility.UrlEncode(values[key]))));

        //    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(
        //        string.Format("https://{0}/nvp", payPalSettings.ApiDomain));

        //    request.Method = "POST";
        //    request.ContentLength = data.Length;

        //    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
        //    {
        //        writer.Write(data);
        //    }

        //    using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
        //    {
        //        return HttpUtility.ParseQueryString(reader.ReadToEnd());
        //    }
        //}


    }
}
