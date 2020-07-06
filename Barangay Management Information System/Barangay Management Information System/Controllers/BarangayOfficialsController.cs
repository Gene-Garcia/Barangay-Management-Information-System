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
    public class BarangayOfficialsController : Controller
    {

        private DBEntities entities = new DBEntities();

        private const int BARANGAY_COUNCELOR_MAX_OFFICIAL = 6;
        private const int BARANGAY_SK_COUNCELOR_MAX_OFFICIAL = 8;

        [Authorize]
        public ActionResult OfficialsChart()
        {
            try
            {
                TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

                //BarangayCaptain chairman = entities.BarangayCaptains.Where(m => m.OfficialTerm.EndYear == null).FirstOrDefault();
                //var councelors = chairman.BarangayCounselors.ToList();
                //SKChairman sk = chairman.SKChairman;
                //var skcouncelors = sk.SKCouncelors.ToList();

                //foreach (var c in councelors)
                //{
                //    entities.BarangayCounselors.Remove(c);
                //    entities.SaveChanges();
                //}

                //foreach (var c in skcouncelors)
                //{
                //    entities.SKCouncelors.Remove(c);
                //    entities.SaveChanges();
                //}

                //entities.BarangayCaptains.Remove(chairman);
                //entities.SaveChanges();

                //entities.SKChairmen.Remove(sk);
                //entities.SaveChanges();

                return View(entities.BarangayCaptains.OrderByDescending(m=>m.OfficialTerm.StartYear).ToList());
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return View();
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult AssignSpecialOfficials()
        {
            try
            {
                TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

                OfficialTerm term = null;

                term = entities.OfficialTerms.Where(m => m.EndYear == null).FirstOrDefault();
                // if there is no null endyear record, then retrieve the highest end year. It is the latest record
                if (term == null) 
                {
                    term = entities.OfficialTerms.OrderByDescending(m => m.EndYear).FirstOrDefault();
                }

                BarangayCaptain chairman = entities.BarangayCaptains.Where(m => m.OfficialTermId == term.OfficialTermId).FirstOrDefault();

                string chairmanId = chairman.ResidentId;
                List<string> councelorIds = chairman.BarangayCounselors.Select(m => m.ResidentId).ToList();
                string skChairmanId = chairman.SKChairman.ResidentId;
                List<string> skCouncelorIds = chairman.SKChairman.SKCouncelors.Select(m => m.ResidentId).ToList();
                List<string> specialPos = chairman.AssignedOfficials.Select(m => m.ResidentId).ToList();

                TempData["Positions"] = entities.AssignedPositions.ToList();

                int legalYear = DateTime.Now.Date.Year - 18;
                int day = DateTime.Now.Date.Day;
                int month = DateTime.Now.Month;
                var residents = entities.ResidentsInformations
                    .Where(m => m.Birthday <= new DateTime(legalYear, month, day)
                    && m.ResidentId != chairmanId
                    && !councelorIds.Contains(m.ResidentId)
                    && m.ResidentId != skChairmanId
                    && !skCouncelorIds.Contains(m.ResidentId)
                    && !specialPos.Contains(m.ResidentId))
                    .ToList();

                return View(residents);

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
        public ActionResult AssignSpecialOfficials(string positionId, string[] residentIds)
        {
            try
            {
                if (positionId == null)
                {
                    return RedirectToAction("AssignSpecialOfficials");
                }

                if (positionId == "none")
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "Please select an assigned position.";
                    return RedirectToAction("AssignSpecialOfficials");
                }

                if (residentIds == null)
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "Must select atleast 1 resident for the position.";
                    return RedirectToAction("AssignSpecialOfficials");
                }

                //Audit Trail
                string userId = User.Identity.GetUserId();
                string posName = entities.AssignedPositions.Where(m => m.PositionId == positionId).Select(m => m.Name).FirstOrDefault();
                string chairman = entities.BarangayCaptains.OrderByDescending(m => m.OfficialTerm.StartYear).Select(m=>m.CaptainId).FirstOrDefault();

                foreach (var residentId in residentIds)
                {
                    entities.AssignedOfficials.Add(new AssignedOfficial()
                    {
                        AssignedOfficialId = KeyGenerator.GenerateId(residentId+chairman),
                        PositionId = positionId,
                        ResidentId = residentId,
                        CaptainId = chairman
                    });
                    entities.SaveChanges();

                    //Audit Trail
                    ResidentsInformation tempResInfo = entities.ResidentsInformations.Where(m => m.ResidentId == residentId).FirstOrDefault();
                    string tempName = tempResInfo.FirstName + " " + tempResInfo.LastName;
                    new AuditTrailer().Record("Assigned " + tempName + " for the position of " + posName, AuditTrailer.BARANGAY_OFFICIAL_TYPE, userId);
                }

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = "Successfully assigned resident(s) for " + posName + " position.";

                return RedirectToAction("OfficialsChart");
            }
            catch(Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Something went wrong, please try again later." + e.ToString();
                return RedirectToAction("OfficialsChart");
            }
        }

        /*
         *  The following ActionResult methods are used for election
         */

        [HttpGet]
        [Authorize]
        public ActionResult ElectSKChairman()
        {
            try
            {
                TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

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

                // Get the newly elected SK Chairman, The record that is not in the BarangayCaptainTable means its the new record
                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();
                SKChairman sKChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                if (sKChairman != null)
                {
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "It appears that you have already elected an SK Chairman.";
                    return RedirectToAction("ElectSKCouncilors");
                }

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
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return View();
            }
        }

        [Authorize]
        /* 
         * A post method, but is only triggered by a button not a form.
         */
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

                // Audit Trail
                string userId = User.Identity.GetUserId();
                new AuditTrailer().Record("Elected " + fullName + " as SK Chairman.", AuditTrailer.BARANGAY_OFFICIAL_TYPE, userId);

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = fullName + " was elected as SK Chairman of Barangay Sinisian.";

                return RedirectToAction("ElectSKCouncilors");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to elect SK Chairman, please try again.";
                return RedirectToAction("ElectSKChairman");
            }
        }
        
        [Authorize]
        [HttpGet]
        public ActionResult ElectSKCouncilors()
        {
            try
            {
                TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();
                SKChairman skChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                if(skChairman == null)
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "It appears that you have not yet elected an SK Chairman.";
                    return RedirectToAction("ElectSKChairman");
                }

                if (skChairman.SKCouncelors.ToList().Count >= BARANGAY_SK_COUNCELOR_MAX_OFFICIAL)
                {
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "It appears that the new SK Chairman already has councilors.";
                    return RedirectToAction("ElectCaptain");
                }

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
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return View();
            }
        }

        
        [Authorize]
        [HttpPost]
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

                // .Length on a null object will raise an error
                if (residentIds.Length < BARANGAY_SK_COUNCELOR_MAX_OFFICIAL || residentIds.Length > BARANGAY_SK_COUNCELOR_MAX_OFFICIAL)
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "Elected SK councilors needs to be " + BARANGAY_SK_COUNCELOR_MAX_OFFICIAL + ".";
                    return RedirectToAction("ElectSKCouncilors");
                }

                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();
                SKChairman skChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                // Audit Trail
                string userId = User.Identity.GetUserId();

                for (int i = 0; i < residentIds.Length; i++)
                {

                    entities.SKCouncelors.Add(new SKCouncelor()
                    {
                        SKCouncelorId = KeyGenerator.GenerateId(residentIds[i]),
                        ResidentId = residentIds[i],
                        SKChairmanId = skChairman.SKChairmanId
                    });
                    entities.SaveChanges();

                    // Audit Trail
                    string tempResId = residentIds[i];
                    ResidentsInformation resident = entities.ResidentsInformations.Where(m => m.ResidentId == tempResId).FirstOrDefault();
                    if (resident != null)
                    {
                        new AuditTrailer().Record("Elected " + resident.FirstName + " " + resident.LastName + " as SK Councelor.", AuditTrailer.BARANGAY_OFFICIAL_TYPE, userId);
                    }
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
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return View();
            }
        }
        
        [Authorize]
        [HttpGet]
        public ActionResult ElectCaptain()
        {
            try
            {
                TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

                // returns the barangaycaptain object that is elected but does not have elected councilors.
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

                // Helps in filtering results.
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
                TempData["alert-msg"] = "Something went wrong, please try again later.";
                return View();
            }
        }

        [Authorize]
        /* 
         * A post method, but is only triggered by a button not a form.
         */
        public ActionResult ElectBrgyCaptain(string residentId)
        {
            try
            {
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

                // Now, elect the barangay Chairman
                BarangayCaptain barangayCaptain = new BarangayCaptain()
                {
                    CaptainId = KeyGenerator.GenerateId(residentId),
                    OfficialTermId = newTerm.OfficialTermId,
                    ResidentId = residentId,
                    SKChairmanId = skChairman.SKChairmanId,
                    
                };
                entities.BarangayCaptains.Add(barangayCaptain);
                entities.SaveChanges();

                // Audit Trail
                string userId = User.Identity.GetUserId();
                ResidentsInformation tempResInfo = entities.ResidentsInformations.Where(m => m.ResidentId == residentId).FirstOrDefault();
                new AuditTrailer().Record("Elected " + tempResInfo.FirstName + " " + tempResInfo.LastName + " as Barangay Chairman.", AuditTrailer.BARANGAY_OFFICIAL_TYPE, userId);

                TempData["alert-type"] = "alert-success";
                TempData["alert-header"] = "Success";
                TempData["alert-msg"] = tempResInfo.FirstName + " " + tempResInfo.LastName + " was elected successfully as the Sinisian Barangay.";

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
                TempData["user-profile-photo"] = UserHelper.GetDisplayPicture(User.Identity.GetUserId(), entities);

                // obtain the record that has a null end year, meaning that is the latest term.
                OfficialTerm officialTerm = entities.OfficialTerms.Where(m => m.EndYear == null).FirstOrDefault();
                BarangayCaptain chairman = entities.BarangayCaptains.Where(m => m.OfficialTermId == officialTerm.OfficialTermId).FirstOrDefault();

                BarangayCounselor chairmanCouncelor = entities.BarangayCounselors.Where(m => m.CaptainId == chairman.CaptainId).FirstOrDefault();
                if (chairmanCouncelor != null)
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
                TempData["alert-msg"] = "Something went wrong, please try again later.";
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

                if (residentIds.Length < BARANGAY_COUNCELOR_MAX_OFFICIAL || residentIds.Length > BARANGAY_COUNCELOR_MAX_OFFICIAL)
                {
                    TempData["alert-type"] = "alert-warning";
                    TempData["alert-header"] = "Warning";
                    TempData["alert-msg"] = "Elected councilors needs to be " + BARANGAY_COUNCELOR_MAX_OFFICIAL + ".";
                    return RedirectToAction("ElectCouncilors");
                }

                // obtain the record that has a null end year, meaning that is the latest term.
                OfficialTerm officialTerm = entities.OfficialTerms.Where(m => m.EndYear == null).FirstOrDefault();
                BarangayCaptain chairman = entities.BarangayCaptains.Where(m => m.OfficialTermId == officialTerm.OfficialTermId).FirstOrDefault();

                //Audit Trail
                string userId = User.Identity.GetUserId();

                for (int i = 0; i < residentIds.Length; i++)
                {
                    entities.BarangayCounselors.Add(new BarangayCounselor()
                    {
                        BarangayCounselorId = KeyGenerator.GenerateId(residentIds[i]),
                        ResidentId = residentIds[i],
                        CaptainId = chairman.CaptainId
                    });
                    entities.SaveChanges();

                    //Audit Trail
                    string resId = residentIds[i];
                    ResidentsInformation tempResInformation = entities.ResidentsInformations.Where(m => m.ResidentId == resId).FirstOrDefault();
                    new AuditTrailer().Record("Elected " + tempResInformation.FirstName + " " + tempResInformation.LastName + " as Barangay Councelor.", AuditTrailer.BARANGAY_OFFICIAL_TYPE, userId);

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