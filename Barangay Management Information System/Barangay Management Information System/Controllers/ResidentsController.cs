using Barangay_Management_Information_System.Classess;
using Barangay_Management_Information_System.Models;
using Barangay_Management_Information_System.Models.Entity;
using Microsoft.AspNet.Identity;
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
                TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

                List<ResidentsInformation> residents = null;
                if (deceased == "deceased")
                {
                    // deceased
                    residents = entities.ResidentsInformations.Where(m => m.Deceaseds.FirstOrDefault() != null).ToList();
                    TempData["isDeceased"] = true;
                }
                else
                {
                    residents = entities.ResidentsInformations.Where(m => m.Deceaseds.FirstOrDefault() == null).ToList();
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
        public ActionResult DeceaseResident(string residentID)
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
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "User cannot be found.";
                    return RedirectToAction("Index");
                }

                int age = CalculateAge(resident.Birthday);

                Deceased deceased = new Deceased()
                {
                    DeceasedId = KeyGenerator.GenerateId(resident.FirstName + resident.LastName + resident.Birthday),
                    ResidentId = resident.ResidentId,
                    DeathDate = DateTime.Now.Date,
                    Age = age
                };

                entities.Deceaseds.Add(deceased);
                entities.SaveChanges();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = resident.FirstName + " " + resident.LastName + " is set as a deceased Sinisian resident.";
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return RedirectToAction("Index");
            }
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
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
                    AddResident(model);

                    return RedirectToAction("Index");
                }
                else // Update
                {
                    UpdateResident(model);

                    return RedirectToAction("Index");
                }

            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return View("Index");
            }
        }

        private void AddResident(RegisterResidentViewModel model)
        {
            try
            {
                ResidentsInformation resident = new ResidentsInformation()
                {
                    ResidentId = KeyGenerator.GenerateId(model.FirstName + model.LastName),
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    Sex = model.Sex,
                    Birthday = model.Birthdate
                };
                entities.ResidentsInformations.Add(resident);
                entities.SaveChanges();

                HouseHoldAddress houseHoldAddress = new HouseHoldAddress()
                {
                    AddressId = KeyGenerator.GenerateId(model.Address),
                    Address = model.Address,
                    SiteId = model.SiteID
                };
                entities.HouseHoldAddresses.Add(houseHoldAddress);
                entities.SaveChanges();

                ResidentsLocation residentsLocation = new ResidentsLocation()
                {
                    ResidentLocationId = KeyGenerator.GenerateId(model.ResidentID + model.Address),
                    ResidentId = resident.ResidentId,
                    AddressId = houseHoldAddress.AddressId
                };
                entities.ResidentsLocations.Add(residentsLocation);
                entities.SaveChanges();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = model.FirstName + " " + model.LastName + " was successfully recorded as a resident of Sinisian.";
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later.";
            }            
        }

        private void UpdateResident(RegisterResidentViewModel model)
        {
            try
            {
                ResidentsInformation resident = entities.ResidentsInformations.Where(m => m.ResidentId == model.ResidentID).FirstOrDefault();
                resident.FirstName = model.FirstName;
                resident.LastName = model.LastName;
                resident.MiddleName = model.MiddleName;
                resident.Birthday = model.Birthdate;
                resident.Sex = model.Sex;

                entities.Entry(resident).State = System.Data.Entity.EntityState.Modified;
                entities.SaveChanges();

                if (resident.ResidentsLocations.FirstOrDefault() == null)
                {
                    HouseHoldAddress houseHoldAddress = new HouseHoldAddress()
                    {
                        AddressId = KeyGenerator.GenerateId(model.Address),
                        Address = model.Address,
                        SiteId = model.SiteID
                    };
                    entities.HouseHoldAddresses.Add(houseHoldAddress);
                    entities.SaveChanges();

                    ResidentsLocation residentsLocation = new ResidentsLocation()
                    {
                        ResidentLocationId = KeyGenerator.GenerateId(model.ResidentID + model.Address),
                        ResidentId = model.ResidentID,
                        AddressId = houseHoldAddress.AddressId
                    };
                    entities.ResidentsLocations.Add(residentsLocation);
                    entities.SaveChanges();

                }
                else
                {
                    HouseHoldAddress houseHoldAddress = resident.ResidentsLocations.FirstOrDefault().HouseHoldAddress;
                    houseHoldAddress.Address = model.Address;
                    houseHoldAddress.SiteId = model.SiteID;
                    entities.Entry(resident).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = model.FirstName + " " + model.LastName +" record was successfully updated.";
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later.";
            }
        }
    }
}