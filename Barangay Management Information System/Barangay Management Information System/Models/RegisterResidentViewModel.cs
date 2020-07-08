using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Barangay_Management_Information_System.Models
{
    public class RegisterResidentViewModel
    {

        [Required]
        public string ResidentID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Sex")]
        public string Sex { get; set; }

        [Required]
        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthdate { get; set; }

        [Required]
        [Display(Name = "Site Location")]
        public string SiteID { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

    }
}