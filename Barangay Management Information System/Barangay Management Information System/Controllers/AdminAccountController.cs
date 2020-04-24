using Barangay_Management_Information_System.Classess;
using Barangay_Management_Information_System.Models;
using Barangay_Management_Information_System.Models.Entity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Barangay_Management_Information_System.Controllers
{
    public class AdminAccountController : Controller
    {
        private DBEntities entities = new DBEntities();
        // GET: AdminAccount
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateAccount()
        {
            TempData["user-profile-photo"] = DisplayPictureRetriever.GetDisplayPicture(User.Identity.GetUserId(), entities);
            TempData["alert-present"] = "0";
            try
            {

                List<string> registeredResidentIds = entities.AspNetUsers.Select(m => m.ResidentId).ToList();

                return View(entities.ResidentsInformations.Where(m => !registeredResidentIds.Contains(m.ResidentId) ).ToList());
            }
            catch(Exception e)
            {
                TempData["alert-present"] = "1";
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Information cannot be retrieved at this time, please try again later";
                return View();
            }
        }

        [Authorize]
        public ActionResult RegisterResident(string residentId)
        {
            try
            {
                TempData["alert-present"] = "0";

                TempData["Roles"] = entities.AspNetRoles.ToList();
                ResidentsInformation resident = entities.ResidentsInformations.Where(m => m.ResidentId == residentId).FirstOrDefault();
                RegisterViewModel register = new RegisterViewModel();
                register.Username = resident.FirstName + "_" + resident.LastName;
                register.Password = "Qwertypad360!";
                register.ResidentId = residentId;

                return PartialView("_RegisterResident", register);
            }
            catch (Exception e)
            {
                TempData["alert-present"] = "1";
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to create account for this resident, please try again later, " + e.Message.ToString();
                return PartialView("_RegisterResident");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ModifyAccount()
        {
            try
            {
                TempData["alert-present"] = "0";

                return View(entities.AspNetUsers.ToList());
            }
            catch (Exception e)
            {
                TempData["alert-present"] = "1";
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to retrieved registered accounts, please try again later, " + e.Message.ToString();
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult DisplayAccountTypes()
        {
            try
            {
                TempData["alert-present"] = "0";
                return PartialView("_DisplayAccountTypes", entities.AspNetRoles.ToList());
            }
            catch (Exception e)
            {
                TempData["alert-present"] = "1";
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to create account for this resident, please try again later, " + e.Message.ToString();
                return PartialView("_DisplayAccountTypes");
            }
        }
    }
}