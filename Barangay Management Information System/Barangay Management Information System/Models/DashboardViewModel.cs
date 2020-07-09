using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barangay_Management_Information_System.Models
{
    public class DashboardViewModel
    {

        public int Residents { get; set; }
        public int SeniorCitizens { get; set; }
        public int Deceased { get; set; }
        public int Voters { get; set; }


        public string DateCitizenRecord { get; set; }
        public int DateCitizenRecordCount { get; set; }
        public int DateCitizenRecordCountSinisianProper { get; set; }
        public int DateCitizenRecordCountSinisianNorth { get; set; }

    }
}