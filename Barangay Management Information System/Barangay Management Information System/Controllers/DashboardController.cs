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
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();
            string locationStorageId = entities.AccountAssets.Where(m => m.AccountId == userId).Select(m => m.LocationStorageId).FirstOrDefault();
            TempData["user-profile-photo"] = entities.LocationStorages.Where(m => m.LocationStorageId == locationStorageId).Select(m => m.Address).FirstOrDefault();

            return View();
        }

        [Authorize]
        public ActionResult Index2()
        {
            return View();
        }
    }
}