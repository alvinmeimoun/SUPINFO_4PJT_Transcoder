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
            int userId = 0;
            if (controller.Request.Cookies["UserID"].Value != null)
            {
                int.TryParse(controller.Request.Cookies["UserID"].Value, out userId);
                return userId;
            }
            else
            {
                return 0;
            }
        }
    }
}
