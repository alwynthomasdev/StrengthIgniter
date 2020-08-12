using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    //TODO: rename invalid token view !!!

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

        //public IActionResult Index()
        //{
        //    return View();
        //}

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
                    await SignInAsync(response, vm.RememberMe);
                    return LocalRedirect(vm.ReturnUrl ?? "/");
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
                    }//TODO: multiple security questions
                });
                return View("RegisterComplete");
            }
            vm.SecurityQuestions = GetSecretQuestionSelectList();//re-get this
            return View(vm);
        }

        [AllowAnonymous]
        [Route("account/validate/{token}")]
        public IActionResult Validate(Guid token)
        {
            RegistrationValidationResponseType response = _RegistrationService.ValidateRegistration(token);
            switch(response)
            {
                case RegistrationValidationResponseType.Success:
                    return View();
                case RegistrationValidationResponseType.NotFound:
                case RegistrationValidationResponseType.RegistrationTokenExpired:
                case RegistrationValidationResponseType.ValidationAttemptFailed:
                default:
                    return View("InvalidToken", new InvalidTokenViewModel
                    {
                        PageTitle = "Account Validation",
                        Message = "Sorry, your account has not been validated."
                    });
            }
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ForgotPassword(PasswordResetRequestViewModel vm)
        {
            _PasswordResetService.RequestPasswordReset(vm.EmailAddress);
            return View("ForgotPasswordSent");
        }

        [AllowAnonymous]
        [Route("account/passwordreset/{token}")]
        public IActionResult PasswordReset(Guid token)
        {
            PasswordResetResponse response = _PasswordResetService.GetSecurityQuestion(token);
            switch(response.ResponseType)
            {
                case PasswordResetResponseType.PasswordResetTokenValid:
                    return View(new PasswordResetViewModel
                    {
                        PasswordResetToken = token,
                        SecurityQuestionAnswerReference = response.UserSecurityQuestionAnswerReference,
                        SecurityQuestion = response.QuestionText
                    });
                case PasswordResetResponseType.PasswordResetAttemptsMaxed:
                    return View("InvalidToken", new InvalidTokenViewModel
                    {
                        PageTitle = "Password Reset",
                        Message = "Sorry, you have reached the maximum allowed attempts to reset your password."
                    });
                case PasswordResetResponseType.PasswordResetTokenExpired:
                case PasswordResetResponseType.PasswordResetTokenInvalid:
                default:
                    return View("InvalidToken", new InvalidTokenViewModel
                    {
                        PageTitle = "Password Reset",
                        Message = "Sorry, your password reset URL appears to be invalid."
                    });
            }
        }

        [AllowAnonymous]
        [Route("account/passwordreset/{token}")]
        [HttpPost]
        public IActionResult PasswordReset(PasswordResetViewModel vm)
        {
            if(ModelState.IsValid)
            {
                PasswordResetResponse response = _PasswordResetService.ResetPassword(new ResetPasswordRequest
                {
                    PasswordResetToken = vm.PasswordResetToken,
                    NewPassword = vm.Password,
                    UserSecurityQuestionAnswerReference = vm.SecurityQuestionAnswerReference,
                    SecurityQuestionAnswer = vm.SecurityAnswer
                });
                switch(response.ResponseType)
                {
                    case PasswordResetResponseType.PasswordReset:
                        return View("InvalidToken", new InvalidTokenViewModel
                        {
                            PageTitle = "Password Reset Complete",
                            Message = "Your password has been reset."
                        });
                    case PasswordResetResponseType.PasswordResetAttemptsMaxed:
                        return View("InvalidToken", new InvalidTokenViewModel
                        {
                            PageTitle = "Password Reset",
                            Message = "Sorry, you have reached the maximum allowed attempts to reset your password."
                        });
                    case PasswordResetResponseType.PasswordResetTokenExpired:
                    case PasswordResetResponseType.PasswordResetTokenInvalid:
                        return View("InvalidToken", new InvalidTokenViewModel
                        {
                            PageTitle = "Password Reset",
                            Message = "Sorry, your password reset URL appears to be invalid."
                        });
                    case PasswordResetResponseType.PasswordResetAttemptFailed:
                    default:
                        vm.PasswordResetAttemptFailed = true;
                        break;
                }
            }
            return View(vm);
        }

        #region Private methods

        private IEnumerable<Claim> CreateClaims(LoginResponse loginResponse)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, loginResponse.UserReference.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, loginResponse.Name));
            claims.Add(new Claim(ClaimTypes.Email, loginResponse.EmailAddress));
            claims.Add(new Claim(ClaimTypes.Role, loginResponse.UserType.GetDescription()));

            return claims;
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
