using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Transcoder.DataAccess.Settings;

namespace Transcoder.WebApp.Web.Settings
{
    public class WebPaypalSettings : PayPalSettings
    {
        public override T Setting<T>(string name) 
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                throw new Exception(string.Format("Could not find setting '{0}',", name));
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
