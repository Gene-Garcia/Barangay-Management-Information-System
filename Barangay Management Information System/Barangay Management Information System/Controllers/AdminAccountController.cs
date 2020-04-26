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
            try
            {

                List<string> registeredResidentIds = entities.AspNetUsers.Select(m => m.ResidentId).ToList();

                return View(entities.ResidentsInformations.Where(m => !registeredResidentIds.Contains(m.ResidentId) ).ToList());
            }
            catch(Exception e)
            {
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
            TempData["user-profile-photo"] = DisplayPictureRetriever.GetDisplayPicture(User.Identity.GetUserId(), entities);
            try
            {

                return View(entities.AspNetUsers.ToList());
            }
            catch (Exception e)
            {
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
                return PartialView("_DisplayAccountTypes", entities.AspNetRoles.ToList());
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to create account for this resident, please try again later, " + e.Message.ToString();
                return PartialView("_DisplayAccountTypes");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddAccountType()
        {
            return PartialView("_AddAccountType");
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddAccountType(AspNetRole role)
        {
            try
            {
                role.Id = KeyGenerator.GenerateId( role.Name );
                entities.AspNetRoles.Add(role);
                entities.SaveChanges();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "Role with name " + role.Name + " is succesfully added.";
                
                return RedirectToAction("ModifyAccount");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to add account type, please try again later, " + e.Message.ToString();
                return RedirectToAction("ModifyAccount");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditAccountType(string roleId)
        {
            try
            {
                AspNetRole role = entities.AspNetRoles.Where(m => m.Id == roleId).FirstOrDefault();
                //return Content(role.Name + " name ");
                return PartialView("_EditAccountType", role);
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to find the account type, please try again later, " + e.Message.ToString();
                return RedirectToAction("ModifyAccount");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAccountType(AspNetRole role)
        {
            try
            {
                entities.Entry(role).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "Role updated.";

                return RedirectToAction("ModifyAccount");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to edit the account type, please try again later, " + e.Message.ToString();
                return RedirectToAction("ModifyAccount");
            }
        }

        [Authorize]
        public ActionResult DeleteAccountType(string roleId)
        {
            try
            {
                entities.AspNetRoles.Remove( entities.AspNetRoles.Where(m => m.Id == roleId).FirstOrDefault() );
                entities.SaveChanges();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "Role deleted.";

                return RedirectToAction("ModifyAccount");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to delete the account type, please try again later, " + e.Message.ToString();
                return RedirectToAction("ModifyAccount");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ModifyUserAccountType(string accountId)
        {
            try
            {
                TempData["roles"] = entities.AspNetRoles.ToList();

                AspNetUserRole aspNetUserRole = entities.AspNetUserRoles.Where(m => m.UserId == accountId).FirstOrDefault();
                return PartialView("_ModifyUserAccountType", aspNetUserRole);
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to configure account, please try again later, " + e.Message.ToString();
                return RedirectToAction("ModifyAccount");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult ModifyUserAccountType(AspNetUserRole userRole)
        {
            try
            {
                entities.Entry(userRole).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges(); 

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "Account configured and account type modified.";

                return RedirectToAction("ModifyAccount");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to configure account, please try again later, " + e.Message.ToString();
                return RedirectToAction("ModifyAccount");
            }
        }
    }
}