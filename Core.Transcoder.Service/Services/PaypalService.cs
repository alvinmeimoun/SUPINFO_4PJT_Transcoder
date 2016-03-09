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

namespace Core.Transcoder.Service.Services
{
    public class PaypalService
    {
        public static PayPalRedirect ExpressCheckout(PayPalOrder order)
        {
            NameValueCollection values = new NameValueCollection();

            values["METHOD"] = "SetExpressCheckout";
            values["RETURNURL"] = PayPalSettings.ReturnUrl;
            values["CANCELURL"] = PayPalSettings.CancelUrl;
            values["AMT"] = "";
            values["PAYMENTACTION"] = "Sale";
            values["CURRENCYCODE"] = "GBP";
            values["BUTTONSOURCE"] = "PP-ECWizard";
            values["USER"] = PayPalSettings.Username;
            values["PWD"] = PayPalSettings.Password;
            values["SIGNATURE"] = PayPalSettings.Signature;
            values["SUBJECT"] = "";
            values["VERSION"] = "2.3";
            values["AMT"] = order.Amount.ToString(CultureInfo.InvariantCulture);

            values = Submit(values);

            string ack = values["ACK"].ToLower();

            if (ack == "success" || ack == "successwithwarning")
            {
                return new PayPalRedirect
                {
                    Token = values["TOKEN"],
                    Url = String.Format("https://{0}/cgi-bin/webscr?cmd=_express-checkout&token={1}",
                        PayPalSettings.CgiDomain, values["TOKEN"])
                };
            }
            else
            {
                throw new Exception(values["L_LONGMESSAGE0"]);
            }
        }

        private static NameValueCollection Submit(NameValueCollection values)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string data = String.Join("&", values.Cast<string>()
                .Select(key => String.Format("{0}={1}", key, HttpUtility.UrlEncode(values[key]))));

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(
                String.Format("https://{0}/nvp", PayPalSettings.ApiDomain));

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

        public class PayPalOrder
        {
            public decimal Amount { get; set; }
        }

        public class PayPalRedirect
        {
            public string Url { get; set; }
            public string Token { get; set; }
        }

        public static class PayPalSettings
        {
            public static string ApiDomain
            {
                get
                {
                    return Setting<bool>("PayPal:Sandbox")
                        ? "api-3t.sandbox.paypal.com"
                        : "api-3t.paypal.com";
                }
            }

            public static string CgiDomain
            {
                get { return Setting<bool>("PayPal:Sandbox") ? "www.sandbox.paypal.com" : "www.paypal.com"; }
            }

            public static string Signature
            {
                get { return Setting<string>("PayPal:Signature"); }
            }

            public static string Username
            {
                get { return Setting<string>("PayPal:Username"); }
            }

            public static string Password
            {
                get { return Setting<string>("PayPal:Password"); }
            }

            public static string ReturnUrl
            {
                get { return Setting<string>("PayPal:ReturnUrl"); }
            }

            public static string CancelUrl
            {
                get { return Setting<string>("PayPal:CancelUrl"); }
            }

            private static T Setting<T>(string name)
            {
                string value = ConfigurationManager.AppSettings[name];

                if (value == null)
                {
                    throw new Exception(String.Format("Could not find setting '{0}',", name));
                }

                return (T) Convert.ChangeType(value, typeof (T), CultureInfo.InvariantCulture);
            }
        }
    }
}
