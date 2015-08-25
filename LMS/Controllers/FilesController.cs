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
        public ActionResult Index(int? id, int? activityTypeId)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var files = db.Files.Include(f => f.User);
            if (activityTypeId != null)
            {
                files = files.Where(f => f.ActivityTypeId == activityTypeId);
            }

            //var adminRole = (from r in db.Roles where r.Name.Contains("admin") select r).FirstOrDefault();
            //var admins = db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(adminRole.Id)).ToList();

            if (!User.IsInRole("admin"))
            {
                id = UserManager.FindById(User.Identity.GetUserId()).GroupId; // non-admins can only see their own group
                var groupActivityTypeIds = user.Group.Activities.Select(a => a.ActivityTypeId).ToList();
                if (activityTypeId != null)
                {
                    groupActivityTypeIds = groupActivityTypeIds.Where(at => at.Equals(activityTypeId)).ToList();
                }
                files = files.Where(f => groupActivityTypeIds.Contains(f.ActivityTypeId) && f.IsShared && (f.GroupId == user.GroupId || f.UserId == user.Id)); //only see your groups shared or your own files
                ViewBag.GroupActivityTypeIds = groupActivityTypeIds;
            }
            else if (id != null)
            {
                ViewBag.GroupActivityTypeIds = db.Groups.Find(id).Activities.Select(a => a.ActivityTypeId).ToList();
                files = files.Where(f => f.GroupId == id);
            }
            else
            {
                ViewBag.GroupActivityTypeIds = db.Activities.Select(a => a.ActivityTypeId)/*.Where(a => a.Equals(activityTypeId ?? a))*/.ToList();
            }

            ViewBag.GroupId = id;
            ViewBag.ActivitiesCount = db.Activities.Where(a => a.GroupId == id).Count();

            return View(files.OrderBy(f => f.FileDate).ToList());
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
        public ActionResult Upload(int? id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            //var activities = db.ActivityTypes.Where(at => at.Activities == user.Group.Activities);

            int? groupId;

            if (User.IsInRole("admin") && id != null)
            {
                groupId = id;
            }
            else
            {
                groupId = user.GroupId;
            }

            ViewBag.ActivityTypeId =
            from a in db.Activities
            where a.GroupId == groupId
            join at in db.ActivityTypes on a.ActivityTypeId equals at.Id
            orderby at.Name ascending
            select new SelectListItem
            {
                Value = at.Id.ToString(),
                Text = at.Name
            };
            ViewBag.GroupId = groupId;

            return View();
        }

        [HttpPost]
        [Authorize]
        //public ActionResult Upload(HttpPostedFileBase Upload, bool isShared, int activityTypeId, string content)
        public ActionResult Upload([Bind(Include = "Upload,IsShared,GroupId,ActivityTypeId")] FileViewModel file, string Comment, int? id)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Couldn't find user!");
            }
            var user = UserManager.FindById(userId);
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
                    GroupId = User.IsInRole("admin") ? file.GroupId : group.Id,
                    IsShared = file.IsShared,
                    FileDate = now,
                    FileName = System.IO.Path.GetFileNameWithoutExtension(file.Upload.FileName),
                    FileExtension = System.IO.Path.GetExtension(file.Upload.FileName),
                    FilePath = filePath + serverFileName,
                    FileSize = file.Upload.ContentLength,
                    FileType = file.Upload.ContentType
                };

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
                return RedirectToAction("Index", new { id = newFile.GroupId });
            }

            var activityTypes =
                from a in db.Activities
                where a.GroupId == file.GroupId
                join at in db.ActivityTypes on a.ActivityTypeId equals at.Id
                orderby at.Name ascending
                select new SelectListItem
                {
                    Value = at.Id.ToString(),
                    Text = at.Name,
                    Selected = at.Id == file.ActivityTypeId
                };

            ViewBag.GroupId = file.GroupId;
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

            var groupActivityTypeIds = db.Activities.Where(a => a.GroupId == file.GroupId).Select(g => g.ActivityTypeId).ToList();
            ViewBag.HasGroupActivity = groupActivityTypeIds.Contains(file.ActivityTypeId);
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes.Where(a => groupActivityTypeIds.Contains(a.Id)), "Id", "Name", file.ActivityTypeId); 
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", file.UserId);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,FileName,FileExtension,GroupId,ActivityTypeId,IsShared")] File file)
        {
             if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.Entry(file).Property(f => f.FileExtension).IsModified = false;
                db.Entry(file).Property(f => f.FilePath).IsModified = false;
                db.Entry(file).Property(f => f.FileSize).IsModified = false;
                db.Entry(file).Property(f => f.FileType).IsModified = false;
                db.Entry(file).Property(f => f.FileDate).IsModified = false;
                db.Entry(file).Property(f => f.UserId).IsModified = false;
                db.Entry(file).Property(f => f.GroupId).IsModified = false;
                //db.Entry(file).Property(f => f.ActivityType).IsModified = file.ActivityTypeId != null;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = file.GroupId });
            }

            ViewBag.HasGroupActivity = db.Activities.Where(a => a.GroupId == file.GroupId).Select(a => a.ActivityTypeId).Contains(file.ActivityTypeId);
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
