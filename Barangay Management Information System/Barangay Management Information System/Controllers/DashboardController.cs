using Barangay_Management_Information_System.Classess;
using Barangay_Management_Information_System.Models;
using Barangay_Management_Information_System.Models.Entity;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
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

            DashboardViewModel dbViewModel = new DashboardViewModel();

            int seniorYear = DateTime.Now.Year - 60;

            DateTime senior = new DateTime(seniorYear, DateTime.Now.Month, DateTime.Now.Day);

            dbViewModel.Residents = entities.ResidentsInformations.Count();
            dbViewModel.SeniorCitizens = entities.ResidentsInformations.Where(m => m.Birthday <= senior).Count();
            dbViewModel.Deceased = entities.ResidentsInformations.Where(m => m.Deceaseds.FirstOrDefault() != null).Count();
            dbViewModel.Voters = entities.ResidentsInformations.Where(m => m.Voters.FirstOrDefault() != null).Count();

            return View(dbViewModel);
        }

        [Authorize]
        public ContentResult GetDateResidentRecorded()
        {
            List<DashboardViewModel> dbvm = new List<DashboardViewModel>();

            List<DateTime> dates = entities.ResidentsInformations.GroupBy(m => m.DateRecorded).Select(m=>m.OrderBy(n=>n.DateRecorded).FirstOrDefault().DateRecorded.Value).ToList();

            foreach (var date in dates)
            {
                dbvm.Add( new DashboardViewModel() {
                    DateCitizenRecord = date.ToString("MMM dd, yyyy"),
                    DateCitizenRecordCount = entities.ResidentsInformations.Where(m => m.DateRecorded == date).Count()
                });
            }

            return Content(JsonConvert.SerializeObject(dbvm), "application/json");
        }
    }
}