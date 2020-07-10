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
                    DateCitizenRecordCount = entities.ResidentsInformations.Where(m => m.DateRecorded == date).Count(),
                    DateCitizenRecordCountSinisianNorth = entities.ResidentsInformations.Where(m => m.DateRecorded == date && m.ResidentsLocations.FirstOrDefault().HouseHoldAddress.Site.Name.ToLower() == "north sinisian").Count(),
                    DateCitizenRecordCountSinisianProper = entities.ResidentsInformations.Where(m => m.DateRecorded == date && m.ResidentsLocations.FirstOrDefault().HouseHoldAddress.Site.Name.ToLower() == "sinisian proper").Count()
                });
            }

            return Content(JsonConvert.SerializeObject(dbvm), "application/json");
        }

        [Authorize]
        public ContentResult GetAgeDistribution()
        {
            List<DashboardViewModel> dbvm = new List<DashboardViewModel>();
            // "Under 5 years", "5 to 17 years", "18 to 24 years", "25 to 44 years", "45 to 64 years", "65 years and over"

            DateTime below5 = new DateTime(DateTime.Now.Year - 4, DateTime.Now.Month, DateTime.Now.Day);

            dbvm.Add(new DashboardViewModel()
            {
                MaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "male" && m.Birthday >= below5).Count(),
                FemaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "female" && m.Birthday >= below5).Count()
            });

            DateTime five = new DateTime(DateTime.Now.Year - 5, DateTime.Now.Month, DateTime.Now.Day);
            DateTime seventeen = new DateTime(DateTime.Now.Year - 17, DateTime.Now.Month, DateTime.Now.Day);

            dbvm.Add(new DashboardViewModel()
            {
                MaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "male" && m.Birthday <= five && m.Birthday >= seventeen).Count(),
                FemaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "female" && m.Birthday <= five && m.Birthday >= seventeen).Count()
            });

            DateTime eighteen = new DateTime(DateTime.Now.Year - 18, DateTime.Now.Month, DateTime.Now.Day);
            DateTime twentyfour = new DateTime(DateTime.Now.Year - 24, DateTime.Now.Month, DateTime.Now.Day);

            dbvm.Add(new DashboardViewModel()
            {
                MaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "male" && m.Birthday <= eighteen && m.Birthday >= twentyfour).Count(),
                FemaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "female" && m.Birthday <= eighteen && m.Birthday >= twentyfour).Count()
            });

            DateTime twentyfive = new DateTime(DateTime.Now.Year - 25, DateTime.Now.Month, DateTime.Now.Day);
            DateTime fourtyfour = new DateTime(DateTime.Now.Year - 44, DateTime.Now.Month, DateTime.Now.Day);

            dbvm.Add(new DashboardViewModel()
            {
                MaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "male" && m.Birthday <= twentyfive && m.Birthday >= fourtyfour).Count(),
                FemaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "female" && m.Birthday <= twentyfive && m.Birthday >= fourtyfour).Count()
            });


            DateTime fourtyfive = new DateTime(DateTime.Now.Year - 45, DateTime.Now.Month, DateTime.Now.Day);
            DateTime sixtyfour = new DateTime(DateTime.Now.Year - 64, DateTime.Now.Month, DateTime.Now.Day);

            dbvm.Add(new DashboardViewModel()
            {
                MaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "male" && m.Birthday <= fourtyfive && m.Birthday >= sixtyfour).Count(),
                FemaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "female" && m.Birthday <= fourtyfive && m.Birthday >= sixtyfour).Count()
            });


            DateTime over65 = new DateTime(DateTime.Now.Year - 65, DateTime.Now.Month, DateTime.Now.Day);

            dbvm.Add(new DashboardViewModel()
            {
                MaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "male" && m.Birthday <= over65).Count(),
                FemaleAgeDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "female" && m.Birthday <= over65).Count()
            });


            return Content(JsonConvert.SerializeObject(dbvm), "application/json");
        }

        [Authorize]
        public ContentResult GetSexDistribution()
        {
            DashboardViewModel dbvm = new DashboardViewModel();

            dbvm.MaleSexDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "male").Count();
            dbvm.FemaleSexDistribution = entities.ResidentsInformations.Where(m => m.Sex.ToLower() == "female").Count();

            return Content(JsonConvert.SerializeObject(dbvm), "application/json");

        }

        [Authorize]
        public ContentResult GetSitioDistribution()
        {
            DashboardViewModel dbvm = new DashboardViewModel();

            dbvm.NSDistribution = entities.ResidentsInformations.Where(m => m.ResidentsLocations.FirstOrDefault().HouseHoldAddress.Site.Name.ToLower() == "north sinisian").Count();
            dbvm.SPDistribution = entities.ResidentsInformations.Where(m => m.ResidentsLocations.FirstOrDefault().HouseHoldAddress.Site.Name.ToLower() == "sinisian proper").Count();
            dbvm.UndefinedDistribution = entities.ResidentsInformations.Where(m => m.ResidentsLocations.FirstOrDefault().HouseHoldAddress.Site.Name.ToLower() != "sinisian proper" && m.ResidentsLocations.FirstOrDefault().HouseHoldAddress.Site.Name.ToLower() != "north sinisian").Count();

            return Content(JsonConvert.SerializeObject(dbvm), "application/json");

        }        

        [Authorize]
        public ContentResult GetDateAccountsCreated()
        {
            List<DashboardViewModel> dbvm = new List<DashboardViewModel>();

            List<DateTime> dates = entities.AspNetUsers.OrderBy(m=>m.DateCreated).GroupBy(m => m.DateCreated).Select(m => m.FirstOrDefault().DateCreated.Value).ToList();

            foreach (var date in dates)
            {
                dbvm.Add( new DashboardViewModel() { 
                    DateAccountCreated = date.ToString("MMM dd, yyyy"),
                    DateAccountCreatedCount = entities.AspNetUsers.Where(m => m.DateCreated == date).Count()
                });
            }

            return Content(JsonConvert.SerializeObject(dbvm), "application/json");

        }

        [Authorize]
        public ContentResult GetAccountTypesDistribution()
        {
            List<DashboardViewModel> dbvm = new List<DashboardViewModel>();

            List<AspNetRole> roles = entities.AspNetRoles.OrderBy(m => m.Name).ToList();

            foreach(var role in roles)
            {
                dbvm.Add( new DashboardViewModel() {
                    AccountTypes = role.Name,
                    AccountTypeUsersCount = entities.AspNetUsers.Where(m=>m.AspNetUserRoles.FirstOrDefault().AspNetRole.Name == role.Name).Count()
                });
            }

            return Content(JsonConvert.SerializeObject(dbvm), "application/json");
        }
    }
}