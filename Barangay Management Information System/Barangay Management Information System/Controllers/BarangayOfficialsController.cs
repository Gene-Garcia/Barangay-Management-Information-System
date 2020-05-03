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

                List<string> SK = entities.BarangayCaptains.Select(m => m.SKChairmanId).ToList();

                SKChairman sKChairman = entities.SKChairmen.Where(m => !SK.Contains(m.SKChairmanId)).FirstOrDefault();

                if (sKChairman != null)
                {
                    TempData["alert-type"] = "alert-info";
                    TempData["alert-header"] = "Information";
                    TempData["alert-msg"] = "It appears that you have already elected an SK Chairman.";
                    return View();
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

        [HttpPost]
        [Authorize]
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

                return RedirectToAction("ElectSKChairman");
                //return RedirectToAction("ElectSKCouncilors");
            }
            catch (Exception e)
            {
                TempData["alert-type"] = "alert-danger";
                TempData["alert-header"] = "Error";
                TempData["alert-msg"] = "Unable to elect SK Chairman, please try again later " + e.Message;
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

                TempData["SKChairman"] = skChairman;

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
                TempData["alert-msg"] = "Unable to elect SK Chairman, please try again later " + e.Message;
                return View();
            }
        }
    }
}