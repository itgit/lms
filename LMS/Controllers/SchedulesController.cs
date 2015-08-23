using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LMS.Controllers
{
    public class SchedulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Schedules
        [Authorize]
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = db.Groups.Find(id);
            if (group == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Group not found");
            }

            if (group.Activities.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "No activities found for this group ");
            }

            var first = 24;
            var last = 0;

            foreach (var activity in group.Activities)
            {
                if (activity.StartTime.Hours < first)
                {
                    first = activity.StartTime.Hours;
                }
                if (activity.EndTime.Hours >= last)
                {
                    last = (int)Math.Ceiling((double)activity.EndTime.TotalMinutes / 60);
                }
            }

            ViewBag.last = last;
            ViewBag.first = first;
            ViewBag.groupid = id;
            ViewBag.groupname = group.Name;

            return View(group.Activities);
        }
    }
}