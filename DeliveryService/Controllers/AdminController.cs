using DeliveryService.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeliveryService.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            if (!CurrentUser.IsAdmin)
            {
                Response.Redirect("/Admin/Login");
                return Content("Nicht angemeldet");
            }

            return View();
        }

        public ActionResult Login(string Name, string Passwort)
        {
            if (CurrentUser.IsAdmin)
                return Index();

            if (!string.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Passwort))
            {
                if (Name == "admin" && Passwort == "admin123")
                {
                    CurrentUser.IsAdmin = true;
                    Response.Redirect("/Admin");
                }
                else
                {
                    ViewBag.Error = true;
                }
            }

            var model = new Models.AdminModel();
            
            return View(model);
        }

        public ActionResult GalleryUpload(string Folder)
        {
            if (!CurrentUser.IsAdmin)
            {
                Response.Redirect("/Admin/Login");
                return Content("Nicht angemeldet");
            }

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Gallery/"), Folder, fileName);
                    file.SaveAs(path);
                }
            }

            return View();
        }
    }
}
