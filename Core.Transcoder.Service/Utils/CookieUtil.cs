using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace Transcoder.WebApp.Web
{
    public static class CookieUtil
    {

        public static int GetUserId(Controller controller)
        {
            var httpCookie = controller.Request.Cookies["UserID"];
            if (httpCookie?.Value != null)
            {
                var userId = 0; 
                int.TryParse(httpCookie.Value, out userId);
                return userId;
            }
            return 0;
        }
    }
}
