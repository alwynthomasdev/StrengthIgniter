using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StrengthIgniter.Core.Services;

namespace StrengthIgniter.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService _LoginService;
        private readonly IRegistrationService _RegistrationService;
        private readonly IPasswordResetService _PasswordResetService;
        private readonly IUserSecurityQuestionResetService _UserSecurityQuestionResetService;

        public AccountController(
            ILoginService loginService,
            IRegistrationService registrationService,
            IPasswordResetService passwordResetService,
            IUserSecurityQuestionResetService userSecurityQuestionResetService
        )
        {
            _LoginService = loginService;
            _RegistrationService = registrationService;
            _PasswordResetService = passwordResetService;
            _UserSecurityQuestionResetService = userSecurityQuestionResetService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

    }
}
