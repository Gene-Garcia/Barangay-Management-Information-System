using Barangay_Management_Information_System.Models;
using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;

namespace Barangay_Management_Information_System.Controllers
{
    public class ResidentsController : Controller
    {
        private DBEntities entities = new DBEntities();

        [Authorize]
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

        [Authorize]
        public ActionResult ViewResident(string residentID)
        {
            try
            {
                if (residentID == null)
                {
                    return RedirectToAction("Index");
                }

                ResidentsInformation resident = entities.ResidentsInformations.Where(m => m.ResidentId == residentID).FirstOrDefault();
                
                if (resident == null)
                {
                    return RedirectToAction("Index");
                }

                return PartialView("_ViewResident", resident);
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddUpdateResident(string residentID)
        {
            try
            {

                TempData["Sites"] = entities.Sites.ToList();

                // add
                if (residentID == null || residentID == "")
                {
                    return PartialView("_AddUpdateResident", new RegisterResidentViewModel());
                }
                else // update
                {

                    ResidentsInformation resident = entities.ResidentsInformations.Where(m => m.ResidentId == residentID).FirstOrDefault();

                    RegisterResidentViewModel viewModel = new RegisterResidentViewModel();

                    viewModel.ResidentID = resident.ResidentId;
                    viewModel.LastName = resident.LastName;
                    viewModel.FirstName = resident.FirstName;
                    viewModel.MiddleName = resident.MiddleName;
                    viewModel.Sex = resident.Sex;
                    viewModel.Birthdate = resident.Birthday;

                    if (resident.ResidentsLocations.FirstOrDefault() != null)
                    {
                        viewModel.Address = resident.ResidentsLocations.FirstOrDefault().HouseHoldAddress.Address;
                        viewModel.SiteID = resident.ResidentsLocations.FirstOrDefault().HouseHoldAddress.SiteId;
                    }         

                    return PartialView("_AddUpdateResident", viewModel);
                }

            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later." + e.ToString();
                return View("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddUpdateResident(RegisterResidentViewModel model)
        {
            try
            {
                // Add
                if (model.ResidentID == null)
                {
                    return Content("Add");
                }
                else // Update
                {
                    return Content("Update" + model.Birthdate);
                }

            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later." + e.ToString();
                return View("Index");
            }
        }
    }
}