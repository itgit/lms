using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ExtensionMethods;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web.Security;

namespace LMS.Controllers
{
    public class FilesController : Controller
    {
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Files
        [Authorize]
        public ActionResult Index(int? id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var files = db.Files.Include(f => f.User); // all files with the same activitytype as the logged in user

            var adminRole = (from r in db.Roles where r.Name.Contains("admin") select r).FirstOrDefault();
            var admins = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(adminRole.Id)).ToList();

            if (!User.IsInRole("admin"))
            {
                var groupActivityTypeIds = user.Group.Activities.Select(a => a.ActivityTypeId).ToList();
                files = files.Where(f => (f.IsShared && (f.User.GroupId == user.GroupId || (f.User.Roles.Select(r => r.RoleId).Contains(adminRole.Id) && groupActivityTypeIds.Contains(f.ActivityTypeId))) || f.UserId == user.Id)); //only see your groups shared or your own files
            }
            else
            {
                if (id != null)
                {
                    var groupActivityTypeIds = db.Groups.Find(id).Activities.Select(a => a.ActivityTypeId).ToList();
                    files = files.Where(f => f.User.GroupId == id || f.User.Roles.Select(r => r.RoleId).Contains(adminRole.Id) && groupActivityTypeIds.Contains(f.ActivityTypeId));
                }
            }

            ViewBag.GroupId = id;
            ViewBag.isYourGroup = UserManager.FindById(User.Identity.GetUserId()).GroupId == id;

            return View(files.OrderBy(f => f.FileName).ToList());
        }

        // GET: Files/Details/5
        [Authorize]
        public ActionResult Details(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Details(Guid id, string content)
        {
            var now = DateTime.Now;
            var userId = User.Identity.GetUserId();

            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(content))
            {
                db.Comments.Add(new Comment() { Content = content, UserId = userId, FileId = file.Id, TimeStamp = now });
                db.SaveChanges();
            }
            return View(file);
        }

        [Authorize]
        public ActionResult Upload()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            //var activities = db.ActivityTypes.Where(at => at.Activities == user.Group.Activities);

            ViewBag.ActivityTypeId =
            from a in db.Activities
            where a.GroupId == user.GroupId
            join at in db.ActivityTypes on a.ActivityTypeId equals at.Id
            orderby at.Name ascending
            select new SelectListItem
            {
                Value = at.Id.ToString(),
                Text = at.Name
            };

            //ViewBag.ActivityTypeId = users;//new SelectList(users.ToList(), "Id", "Name");
            //"Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize]
        //public ActionResult Upload(HttpPostedFileBase Upload, bool isShared, int activityTypeId, string content)
        public ActionResult Upload([Bind(Include = "Upload,IsShared,ActivityTypeId")] FileViewModel file, string Comment)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Couldn't find user!");
            }
            var user = db.Users.Find(userId);
            var group = db.Users.Find(userId).Group;

            if (ModelState.IsValid)
            {
                var fileId = Guid.NewGuid();
                var fileName = System.IO.Path.GetFileName(file.Upload.FileName);

                string filePath;

                if (group == null)
                {
                    filePath = "~/Userfiles/";
                }
                else
                {
                    filePath = System.IO.Path.Combine("~/Userfiles/", Regex.Replace(group.Name.RemoveDiacritics(), @"[^A-Za-z0-9]+", "_") + "/");
                }

                var activityType = db.ActivityTypes.Find(file.ActivityTypeId);

                if (activityType != null)
                {
                    filePath = System.IO.Path.Combine(filePath, Regex.Replace(activityType.Name.RemoveDiacritics(), @"[^A-Za-z0-9]+", "_") + "/");
                }

                var serverFilePath = Server.MapPath(filePath);
                var serverFileName = fileId + "_" + Regex.Replace(fileName.RemoveDiacritics(), @"[^A-Za-z0-9.]+", "_");

                try
                {
                    // Determine whether the directory exists. 
                    if (!System.IO.Directory.Exists(serverFilePath))
                    {
                        System.IO.Directory.CreateDirectory(serverFilePath);
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, string.Format("Couldn't create folder for \"{0}\"", user.Group.Name));
                }

                try
                {
                    file.Upload.SaveAs(System.IO.Path.Combine(serverFilePath, serverFileName));
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, string.Format("Couldn't create file \"{0}\"", file.Upload.FileName));
                }

                var now = DateTime.Now;

                var newFile = new File()
                {
                    Id = fileId,
                    UserId = user.Id,
                    ActivityTypeId = file.ActivityTypeId,
                    IsShared = file.IsShared,
                    FileDate = now,
                    FileName = System.IO.Path.GetFileNameWithoutExtension(file.Upload.FileName),
                    FileExtension = System.IO.Path.GetExtension(file.Upload.FileName),
                    FilePath = filePath + serverFileName,
                    FileSize = file.Upload.ContentLength,
                    FileType = file.Upload.ContentType
                };

                //file.Id = fileId;
                //file.FileDate = now;
                //file.UserId = user.Id;
                //file.FileName = file.Upload.FileName;
                //file.FilePath = filePath + serverFileName;
                //file.FileSize = file.Upload.ContentLength;
                //file.FileType = file.Upload.ContentType;

                db.Files.Add(newFile);

                db.SaveChanges();

                if (!String.IsNullOrEmpty(Comment))
                {
                    using (var dbcontext = new ApplicationDbContext())
                    {
                        dbcontext.Comments.Add(new Comment() { Content = Comment, UserId = user.Id, FileId = fileId, TimeStamp = now });
                        dbcontext.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }

            var activityTypes =
                from a in db.Activities
                where a.GroupId == user.GroupId
                join at in db.ActivityTypes on a.ActivityTypeId equals at.Id
                orderby at.Name ascending
                select new SelectListItem
                {
                    Value = at.Id.ToString(),
                    Text = at.Name,
                    Selected = at.Id == file.ActivityTypeId
                };

            ViewBag.ActivityTypeId = activityTypes;
            return View(new FileViewModel() { ActivityTypeId = file.ActivityTypeId, Comment = Comment, IsShared = file.IsShared, Upload = file.Upload });
        }

        [Authorize]
        public ActionResult Download(Guid id)
        {
            File file = db.Files.Find(id);

            if (file == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Couldn't find file!");
            }

            try
            {
                var fs = System.IO.File.OpenRead(Server.MapPath(file.FilePath));
                return File(fs, file.FileType, file.FileName + file.FileExtension);
            }
            catch
            {
                throw new HttpException(404, "Couldn't find " + file.FileName + file.FileExtension);
            }
        }

        [Authorize]
        public ActionResult Thumbnail(Guid id, int size = 240)
        {
            File file = db.Files.Find(id);

            using (var stream = new System.IO.MemoryStream())
            {
                var image = Image.FromFile(Server.MapPath(file.FilePath));
                double width = image.Width;
                double height = image.Height;
                if (width >= height)
                {
                    height *= size / width;
                    width = size;
                }
                else
                {
                    width *= size / height;
                    height = size;
                }
                Image thumbnail = new Bitmap((int)width, (int)height);
                Graphics.FromImage(thumbnail).DrawImage(image, 0, 0, (int)width, (int)height);
                thumbnail.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }

        }

        // GET: Files/Edit/5
        [Authorize]
        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }

            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", file.ActivityTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", file.UserId);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,FileName,FileExtension,IsShared")] File file)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

                //using(var dbcontext = new ApplicationDbContext())
                //{
                //    if (!UserManager.IsInRole(user.Id, "admin") && user.Id != dbcontext.Files.Find(file.Id).UserId)
                //    {
                //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "This is not your file!");
                //    }
                //}

            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.Entry(file).Property(f => f.FileExtension).IsModified = false;
                db.Entry(file).Property(f => f.FilePath).IsModified = false;
                db.Entry(file).Property(f => f.FileSize).IsModified = false;
                db.Entry(file).Property(f => f.FileType).IsModified = false;
                db.Entry(file).Property(f => f.FileDate).IsModified = false;
                db.Entry(file).Property(f => f.ActivityTypeId).IsModified = false;
                db.Entry(file).Property(f => f.UserId).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityTypeId = new SelectList(db.Activities, "Id", "FirstName", file.ActivityTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", file.UserId);
            return View(file);
        }

        // GET: Files/Delete/5
        [Authorize]
        public ActionResult Delete(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(Guid id)
        {
            File file = db.Files.Find(id);
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (!UserManager.IsInRole(user.Id, "admin") && user.Id != file.UserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "This is not your file!");
            }

            try
            {
                System.IO.File.Delete(Server.MapPath(file.FilePath));
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Couldn't delete file for some reason...");
            }

            db.Files.Remove(file);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
