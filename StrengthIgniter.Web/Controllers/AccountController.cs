using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeFluff.Extensions.IEnumerable;
using CodeFluff.Extensions.Object;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services;
using StrengthIgniter.Web.Models;

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

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if(ModelState.IsValid)
            {
                LoginResponse response = _LoginService.Login(new LoginRequest
                {
                    EmailAddress = vm.EmailAddress,
                    Password = vm.Password
                });

                if(response.ResponseType > 0)
                {
                    await SignInAsync(response);
                    return LocalRedirect(vm.ReturnUrl);
                }
                else vm.LoginAttemptFailed = true;
            }
            return View(vm);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            RegistrationViewModel vm = new RegistrationViewModel();
            vm.SecurityQuestions = GetSecretQuestionSelectList();

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegistrationViewModel vm)
        {
            if(ModelState.IsValid)
            {
                RegistrationResponseType response = _RegistrationService.Register(new RegistrationModel
                {
                    Name = vm.Name,
                    EmailAddress = vm.EmailAddress,
                    Password = vm.Password,
                    SecurityQuestionAnswers = new List<SecurityQuestionAnswerModel> { 
                        new SecurityQuestionAnswerModel { QuestionText = vm.SecurityQuestion, Answer = vm.SecurityQuestionAnswer } 
                    }//TODO: multiple security questiosn
                });
                return View("RegisterComplete");
            }
            vm.SecurityQuestions = GetSecretQuestionSelectList();//re-get this
            return View(vm);
        }

        #region Private methods

        private IEnumerable<Claim> CreateClaims(LoginResponse loginResponse)
        {
            return new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, loginResponse.UserReference.ToString()),
                new Claim(ClaimTypes.Name, loginResponse.Name),
                new Claim(ClaimTypes.Email, loginResponse.EmailAddress),
                new Claim(ClaimTypes.Role, loginResponse.UserType.GetDescription())
            };
        }

        private async Task SignInAsync(LoginResponse loginResponse, bool isPersistent = false)
        {
            IEnumerable<Claim> claims = CreateClaims(loginResponse);
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties() { IsPersistent = isPersistent } 
            );
        }

        private IEnumerable<SelectListItem> GetSecretQuestionSelectList()
        {
            var secretQuestions = _RegistrationService.GetSecretQuestions();
            if(secretQuestions.HasItems())
            {
                List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Text = "", Value = "", Selected = true } };
                items.AddRange(secretQuestions.Select(q => new SelectListItem { Text = q.QuestionText, Value = q.QuestionText }));
                return items;
            }
            return new SelectListItem[] { };//empty list
        }

        #endregion

    }
}
