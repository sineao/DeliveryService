using DeliveryService.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                    var path = Path.Combine(Server.MapPath("~/Content/Galery/"), Folder, newFileName);

                    for (int i = 1; System.IO.File.Exists(path); i++)
                    {
                        newFileName = Name + "-" + i + extension;
                        path = Path.Combine(Server.MapPath("~/Content/Galery/"), Folder, newFileName);
                    }
                    file.SaveAs(path);

                    if (!string.IsNullOrEmpty(Description))
                    {
                        System.IO.File.WriteAllText(Path.Combine(Server.MapPath("~/Content/Galery/"), Folder + "/description", newFileName) + ".txt", Description);
                    }

                    System.IO.File.WriteAllBytes(Path.Combine(Server.MapPath("~/Content/Galery/"), Folder + "/thumbnail", newFileName), Utilities.ResizeImage(System.IO.File.ReadAllBytes(path), 150, 150, false));
                }
            }

            var model = new Models.GaleryModel();

            model.Folders = Directory.GetDirectories(Server.MapPath("~/Content/Galery/")).Select(x => new DirectoryInfo(x).Name).ToList();

            return View(model);
        }

        public ActionResult NewFolder(string Name)
        {
            if (!CurrentUser.IsAdmin)
            {
                Response.Redirect("/Admin/Login");
                return Content("Nicht angemeldet");
            }

            if (!string.IsNullOrEmpty(Name))
            {
                var galeryPath = Server.MapPath("~/Content/Galery/" + Name + "/");
                if (!Directory.Exists(galeryPath))
                {
                    Directory.CreateDirectory(galeryPath);
                    Directory.CreateDirectory(galeryPath + "thumbnail/");
                    Directory.CreateDirectory(galeryPath + "description/");
                }
            }

            Response.Redirect("/Admin/Galery");
            return Content("hinzugefügt");
        }

        public ActionResult Galery()
        {
            if (!CurrentUser.IsAdmin)
            {
                Response.Redirect("/Admin/Login");
                return Content("Nicht angemeldet");
            }

            var model = new Models.GaleryModel();

            foreach (var directory in Directory.GetDirectories(Server.MapPath("~/Content/Galery/")))
            {
                var newFolder = new Models.GaleryFolder();
                newFolder.Name = new DirectoryInfo(directory).Name;
                foreach (var file in Directory.GetFiles(directory))
                {
                    var newFile = new Models.Picture();
                    newFile.File = Utilities.MapURL(file);
                    newFile.Name = new FileInfo(file).Name;

                    newFile.Thumbnail = Utilities.MapURL(file.Replace(newFile.Name, "thumbnail/" + newFile.Name));
                    newFile.Description = System.IO.File.ReadAllText(file.Replace(newFile.Name, "description/" + newFile.Name + ".txt"));

                    newFolder.Pictures.Add(newFile);
                }
                model.GaleryFolders.Add(newFolder);
            }


            return View(model);
        }

        public ActionResult DrawGalery()
        {
            if (!CurrentUser.IsAdmin)
            {
                Response.Redirect("/Admin/Login");
                return Content("Nicht angemeldet");
            }


            var model = new Models.GaleryModel();

            foreach (var directory in Directory.GetDirectories(Server.MapPath("~/Content/Galery/")))
            {
                var newFolder = new Models.GaleryFolder();
                newFolder.Name = new DirectoryInfo(directory).Name;
                foreach (var file in Directory.GetFiles(directory))
                {
                    var newFile = new Models.Picture();
                    newFile.File = Utilities.MapURL(file);
                    newFile.Name = new FileInfo(file).Name;

                    newFile.Thumbnail = Utilities.MapURL(file.Replace(newFile.Name, "thumbnail/" + newFile.Name));
                    newFile.Description = System.IO.File.ReadAllText(file.Replace(newFile.Name, "description/" + newFile.Name + ".txt"));

                    newFolder.Pictures.Add(newFile);
                }
                model.GaleryFolders.Add(newFolder);
            }


            StringBuilder galery = new StringBuilder();


            galery.AppendLine("<section class=\"two\" id=\"galery\">");
            galery.AppendLine("	<div class=\"container\">");
            galery.AppendLine("");
            galery.AppendLine("		<header>");
            galery.AppendLine("			<h2>Galery</h2>");
            galery.AppendLine("		</header>");
            galery.AppendLine("");
            galery.AppendLine("		<p>");
            galery.AppendLine("			Hier sehen Sie diverse Bilder.");
            galery.AppendLine("		</p>");
            galery.AppendLine("");

            foreach (var folder in model.GaleryFolders)
            {
                if (folder.Pictures == null || folder.Pictures.Count <= 0)
                    continue;
                galery.AppendLine("		<header>");
                galery.AppendLine("			<h3>" + folder.Name + "</h3>");
                galery.AppendLine("		</header>");
                galery.AppendLine("		<div class=\"row zoom-gallery\">");
                galery.AppendLine("			<div class=\"4u\">");

                for (int i = 0; i < folder.Pictures.Count; i++)
                {
                    if (i % 3 == 0)
                    {
                        galery.AppendLine("				<article class=\"item\">");
                        galery.AppendLine("					<a class=\"image full colorbox-" + folder.Name + "\" href=\"" + folder.Pictures[i].File + "\" title=\"" + folder.Pictures[i].Description + "\"><img alt=\"\" src=\"" + folder.Pictures[i].Thumbnail + "\"></a>");
                        galery.AppendLine("					<header>");
                        galery.AppendLine("						<h3>" + folder.Pictures[i].Description + "</h3>");
                        galery.AppendLine("					</header>");
                        galery.AppendLine("				</article>");
                    }
                }

                galery.AppendLine("			</div>");
                galery.AppendLine("			<div class=\"4u\">");

                for (int i = 0; i < folder.Pictures.Count; i++)
                {
                    if (i % 3 == 1)
                    {
                        galery.AppendLine("				<article class=\"item\">");
                        galery.AppendLine("					<a class=\"image full colorbox-" + folder.Name + "\" href=\"" + folder.Pictures[i].File + "\" title=\"" + folder.Pictures[i].Description + "\"><img alt=\"\" src=\"" + folder.Pictures[i].Thumbnail + "\"></a>");
                        galery.AppendLine("					<header>");
                        galery.AppendLine("						<h3>" + folder.Pictures[i].Description + "</h3>");
                        galery.AppendLine("					</header>");
                        galery.AppendLine("				</article>");
                    }
                }

                galery.AppendLine("			</div>");
                galery.AppendLine("			<div class=\"4u\">");

                for (int i = 0; i < folder.Pictures.Count; i++)
                {
                    if (i % 3 == 2)
                    {
                        galery.AppendLine("				<article class=\"item\">");
                        galery.AppendLine("					<a class=\"image full colorbox-" + folder.Name + "\" href=\"" + folder.Pictures[i].File + "\" title=\"" + folder.Pictures[i].Description + "\"><img alt=\"\" src=\"" + folder.Pictures[i].Thumbnail + "\"></a>");
                        galery.AppendLine("					<header>");
                        galery.AppendLine("						<h3>" + folder.Pictures[i].Description + "</h3>");
                        galery.AppendLine("					</header>");
                        galery.AppendLine("				</article>");
                    }
                }

                galery.AppendLine("			</div>");
                galery.AppendLine("		</div>");
            }
            galery.AppendLine("");
            galery.AppendLine("	</div>");
            galery.AppendLine("</section>");


            galery.AppendLine("<script>");
            galery.AppendLine("     $(document).ready(function(){");

            foreach (var folder in model.GaleryFolders)
            {
                //$('.zoom-gallery').magnificPopup({
                //delegate: 'a',
                //type: 'image',
                //closeOnContentClick: false,
                //closeBtnInside: false,
                //mainClass: 'mfp-with-zoom mfp-img-mobile',
                //image: {
                //    verticalFit: true,
                //    titleSrc: function (item) {
                //        return item.el.attr('title');
                //    }
                //},
                //gallery: {
                //    enabled: true
                //},
                //zoom: {
                //    enabled: true,
                //    duration: 300, // don't foget to change the duration also in CSS
                //    opener: function (element) {
                //        return element.find('img');
                //    }
                //}

            });
            }
            galery.AppendLine("     });");
            galery.AppendLine("</script>");

            System.IO.File.WriteAllText(Server.MapPath("~/Content/Galery/galery.html"), galery.ToString());

            Response.Redirect("/Admin/Galery");
            return Content("Aktualisiert");
        }
    }
}
