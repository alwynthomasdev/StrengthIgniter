using Microsoft.AspNetCore.Mvc.Rendering;
using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace StrengthIgniter.Web.Models
{
    public class RegistrationViewModel
    {

        [Required]
        [Display(Name="Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name="Password")]
        public string Password { get; set; }

        [Display(Name= "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords Must Match")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<SelectListItem> SecurityQuestions { get; set; }
       
        [Required]
        [Display(Name="Security Question")]
        public string SecurityQuestion { get; set; }
        [Required]
        [Display(Name="Security Question Answer")]
        public string SecurityQuestionAnswer { get; set; }

    }
}
