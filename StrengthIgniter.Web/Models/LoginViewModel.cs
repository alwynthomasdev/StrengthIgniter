using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web.Models
{
    public class LoginViewModel
    {
        [Display(Name ="Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name ="Password")]
        public string Password { get; set; }
        [Display(Name ="Remeber Me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public bool LoginAttemptFailed { get; set; }
    }
}
