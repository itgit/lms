using LMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace LMS.Controllers
{
    public class UploadController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileId = Path.GetRandomFileName();
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/Userfiles/"), fileId + "_" + fileName);
                    file.SaveAs(filePath);
                    db.Files.Add(new LMS.Models.File() { FileName = fileName, FilePath = filePath, Id = fileId, UserId = User.Identity.GetUserId() });
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
    }
}