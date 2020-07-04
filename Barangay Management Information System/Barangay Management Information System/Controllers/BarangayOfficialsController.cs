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

        [Authorize]
        public ActionResult OfficialsChart()
        {
            try
            {
                return View(entities.BarangayCaptains.ToList());
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
        public ActionResult ElectSKChairman()
        {
            try
            {
                // Check first if a barangay captain was elected already
                // returns the barangaycaptain object that is elected but does not have elected councilors
                List<BarangayCaptain> chairmanWithCouncilors = entities.BarangayCounselors.Select(m => m.BarangayCaptain).ToList();
                var ids = chairmanWithCouncilors.Select(m => m.ResidentId).ToList();

                BarangayCaptain chairmanWithNoCouncilors = entities.BarangayCaptains.Where(m => !ids.Contains(m.ResidentId)).FirstOrDefault();

                if (chairmanWithNoCouncilors != null)
                {
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "It appears that there is already an elected Barangay Chairman.";
                    return RedirectToAction("ElectCouncilors");
                }

                if (entities.OfficialTerms.Where(m => m.EndYear == null).FirstOrDefault() != null)
                {
                    return Content("Change End Year");
                }
                else
                {
                    OfficialTerm latestTerm = entities.OfficialTerms.OrderByDescending(m => m.EndYear).FirstOrDefault();
                    if (latestTerm.EndYear != DateTime.Now.Year)
                    {
                        return Content("It is still not time for election");
                    }
                }

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

                if (skChairman.SKCouncelors.ToList().Count > 0)
                {
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "It appears that the new SK Chairman already has councilors.";
                    return RedirectToAction("ElectCaptain");
                }

                // Verify first if skChairman has already elected councilors
                // update the view, display the current councilors

                TempData["SKChairmanFN"] = skChairman.ResidentsInformation.FirstName;
                TempData["SKChairmanMN"] = skChairman.ResidentsInformation.MiddleName;
                TempData["SKChairmanLN"] = skChairman.ResidentsInformation.LastName;

                int legalYear = DateTime.Now.Date.Year - 18;
                int day = DateTime.Now.Date.Day;
                int month = DateTime.Now.Month;
                var residents = entities.ResidentsInformations.Where(m => m.Birthday <= new DateTime(legalYear, month, day) && m.ResidentId != skChairman.ResidentId).ToList();

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
                
                if (residentIds.Length < 8 || residentIds.Length > 8) // .Length on a null object will raise an error
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
                // returns the barangaycaptain object that is elected but does not have elected councilors
                List<BarangayCaptain> chairmanWithCouncilors = entities.BarangayCounselors.Select(m => m.BarangayCaptain).ToList();
                var ids = chairmanWithCouncilors.Select(m => m.ResidentId).ToList();

                BarangayCaptain chairmanWithNoCouncilors = entities.BarangayCaptains.Where(m => !ids.Contains(m.ResidentId)).FirstOrDefault();

                if (chairmanWithNoCouncilors != null)
                {
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "It appears that there is already an elected Barangay Chairman.";
                    return RedirectToAction("ElectCouncilors");
                }

                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();
                SKChairman skChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();
                List<string> skcouncilorsIds = entities.SKCouncelors.Where(m => m.SKChairmanId == skChairman.SKChairmanId).Select(m => m.ResidentId).ToList();

                int legalYear = DateTime.Now.Date.Year - 18;
                int day = DateTime.Now.Date.Day;
                int month = DateTime.Now.Month;
                var residents = entities.ResidentsInformations
                    .Where(m => m.Birthday <= new DateTime(legalYear, month, day)
                    && m.ResidentId != skChairman.ResidentId
                    && !skcouncilorsIds.Contains(m.ResidentId))
                    .ToList();

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
        // A post method, but is only triggered by a button not a form
        public ActionResult ElectBrgyCaptain(string residentId)
        {
            try
            {
                OfficialTerm term = entities.OfficialTerms.OrderByDescending(m => m.EndYear).FirstOrDefault();
                //term.EndYear = DateTime.Now.Year; // update the end year of the last term
                //entities.Entry(term).State = System.Data.Entity.EntityState.Modified;
                //entities.SaveChanges();

                // Create new Official Term, term.EndYear will be the new StartYear. new EndYear will be null can be set later
                OfficialTerm newTerm = new OfficialTerm()
                {
                    OfficialTermId = KeyGenerator.GenerateId(),
                    StartYear = (int)DateTime.Now.Year
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

                ResidentsInformation name = entities.ResidentsInformations.Where(m => m.ResidentId == residentId).FirstOrDefault();

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = name.FirstName + " " + name.LastName + " was elected as Barangay Captain of Barangay Sinisian.";

                return RedirectToAction("ElectCouncilors");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to elect Barangay Captain, please try again later " + e.Message;
                return RedirectToAction("ElectCaptain");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ElectCouncilors()
        {
            try
            {
                // obtain the record that has a null end year, meaning that is the latest term.
                OfficialTerm officialTerm = entities.OfficialTerms.Where(m => m.EndYear == null).FirstOrDefault();
                BarangayCaptain chairman = entities.BarangayCaptains.Where(m => m.OfficialTermId == officialTerm.OfficialTermId).FirstOrDefault();

                if (entities.BarangayCounselors.Where(m=> m.CaptainId == chairman.CaptainId).ToList().Count > 0)
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "The current Chairman already has its councilors.";
                    return RedirectToAction("Index", "Dashboard");
                }   

                // Verify first if skChairman has already elected councilors
                // update the view, display the current councilors

                TempData["ChairmanFN"] = chairman.ResidentsInformation.FirstName;
                TempData["ChairmanMN"] = chairman.ResidentsInformation.MiddleName;
                TempData["ChairmanLN"] = chairman.ResidentsInformation.LastName;

                List<string> skCouncilorsId = chairman.SKChairman.SKCouncelors.Select(m=>m.ResidentId).ToList();
                
                int legalYear = DateTime.Now.Date.Year - 18;
                int day = DateTime.Now.Date.Day;
                int month = DateTime.Now.Month;
                var residents = entities.ResidentsInformations
                    .Where(m => m.Birthday <= new DateTime(legalYear, month, day) 
                    && m.ResidentId != chairman.ResidentId
                    && !skCouncilorsId.Contains(m.ResidentId)
                    && m.ResidentId != chairman.SKChairman.ResidentId)
                    .ToList();

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
        public ActionResult ElectCouncilors(string[] residentIds)
        {
            try
            {
                if (residentIds == null)
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "It appears that you have not selected any residents as Councilors.";
                    return RedirectToAction("ElectCouncilors");
                }

                if (residentIds.Length < 6 || residentIds.Length > 6) // .Length on a null object will raise an error
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "Elected councilors needs to be 6.";
                    return RedirectToAction("ElectCouncilors");
                }

                // obtain the record that has a null end year, meaning that is the latest term.
                OfficialTerm officialTerm = entities.OfficialTerms.Where(m => m.EndYear == null).FirstOrDefault();
                BarangayCaptain chairman = entities.BarangayCaptains.Where(m => m.OfficialTermId == officialTerm.OfficialTermId).FirstOrDefault();

                for (int i = 0; i < residentIds.Length; i++)
                {
                    entities.BarangayCounselors.Add(new BarangayCounselor()
                    {
                        BarangayCounselorId = KeyGenerator.GenerateId(residentIds[i]),
                        ResidentId = residentIds[i],
                        CaptainId = chairman.CaptainId
                    });
                    entities.SaveChanges();
                }

                List<string> ids = new List<string>();
                for (int i = 0; i < residentIds.Length; i++) { ids.Add(residentIds[i]); }
                TempData["Ids"] = ids;

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "New batch of Councilors have been elected.";

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later " + e.Message;
                return RedirectToAction("ElectCouncilors");
            }
        }
    }
}