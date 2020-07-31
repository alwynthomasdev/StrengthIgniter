using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeFluff.Extensions.IEnumerable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm)
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
                    //TODO: create auth cookie
                    //TODO: redirect
                }
                else vm.LoginAttemptFailed = true;
            }
            return View(vm);
        }

        public IActionResult Register()
        {
            RegistrationViewModel vm = new RegistrationViewModel();
            vm.SecurityQuestions = GetSecretQuestionSelectList();

            return View(vm);
        }

        [HttpPost]
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
