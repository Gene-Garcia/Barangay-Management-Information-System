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
    }
}