using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class PaypalModels
    {
        public class PayPalOrder
        {
            public decimal Amount { get; set; }
        }

        public class PayPalRedirect
        {
            public string Url { get; set; }
            public string Token { get; set; }
        }
    }
}
