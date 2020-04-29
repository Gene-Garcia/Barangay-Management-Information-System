using Barangay_Management_Information_System.Classess;
using Barangay_Management_Information_System.Models.Entity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Barangay_Management_Information_System.Controllers
{
    
    public class DashboardController : Controller
    {
        private DBEntities entities = new DBEntities();

        // GET: Dashboard
        [Authorize]
        public ActionResult Index()
        {
            TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

            return View();
        }
    }
}