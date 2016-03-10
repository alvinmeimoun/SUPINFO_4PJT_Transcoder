using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.DataAccess.ViewModels
{
    public class PaypalModels
    {
        //public class PayPalOrder
        //{
        //    public decimal Amount { get; set; }
        //}

        //public class PayPalRedirect
        //{
        //    public string Url { get; set; }
        //    public string Token { get; set; }
        //}

        public class PayPalTransaction
        {
            public string RequestId { get; set; }
            public string TrackingReference { get; set; }
            public DateTime RequestTime { get; set; }
            public string RequestStatus { get; set; }
            public string TimeStamp { get; set; }
            public string RequestError { get; set; }
            public string Token { get; set; }
            public string PayerId { get; set; }
            public string RequestData { get; set; }
            public string PaymentTransactionId { get; set; }
            public string PaymentError { get; set; }
        }
    }
}
