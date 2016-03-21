using Core.Transcoder.PayPalMvc.Enums;

namespace Core.Transcoder.PayPalMvc
{
    /// <summary>
    /// Represents a transaction registration that is sent to PayPal. 
    /// This should be serialized using the HttpPostSerializer.
    /// </summary>
    public class GetExpressCheckoutDetailsRequest : CommonRequest
    {
        readonly string token;

        public GetExpressCheckoutDetailsRequest(string token)
        {
            base.method = RequestType.GetExpressCheckoutDetails;
            
            this.token = token;
        }

        public string TOKEN
        {
            get { return token; }
        }

    }
}