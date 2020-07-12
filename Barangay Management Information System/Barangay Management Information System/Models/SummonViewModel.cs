using Barangay_Management_Information_System.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Barangay_Management_Information_System.Models
{
    public class SummonViewModel
    {

        public Summon Summon { get; set; }
        public List<string> AssociatedResidentID { get; set; }

    }
}