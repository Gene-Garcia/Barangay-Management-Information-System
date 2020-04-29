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
    public class BarangayClearanceController : Controller
    {
        private DBEntities entities = new DBEntities();
        // GET: BarangayClearance 
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);
            try
            {
                return View(entities.ResidentsInformations.ToList());
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "List of residents cannot be retrieved at this moment, please try again later " + e.Message;
                return View();
            }            
        }

        [Authorize]
        public ActionResult View(string residentId)
        {
            TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);
            try
            {
                return View(entities.ResidentsInformations.Where(m => m.ResidentId == residentId).FirstOrDefault());
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "List of residents cannot be retrieved at this moment, please try again later " + e.Message;
                return View();
            }
        }
    }
}