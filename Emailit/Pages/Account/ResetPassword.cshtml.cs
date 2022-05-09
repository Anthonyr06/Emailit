using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Emailit.Models;
using Emailit.Services;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly IUserRepository _userData;
        private readonly ITokenProvider _TokenProvider;
        private readonly ILogger<ResetPasswordModel> _Logger;

        [BindProperty]
        public PasswordViewModelCreate VmPwd { get; set; }
        public string TokenIssuer { get; set; }

        public ResetPasswordModel(IUserRepository userRepository, ITokenProvider tokenProvider, ILogger<ResetPasswordModel> logger)
        {
            _userData = userRepository;
            _TokenProvider = tokenProvider;
            _Logger = logger;
        }


        public async Task<IActionResult> OnGet(string i, string t)
        {

            if (string.IsNullOrEmpty(t))
            {
                return NotFound();
            }

            if (!_TokenProvider.ValidateForgotPasswordToken(t, i))
            {
                return RedirectToPagePermanent("./ResetPasswordExpired", new { i });
            }

            //Cleaning cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TokenIssuer = i;
            return Page();
        }


        public async Task<IActionResult> OnPost(string i, string t)
        {
            if (!_TokenProvider.ValidateForgotPasswordToken(t, i))
            {
                return RedirectToPagePermanent("./ResetPasswordExpired", new { i });
            }

            if (ModelState.IsValid)
            {
                var claims = _TokenProvider.GetClaims(t);

                if (!int.TryParse(claims.First(c => c.Type == "nameid").Value,
                    out int userId))
                {
                    return RedirectToPagePermanent("./ResetPasswordExpired", new { i });
                }

                string UserPwdTokenClaim = claims.First(c => c.Type == IClaimFactory.HashedUserPassword).Value;

                User user = await _userData.GetAsync(userId);

                if (user == null)
                {
                    _Logger.LogInformation($"Error ResetPassword, User [UserId: {user.UserId}] no longer exists.");
                    return RedirectToPagePermanent("./ResetPasswordExpired", new { i });
                }

                if (UserPwdTokenClaim != user.Password)
                {
                    //The token has already been used - the password was previously changed
                    return RedirectToPagePermanent("./ResetPasswordExpired", new { i });
                }

                var passwordHasher = new PasswordHasher<string>();
                var hashedPassword = passwordHasher.HashPassword(null, VmPwd.Password);

                user.Password = hashedPassword;
                user.MustChangePassword = false;


                User savedUser = null;

                try
                {
                    savedUser = await _userData.UpdateAsync(user);
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Error updating user password [UserId: {user.UserId}].");
                    _Logger.LogDebug($"Error details: {ex}");
                    ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
                }

                if (savedUser == null)
                {
                    TokenIssuer = i;
                    return Page();
                }



                _Logger.LogInformation($"The user [UserId: {user.UserId}] has successfully reset its password.");

                return RedirectToPagePermanent("./ResetPasswordConfirmation");

            }
            return Page();

        }


        //Workaround due to BUG in data annotation [Compare]//https://github.com/dotnet/aspnetcore/issues/4895
        //Behavior: Although the form was fine, the modelState.IsValid returned false
        public class PasswordViewModelCreate
        {
            [Display(Name = "Password"), DataType(DataType.Password), Required(ErrorMessage = "Enter a password"),
                StringLength(UserDataValidation.MaxPasswordLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1} characters.", 
                MinimumLength = UserDataValidation.MinPasswordLenght)]
            public string Password { get; set; }

            [Display(Name = "Confirm Password"), DataType(DataType.Password), Required(ErrorMessage = "You must confirm the password"),
               StringLength(UserDataValidation.MaxPasswordLenght, ErrorMessage = "The password must have a minimum of {2} and a maximum of {1} characters.", 
                MinimumLength = UserDataValidation.MinPasswordLenght)]
            [Compare(nameof(Password), ErrorMessage = "Password and password confirmation must be the same.")]
            public string ConfirmPassword { get; set; }
        }
    }
}