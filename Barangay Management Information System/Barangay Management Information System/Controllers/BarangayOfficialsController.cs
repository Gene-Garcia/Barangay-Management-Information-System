using Barangay_Management_Information_System.Classess;
using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Barangay_Management_Information_System.Controllers
{
    public class BarangayOfficialsController : Controller
    {

        private DBEntities entities = new DBEntities();

        // GET: BarangayOfficials
        public ActionResult Index()
        {
            return View(entities.ResidentsInformations.ToList());
        }

        public ActionResult View(string[] residentIds)
        {

            List<string> ids = new List<string>();

            for (int i = 0; i < residentIds.Length; i++)
            {
                ids.Add(residentIds[i]);
            }

            TempData["Ids"] = ids;
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ElectSKChairman()
        {
            try
            {

                // Check first if latest term is the same as the current year
                    // Check first if there is a null EndYear, if there is notify and redirect to change the EndYear

                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();

                // if SKChairman table has an SKChairman that is not referenced to BarangayCaptains table
                // that record, SKChairman, is just recently elected. The records show that an entry in SKChairman still not have its BarangayCaptain.
                // It might occur that in electing the SK the user might have stopped.This way there would be no multiple entries that does not have reference to BarangayCaptain
                SKChairman sKChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                if (sKChairman != null)
                {
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "It appears that you have already elected an SK Chairman.";
                    return RedirectToAction("ElectSKCouncilors");
                }
                else
                {
                    int legalYear = DateTime.Now.Date.Year - 18;
                    int day = DateTime.Now.Date.Day;
                    int month = DateTime.Now.Month;
                    var residents = entities.ResidentsInformations.Where(m => m.Birthday <= new DateTime(legalYear, month, day)).ToList();

                    return View(residents);
                }

            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later " + e.Message;
                return View();
            }
        }

        [Authorize]
        // A post method, but is only triggered by a button not a form
        public ActionResult ElectChairmanSK(string residentId, string fullName)
        {
            try
            {

                SKChairman chairman = new SKChairman() {
                    SKChairmanId = KeyGenerator.GenerateId(fullName),
                    ResidentId = residentId
                };

                entities.SKChairmen.Add(chairman);
                entities.SaveChanges();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = fullName + " was elected as SK Chairman of Barangay Sinisian.";

                return RedirectToAction("ElectSKCouncilors");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to elect SK Chairman, please try again. " + e.Message;
                return RedirectToAction("ElectSKChairman");
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ElectSKCouncilors()
        {
            try
            {
                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();

                SKChairman skChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                if(skChairman == null)
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "It appears that you have not yet elected an SK Chairman.";
                    return RedirectToAction("ElectSKChairman");
                }

                // Verify first if skChairman has already elected councilors
                // update the view, display the current councilors

                TempData["SKChairmanFN"] = skChairman.ResidentsInformation.FirstName;
                TempData["SKChairmanMN"] = skChairman.ResidentsInformation.MiddleName;
                TempData["SKChairmanLN"] = skChairman.ResidentsInformation.LastName;

                int legalYear = DateTime.Now.Date.Year - 18;
                int day = DateTime.Now.Date.Day;
                int month = DateTime.Now.Month;
                var residents = entities.ResidentsInformations.Where(m => m.Birthday <= new DateTime(legalYear, month, day)).ToList();

                return View(residents);
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later " + e.Message;
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult ElectSkCouncilors(string[] residentIds)
        {
            try
            {
                if(residentIds == null) { 
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "It appears that you have not selected any residents as SK Councilors.";
                    return RedirectToAction("ElectSKCouncilors");
                }
                
                if (residentIds.Length <= 8) // .Length on a null object will raise an error
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "Elected SK councilors needs to be 8.";
                    return RedirectToAction("ElectSKCouncilors");
                }

                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();
                SKChairman skChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                for (int i = 0; i < residentIds.Length; i++)
                {
                    entities.SKCouncelors.Add(new SKCouncelor() 
                    {
                        SKCouncelorId = KeyGenerator.GenerateId(residentIds[i]),
                        ResidentId = residentIds[i],
                        SKChairmanId = skChairman.SKChairmanId
                    });
                    entities.SaveChanges();
                }

                List<string> ids = new List<string>();
                for (int i = 0; i < residentIds.Length; i++) { ids.Add(residentIds[i]); }
                TempData["Ids"] = ids;

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "New batch of SK Councilors have been elected.";

                return RedirectToAction("ElectCaptain");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later " + e.Message;
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ElectCaptain()
        {
            try
            {
                List<ResidentsInformation> residents = entities.ResidentsInformations.ToList();
                return View(residents);
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later " + e.Message;
                return View();
            }
        }

        [Authorize]
        public ActionResult ElectBrgyCaptain(string residentId)
        {
            try
            {
                OfficialTerm term = entities.OfficialTerms.OrderByDescending(m => m.EndYear).FirstOrDefault();

                // Create new Official Term, term.EndYear will be the new StartYear. new EndYear will be null can be set later
                OfficialTerm newTerm = new OfficialTerm()
                {
                    OfficialTermId = KeyGenerator.GenerateId(),
                    StartYear = (int)term.EndYear
                };
                entities.OfficialTerms.Add( newTerm );
                entities.SaveChanges();

                // Get the newly elected SK Chairman, The record that is not in the BarangayCaptainTable means its the new record
                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();
                SKChairman skChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                // Now, elect the barangay Captain
                BarangayCaptain barangayCaptain = new BarangayCaptain()
                {
                    CaptainId = KeyGenerator.GenerateId(residentId),
                    OfficialTermId = newTerm.OfficialTermId,
                    ResidentId = residentId,
                    SKChairmanId = skChairman.SKChairmanId,
                    
                };
                entities.BarangayCaptains.Add(barangayCaptain);
                entities.SaveChanges();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "New Barangay Captain of Barangay Sinisian was elected.";

                return RedirectToAction("ElectCaptain");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to elect Barangay Captain, please try again later " + e.Message;
                return RedirectToAction("ElectCaptain");
            }
        }
    }
}