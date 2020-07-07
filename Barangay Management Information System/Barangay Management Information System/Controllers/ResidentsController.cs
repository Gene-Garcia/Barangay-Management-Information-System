using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Barangay_Management_Information_System.Controllers
{
    public class ResidentsController : Controller
    {
        private DBEntities entities = new DBEntities();

        // GET: Residents
        public ActionResult Index(string deceased)
        {

            try
            {
                List<ResidentsInformation> residents = null;
                if (deceased == "deceased")
                {
                    // deceased
                    residents = entities.ResidentsInformations.Where(m => m.Deceaseds.FirstOrDefault() != null).ToList();
                }
                else
                {
                    residents = entities.ResidentsInformations.ToList();
                }

                return View(residents);
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return View();
            }

            return Content(deceased);
        }
    }
}