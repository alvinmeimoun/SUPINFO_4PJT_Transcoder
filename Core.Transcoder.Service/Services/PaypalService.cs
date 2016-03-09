using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Core.Transcoder.DataAccess.Settings;
using Core.Transcoder.DataAccess.ViewModels;

namespace Core.Transcoder.Service.Services
{
    public class PaypalService
    {
        private PayPalSettings payPalSettings;

        public PaypalService(PayPalSettings payPalSettings)
        {
            this.payPalSettings = payPalSettings;
        }

        public PaypalModels.PayPalRedirect ExpressCheckout(PaypalModels.PayPalOrder order)
        {
            NameValueCollection values = new NameValueCollection();

            values["METHOD"] = "SetExpressCheckout";
            values["RETURNURL"] = payPalSettings.ReturnUrl;
            values["CANCELURL"] = payPalSettings.CancelUrl;
            values["AMT"] = "";
            values["PAYMENTACTION"] = "Sale";
            values["CURRENCYCODE"] = "GBP";
            values["BUTTONSOURCE"] = "PP-ECWizard";
            values["USER"] = payPalSettings.Username;
            values["PWD"] = payPalSettings.Password;
            values["SIGNATURE"] = payPalSettings.Signature;
            values["SUBJECT"] = "";
            values["VERSION"] = "2.3";
            values["AMT"] = order.Amount.ToString(CultureInfo.InvariantCulture);

            values = Submit(values);

            string ack = values["ACK"].ToLower();

            if (ack == "success" || ack == "successwithwarning")
            {
                return new PaypalModels.PayPalRedirect
                {
                    Token = values["TOKEN"],
                    Url = string.Format("https://{0}/cgi-bin/webscr?cmd=_express-checkout&token={1}",
                        payPalSettings.CgiDomain, values["TOKEN"])
                };
            }
            else
            {
                throw new Exception(values["L_LONGMESSAGE0"]);
            }
        }

        private NameValueCollection Submit(NameValueCollection values)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string data = string.Join("&", values.Cast<string>()
                .Select(key => string.Format("{0}={1}", key, HttpUtility.UrlEncode(values[key]))));

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(
                string.Format("https://{0}/nvp", payPalSettings.ApiDomain));

            request.Method = "POST";
            request.ContentLength = data.Length;

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(data);
            }

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return HttpUtility.ParseQueryString(reader.ReadToEnd());
            }
        }

        
    }
}
