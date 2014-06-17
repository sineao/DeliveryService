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
                    return Content("Angemeldet");
                }
                else
                {
                    ViewBag.Error = true;
                }
            }

            var model = new Models.AdminModel();
            
            return View(model);
        }

        public ActionResult GaleryUpload(string Folder, string Name, string Description)
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

                    if (!string.IsNullOrEmpty(Description))
                    {
                        System.IO.File.WriteAllText(Path.Combine(Server.MapPath("~/Content/Gallery/"), Folder + "/description", newFileName) + ".txt", Description);
                    }

                    System.IO.File.WriteAllBytes(Path.Combine(Server.MapPath("~/Content/Gallery/"), Folder + "/thumbnail", newFileName), Utilities.ResizeImage(System.IO.File.ReadAllBytes(path), 150, 150, false));
                }
            }

            var model = new Models.GaleryModel();

            model.Folders = Directory.GetDirectories(Server.MapPath("~/Content/Gallery/")).Select(x=>new DirectoryInfo(x).Name).ToList();

            return View(model);
        }

        public ActionResult Galery()
        {
            var model = new Models.GaleryModel();

            foreach(var directory in Directory.GetDirectories(Server.MapPath("~/Content/Gallery/")))
            {
                var newFolder = new Models.GaleryFolder();
                newFolder.Name = new DirectoryInfo(directory).Name;
                foreach (var file in Directory.GetFiles(directory))
                {
                    var newFile = new Models.Picture();
                    newFile.File = Utilities.MapURL(file);
                    newFile.Name = new FileInfo(file).Name;

                    newFile.Thumbnail =  Utilities.MapURL(file.Replace(newFile.Name, "thumbnail/" + newFile.Name));
                    newFile.Description = System.IO.File.ReadAllText(file.Replace(newFile.Name, "description/" + newFile.Name + ".txt"));

                    newFolder.Pictures.Add(newFile);
                }
                model.GaleryFolders.Add(newFolder);
            }


            return View(model);
        }
    }
}
