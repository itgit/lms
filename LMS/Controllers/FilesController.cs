﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using Microsoft.AspNet.Identity;
using ExtensionMethods;
using System.Text.RegularExpressions;

namespace LMS.Controllers
{
    public class FilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Files
        public ActionResult Index()
        {
            var files = db.Files.Include(f => f.Group).Include(f => f.User);
            return View(files.ToList());
        }

        // GET: Files/Details/5
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

        //// GET: Files/Create
        //public ActionResult Create()
        //{
        //    ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name");
        //    ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
        //    return View();
        //}

        //// POST: Files/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,FileName,FilePath,UserId,GroupId")] File file)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Files.Add(file);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name", file.GroupId);
        //    ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", file.UserId);
        //    return View(file);
        //}

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase Upload)
        {
            if (ModelState.IsValid)
            {
                if (Upload != null && Upload.ContentLength > 0)
                {
                    var fileId = Guid.NewGuid();
                    var fileName = System.IO.Path.GetFileName(Upload.FileName);
                    var userId = User.Identity.GetUserId();
                    if (userId == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Couldn't find user!");
                    }
                    var group = db.Users.Find(userId).Group;
                    if (group == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Couldn't find group!");
                    }
                    var filePath = System.IO.Path.Combine("~/Userfiles/", Regex.Replace(group.Name.RemoveDiacritics(), @"[^A-Za-z0-9]+", "_") + "/");
                    var serverFilePath = Server.MapPath(filePath);
                    var serverFileName = fileId + "_" +Regex.Replace(fileName.RemoveDiacritics(), @"[^A-Za-z0-9.]+", "_");
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
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, string.Format("Couldn't create folder for \"{0}\"", group.Name));
                    }
                    try
                    {
                        Upload.SaveAs(System.IO.Path.Combine(serverFilePath, serverFileName));
                    }
                    catch
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, string.Format("Couldn't create file \"{0}\"", Upload.FileName));
                    }
                    db.Files.Add(new LMS.Models.File() { Id = fileId, FileName = fileName, FilePath = filePath + serverFileName, FileSize = Upload.ContentLength, FileType = Upload.ContentType, FileDate = DateTime.Now, UserId = User.Identity.GetUserId(), GroupId = group.Id });
                    db.SaveChanges();
                }
                else
                {
                    return View(Upload);
                }
            }

            return RedirectToAction("Index", new { message = "File uploaded successfully!" });
        }

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
                return File(fs, file.FileType, file.FileName);
            }
            catch
            {
                throw new HttpException(404, "Couldn't find " + file.FileName);
            }
        }

        // GET: Files/Edit/5
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
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name", file.GroupId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", file.UserId);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FileName,IsShared")] File file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.Entry(file).Property(f => f.FilePath).IsModified = false;
                db.Entry(file).Property(f => f.FileSize).IsModified = false;
                db.Entry(file).Property(f => f.FileType).IsModified = false;
                db.Entry(file).Property(f => f.FileDate).IsModified = false;
                db.Entry(file).Property(f => f.GroupId).IsModified = false;
                db.Entry(file).Property(f => f.UserId).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Name", file.GroupId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", file.UserId);
            return View(file);
        }

        // GET: Files/Delete/5
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
        public ActionResult DeleteConfirmed(Guid id)
        {
            File file = db.Files.Find(id);
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