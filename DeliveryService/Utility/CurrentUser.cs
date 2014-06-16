using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace DeliveryService.Utility
{
    public static class CurrentUser
    {
        public static bool IsAdmin
        {
            get { return (System.Web.HttpContext.Current.Session["IsAdmin"] != null && (bool)System.Web.HttpContext.Current.Session["IsAdmin"] == true); }
            set { System.Web.HttpContext.Current.Session["IsAdmin"] = value; }
        }
    }
}
