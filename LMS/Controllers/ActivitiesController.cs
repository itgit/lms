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
using Microsoft.AspNet.Identity.Owin;

namespace LMS.Controllers
{
    public class ActivitiesController : Controller
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

        // GET: Activities
        [Authorize]
        public ActionResult Index(int? id)
        {
            if (!User.IsInRole("admin"))
            {
                id = UserManager.FindById(User.Identity.GetUserId()).GroupId;
            }
            else if (id == null)
            {
                if (db.Activities != null)
                {
                    ViewBag.groups = db.Groups.Where(g => g.Activities.Count() > 0).OrderBy(g => g.Name).ToList();
                    return View(db.Activities.OrderBy(a => (int)a.Day * 1440 + a.StartTime.Hours * 60 + a.StartTime.Minutes).ToList());
                }
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "No activities found");
            }

            var group = db.Groups.Find(id);
            if (group == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Group not found");
            }
            ViewBag.groupid = id;
            ViewBag.groups = new List<Group>() { db.Groups.Find(id) };
            return View(group.Activities.OrderBy(a => (int)a.Day * 1440 + a.StartTime.Hours * 60 + a.StartTime.Minutes).ToList());
        }

        // GET: Activities/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Can't create activity without group");
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Group not found");
            }

            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name");
            ViewBag.groupname = group.Name;
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "Id,ActivityTypeId,Day,StartTime,EndTime,GroupId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activity.ActivityTypeId);
                if (activity.StartTime >= activity.EndTime)
                {
                    ViewBag.errormessage = "An activity cannot end before it starts!";
                    return View(activity);  // Illegal activity
                }
                foreach (var otheractivity in db.Activities.Where(a => a.GroupId == activity.GroupId).Include(a => a.ActivityType))
                {
                    if (activity.Day == otheractivity.Day)
                    {
                        if ((activity.StartTime >= otheractivity.StartTime && activity.StartTime < otheractivity.EndTime) ||
                            (activity.EndTime > otheractivity.StartTime && activity.EndTime <= otheractivity.EndTime) ||
                            (activity.StartTime <= otheractivity.StartTime && activity.EndTime >= otheractivity.EndTime))
                        {
                            ViewBag.errormessage = "This time period is already taken by another activity (" + otheractivity.ActivityType.Name + " " + otheractivity.StartTime + "-" + otheractivity.EndTime + ")!";
                            return View(activity);  // Illegal activity
                        }
                    }
                }
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = activity.GroupId });
            }

            return View(activity);
        }

        // GET: Activities/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activity.ActivityTypeId);

            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "Id,ActivityTypeId,Day,StartTime,EndTime,GroupId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activity.ActivityTypeId);
                if (activity.StartTime >= activity.EndTime)
                {
                    ViewBag.errormessage = "An activity cannot end before it starts!";
                    return View(activity);  // Illegal activity
                }
                foreach (var otheractivity in db.Activities.Where(a => a.GroupId == activity.GroupId && a.Id != activity.Id).Include(a => a.ActivityType))
                {
                    if (activity.Day == otheractivity.Day)
                    {
                        if ((activity.StartTime >= otheractivity.StartTime && activity.StartTime < otheractivity.EndTime) ||
                            (activity.EndTime > otheractivity.StartTime && activity.EndTime <= otheractivity.EndTime) ||
                            (activity.StartTime <= otheractivity.StartTime && activity.EndTime >= otheractivity.EndTime))
                        {
                            ViewBag.errormessage = "This time period is already taken by another activity (" + otheractivity.ActivityType.Name + " " + otheractivity.StartTime + "-" + otheractivity.EndTime + ")!";
                            return View(activity);  // Illegal activity
                        }
                    }
                }
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = activity.GroupId });
            }
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activity.ActivityTypeId);

            return View(activity);
        }

        // GET: Activities/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = activity.GroupId });
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
