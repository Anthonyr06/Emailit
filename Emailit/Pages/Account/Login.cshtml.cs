using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Emailit.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IUserRepository _userData;
        private readonly IUserSessionRepository _UsersSessionsData;
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly ILogger<LoginModel> _Logger;
        private readonly IClaimFactory _ClaimFactory;
        private readonly ITokenProvider _TokenProvider;
        private readonly IConfiguration _Configuration;


        [BindProperty, Display(Name = "Email"), DataType(DataType.EmailAddress),
            EmailAddress(ErrorMessage = "You must introduce a valid {0}."), Required(ErrorMessage = "Enter a {0}."),
            StringLength(UserDataValidation.MaxEmailLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1} characters",
            MinimumLength = UserDataValidation.MinEmailLenght)]
        public string Email { get; set; }

        [BindProperty, Display(Name = "Password"), DataType(DataType.Password), Required(ErrorMessage = "Enter a password"),
            StringLength(UserDataValidation.MaxPasswordLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1} characters", 
            MinimumLength = UserDataValidation.MinPasswordLenght)]
        public string Password { get; set; }

        public LoginModel(IUserRepository userRepository, IUserSessionRepository userSession,
            IActionContextAccessor actionContextAccessor, ILogger<LoginModel> logger, IClaimFactory claimFactory,
            ITokenProvider tokenProvider, IConfiguration configuration)
        {
            _Logger = logger;
            _ClaimFactory = claimFactory;
            _userData = userRepository;
            _UsersSessionsData = userSession;
            _ActionContextAccessor = actionContextAccessor;
            _TokenProvider = tokenProvider;
            _Configuration = configuration;
        }
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPagePermanent("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(string ReturnUrl = null)
        {
            //clearing cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (ModelState.IsValid)
            {
                User user = await _userData.GetByEmailAsync(Conversions.ExtraSpaceRemover(Email.ToLower()));

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The information provided above is incorrect.");
                    return Page();
                }


                var passwordHasher = new PasswordHasher<string>();//verifying password
                bool isPwdCorrect = passwordHasher.VerifyHashedPassword(null, user.Password, Password) == PasswordVerificationResult.Success;

                UserSession Session = null;

                try
                {
                    Session = await _UsersSessionsData.AddAsync(GetSesionLogInfo(user.UserId, isPwdCorrect));

                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Error saving user session information [UserId: {user.UserId}].");
                    _Logger.LogDebug($"Error details: {ex}");
                    ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
                }

                if (Session == null)
                {
                    return Page();
                }

                int allowedAttempts = _Configuration.GetValue<int>("User:LoginFailedAttemps");

                if (!isPwdCorrect)
                {
                    if (user.LoginAttempts < allowedAttempts)
                    {
                        user.LoginAttempts += 1;

                        try
                        {
                            await _userData.UpdateAsync(user);
                        }
                        catch
                        {
                            _Logger.LogError($"{Session.IP} tried to login with user Id: {user.UserId}" +
                                $"AND IT HAS FAILED");
                        }

                    }

                    _Logger.LogInformation($"{Session.IP} tried to login with user Id: {user.UserId}");

                    ModelState.AddModelError(string.Empty, "The information provided above is incorrect.");
                    return Page();
                }


                if (user.LoginAttempts >= allowedAttempts)// the user reached the maximum number of attempts.
                {
                    ModelState.AddModelError(string.Empty, "Your user has been blocked. Communicate with the IT department.");
                    _Logger.LogInformation($"User [ID:{user.UserId}] tried to login and is blocked.");
                    return Page();
                }


                if (user.MustChangePassword)
                {
                    string issuer = "./Login";

                    var token = _TokenProvider.GenerateForgotPasswordToken(user.UserId, user.Password, issuer);

                    return RedirectToPage("./ResetPassword", new { i = issuer, t = token });
                }

                var NewClaimPrincipal = await _ClaimFactory.GetClaimPrincipal(user, Session);

                bool error = false;

                try
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, NewClaimPrincipal,
                        new AuthenticationProperties { IsPersistent = true });

                    if (user.LoginAttempts > 0) //reset count of failed attempts if the user succeeded in logging in.
                    {
                        user.LoginAttempts = 0;
                        await _userData.UpdateAsync(user);
                    }
                    _Logger.LogInformation($"The user {user.Email} has successfully logged in.");
                }
                catch (Exception ex)
                {
                    _Logger.LogDebug($"Error logging in user: {ex}");
                    _Logger.LogError($"Error logging in user: {Email}.");
                    ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
                    error = true;
                }



                if (!error)
                {
                    if (!Url.IsLocalUrl(ReturnUrl)) return RedirectToPagePermanent("/Index");
                    return RedirectPermanent(ReturnUrl);
                }
            }
            return Page();
        }


        /// <summary>
        /// Gets an object of type UserSession with the current session information of the entered user.
        /// </summary>
        /// <param name="userId">id of the user who just logged in</param>
        /// <param name="isPwdCorrect">if the user entered the wrong password mark as false.</param>
        /// <returns>An object of type UserSession.</returns>
        public UserSession GetSesionLogInfo(int userId, bool isPwdCorrect)
        {
            IPAddress ip = _ActionContextAccessor.ActionContext.HttpContext.Connection.RemoteIpAddress;

            var sesion = new UserSession
            {
                Successful = isPwdCorrect,
                UserId = userId,
                StartDate = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow,
                IP = ip
            };

            if (ip == null) _Logger.LogWarning($"Failed to get user session IP. [IdSesion: {sesion.UserSessionId}.]");

            return sesion;
        }
    }
}