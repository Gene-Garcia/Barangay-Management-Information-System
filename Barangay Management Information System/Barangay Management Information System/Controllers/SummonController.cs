using Barangay_Management_Information_System.Classess;
using Barangay_Management_Information_System.Models;
using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Barangay_Management_Information_System.Controllers
{
    public class SummonController : Controller
    {
        private DBEntities entities = new DBEntities();

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                SummonViewModel viewModel = new SummonViewModel();

                List<ResidentsInformation> residents = entities.ResidentsInformations.Where(m=>m.Deceaseds.FirstOrDefault() == null).ToList();
                viewModel.CheckBoxes = new SummonViewModel.CheckBoxModel[residents.Count()];
                int ctr = 0;
                foreach(var resident in residents)
                {
                    var temp = new SummonViewModel.CheckBoxModel()
                    {
                        IsSelected = false,
                        ResidentId = resident.ResidentId,
                        FullName = resident.FirstName + " " + resident.LastName
                    };
                    viewModel.CheckBoxes[ctr++] = temp;
                }

                viewModel.CheckBoxes = viewModel.CheckBoxes.OrderBy(m=> m.FullName).ToArray();

                TempData["Status"] = entities.SummonStatus.ToList();

                return View(viewModel);

            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later." + e.ToString();
                return View();
            }
        }
        
        [Authorize]
        [HttpPost]
        public ActionResult Index(SummonViewModel model)
        {
            try
            {
                model.Summon.DateReported = DateTime.Now.Date;
                model.Summon.SummonId = KeyGenerator.GenerateId(model.Summon.ReportDescription);
                entities.Summons.Add(model.Summon);
                entities.SaveChanges();

                foreach (var resident in model.CheckBoxes)
                {
                    if (resident.IsSelected == true)
                    {
                        entities.SummonInvolvedResidents.Add(new SummonInvolvedResident()
                        {
                            ResidentId = resident.ResidentId,
                            SummonId = model.Summon.SummonId,
                            InvolvedId = KeyGenerator.GenerateId(resident.FullName)
                        });
                        entities.SaveChanges();
                    }
                }

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "A Summon report was succesfully created with an ID of " + model.Summon.SummonId + ". Please copy this id as reference.";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later." + e.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}