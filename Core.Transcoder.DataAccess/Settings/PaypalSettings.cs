using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.Settings
{
    public abstract class PayPalSettings : ISettingsProvider
    {
        public string ApiDomain
        {
            get
            {
                return Setting<bool>("PayPal:Sandbox")
                    ? "api-3t.sandbox.paypal.com"
                    : "api-3t.paypal.com";
            }
        }

        public string CgiDomain
        {
            get { return Setting<bool>("PayPal:Sandbox") ? "www.sandbox.paypal.com" : "www.paypal.com"; }
        }

        public string Signature
        {
            get { return Setting<string>("PayPal:Signature"); }
        }

        public string Username
        {
            get { return Setting<string>("PayPal:Username"); }
        }

        public string Password
        {
            get { return Setting<string>("PayPal:Password"); }
        }

        public string ReturnUrl
        {
            get { return Setting<string>("PayPal:ReturnUrl"); }
        }

        public string CancelUrl
        {
            get { return Setting<string>("PayPal:CancelUrl"); }
        }
        
        public abstract T Setting<T>(string name);
    }
}
