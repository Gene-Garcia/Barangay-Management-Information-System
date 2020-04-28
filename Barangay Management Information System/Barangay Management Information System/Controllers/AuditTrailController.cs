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
    public class AuditTrailController : Controller
    {

        private DBEntities entities = new DBEntities();

        // GET: AuditTrail
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            TempData["user-profile-photo"] = DisplayPictureRetriever.GetDisplayPicture(User.Identity.GetUserId(), entities);
            try
            {
                List<AuditTrail> audit = entities.AuditTrails.ToList();

                if (audit.Count < 1)
                {
                    TempData["alert-type"] = "alert-danger";
                    TempData["alert-header"] = "Error";
                    TempData["alert-msg"] = "No audit trail record found.";
                }

                return View( audit );
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Cannot get audit trails at this moment, please try again later.";
                return View();
            }
            
        }
    }
}