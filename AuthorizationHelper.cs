using System;
using System.Web;

namespace NMU_BookTrade
{
    public static class AuthorizationHelper
    {
        // This method checks if a user is logged in and has the correct access role
        public static void Authorize(string requiredAccessID)
        {
            if (HttpContext.Current.Session["AccessID"] == null)
            {
                // Not logged in – send back to login with return URL
                string returnUrl = HttpContext.Current.Request.RawUrl;
                HttpContext.Current.Response.Redirect("~/UserManagement/Login.aspx?returnUrl=" + HttpUtility.UrlEncode(returnUrl));
            }

            string currentAccessID = HttpContext.Current.Session["AccessID"].ToString();

            if (currentAccessID != requiredAccessID)
            {
                // Wrong role – send to error page
                HttpContext.Current.Response.Redirect("~/UserManagement/AccessDenied.aspx");
            }
        }

    }
}
