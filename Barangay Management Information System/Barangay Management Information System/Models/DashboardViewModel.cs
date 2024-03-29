﻿using System;
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

        // Chart 1, Line
        public string DateCitizenRecord { get; set; }
        public int DateCitizenRecordCount { get; set; }
        public int DateCitizenRecordCountSinisianProper { get; set; }
        public int DateCitizenRecordCountSinisianNorth { get; set; }

        // Chart 2, Bar
        public int MaleAgeDistribution { get; set; }
        public int FemaleAgeDistribution { get; set; }

        // Chart 3, Doughnut
        public int MaleSexDistribution { get; set; }
        public int FemaleSexDistribution { get; set; }

        // Chart 4, Pie
        public int NSDistribution { get; set; }
        public int SPDistribution { get; set; }
        public int UndefinedDistribution { get; set; }

        // Chart 5, Line
        public string DateAccountCreated { get; set; }
        public int DateAccountCreatedCount { get; set; }

        // Chart 6, Pie
        public string AccountTypes { get; set; }
        public int AccountTypeUsersCount { get; set; }

        // Chart 7, Line
        public string ChairmanNames { get; set; }
        public int SettledReportsCounts { get; set; }
        public int UnsettledReportsCounts { get; set; }
    }
}