using StrengthIgniter.Web.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web.Models
{
    public class PasswordResetRequestViewModel
    {
        [Display(Name="Email Address")]
        [Required]
        public string EmailAddress { get; set; }
    }

    public class PasswordResetViewModel
    {
        public Guid PasswordResetToken { get; set; }
        public Guid SecurityQuestionAnswerReference { get; set; }
        public string SecurityQuestion { get; set; }
        public bool PasswordResetAttemptFailed { get; set; }

        [Required]
        [Display(Name = "Password")]
        [ConfigurableMinLength("CoreServiceConfiguration:RegistrationServiceConfig:PassswordMinLength")]
        [ConfigurableMaxLength("CoreServiceConfiguration:RegistrationServiceConfig:PasswordMaxLength")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords Must Match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Answer")]
        public string SecurityAnswer { get; set; }
    }

}
