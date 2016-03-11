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

        public SetExpressCheckoutResponse SendPayPalSetExpressCheckoutRequest(PanierViewModel panier, string serverURL, string userEmail = null)
        {
            try
            {
                List<ExpressCheckoutItem> expressCheckoutItems = null;
                if (panier.ListOfConversions != null)
                {
                    expressCheckoutItems = new List<ExpressCheckoutItem>();
                    foreach (ListTaskViewModel item in panier.ListOfConversions)
                        expressCheckoutItems.Add(new ExpressCheckoutItem(1, new decimal(item.PRICE), item.FILE_URL_ACCESS, ""));
                }

                SetExpressCheckoutResponse response = _payPalTransactionRegistrar.SendSetExpressCheckout("EUR", new decimal(panier.GlobalPrice), "Transcoder", panier.PaypalTransactionId, serverURL, expressCheckoutItems, userEmail);

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
                GetExpressCheckoutDetailsResponse response = _payPalTransactionRegistrar.SendGetExpressCheckoutDetails(token);

                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex);
            }
            return null;
        }

        public DoExpressCheckoutPaymentResponse SendPayPalDoExpressCheckoutPaymentRequest(PanierViewModel panier, string token, string payerId)
        {
            try
            {
                DoExpressCheckoutPaymentResponse response = _payPalTransactionRegistrar.SendDoExpressCheckoutPayment(token, payerId, "EUR", new decimal(panier.GlobalPrice));

                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex);
            }
            return null;
        }

    }
}
