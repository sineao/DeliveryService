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
            {
                Response.Redirect("/Admin");
                return Content("Bereits angemeldet");
            }
                

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

        public ActionResult Galery(string Folder, string Name)
        {
            if (!CurrentUser.IsAdmin)
            {
                Response.Redirect("/Admin/Login");
                return Content("Nicht angemeldet");
            }

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(Name))
                {
                    var extension = Path.GetExtension(file.FileName);
                    var newFileName = Name + extension;
                    var path = Path.Combine(Server.MapPath("~/Content/Gallery/"), Folder, newFileName);

                    for (int i = 1; System.IO.File.Exists(path); i++ )
                    {
                        newFileName = Name + "-" + i + extension;
                        path = Path.Combine(Server.MapPath("~/Content/Gallery/"), Folder, newFileName);
                    }
                    file.SaveAs(path);
                }
            }

            var model = new Models.GaleryModel();

            model.Folders = Directory.GetDirectories(Server.MapPath("~/Content/Gallery/")).Select(x=>new DirectoryInfo(x).Name).ToList();

            return View(model);
        }
    }
}
