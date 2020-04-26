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
    public class PersonalAccountController : Controller
    {

        private DBEntities entities = new DBEntities();
        // GET: PersonalAccount
        public ActionResult Index()
        {
            TempData["user-profile-photo"] = DisplayPictureRetriever.GetDisplayPicture(User.Identity.GetUserId(), entities);
            try
            {
                string id = User.Identity.GetUserId();
                return View(entities.AspNetUsers.Where(m=>m.Id == id).FirstOrDefault());
            }
            catch (Exception e)
            {
                return View();
            }

            
        }

        [HttpGet]
        [Authorize]
        public ActionResult UploadDisplayPicture()
        {
            return PartialView("_UploadDisplayPicture");
        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadDisplayPicture(HttpPostedFileBase displayPicture)
        {
            try
            {
                string userId = User.Identity.GetUserId();
                new ImageUploader().SaveImage(displayPicture, userId);

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "success";
                TempData["alert-msg"] = "Display picture updated.";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Picture cannot be uploaded, please try again later " + e.Message;
                return RedirectToAction("Index");
            }
        }
    }
}