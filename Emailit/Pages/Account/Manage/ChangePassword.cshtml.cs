using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Emailit.Models;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly ILogger<ChangePasswordModel> _Logger;
        private readonly IUserRepository _usersData;

        public ChangePasswordModel(ILogger<ChangePasswordModel> logger, IUserRepository userRepository, IActionContextAccessor actionContextAccessor)
        {
            _Logger = logger;
            _usersData = userRepository;
            _ActionContextAccessor = actionContextAccessor;
        }

        [BindProperty, Display(Name = "Old password"), DataType(DataType.Password), Required(ErrorMessage = "Enter a password"),
            StringLength(UserDataValidation.MaxPasswordLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1}.", MinimumLength = UserDataValidation.MinPasswordLenght)]
        public string OldPassword { get; set; }

        [BindProperty]
        public PasswordViewModelCreate VmPwd { get; set; }
        public string SuccessfulStatusMessage { get; private set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Getting UserId of the current user, from the cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int Id))
            {
                _Logger.LogError($"An error occurred while obtaining the user ID.");
                return Page();
            }

            var user = await _usersData.GetAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            var passwordHasher = new PasswordHasher<string>();

            bool isPwdCorrect = passwordHasher
                .VerifyHashedPassword(null, user.Password, OldPassword) == PasswordVerificationResult.Success;

            if (!isPwdCorrect)
            {
                ModelState.AddModelError(string.Empty, $"Previous password not correct.");
                return Page();
            }

            var NewPassword = passwordHasher.HashPassword(null, VmPwd.Password);
            user.Password = NewPassword;

            User savedUser = null;

            try
            {
                savedUser = await _usersData.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _Logger.LogDebug($"An error occurred while editing user: {ex}");
                _Logger.LogError($"An error occurred while editing user ID: {user.UserId}.");
                ModelState.AddModelError(string.Empty, $"An error has occurred when changing the password. Try it again." +
                    $" If this continues, please communicate with the IT department");
            }

            if (savedUser == null)
            {
                return Page();
            }

            SuccessfulStatusMessage = "Data updated correctly!";

            return Page();
        }


        //Workaround due to BUG in data annotation [Compare]//https://github.com/dotnet/aspnetcore/issues/4895
        //Behavior: Although the form was fine, the modelState.IsValid returned false
        public class PasswordViewModelCreate
        {
            [Display(Name = "Password"), DataType(DataType.Password), Required(ErrorMessage = "Enter a password"),
                StringLength(UserDataValidation.MaxPasswordLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1}.", MinimumLength = UserDataValidation.MinPasswordLenght)]
            public string Password { get; set; }

            [Display(Name = "Confirm Password"), DataType(DataType.Password), Required(ErrorMessage = "You must confirm the password"),
               StringLength(UserDataValidation.MaxPasswordLenght, ErrorMessage = "The password must have a minimum of {2} and a maximum of {1}.", MinimumLength = UserDataValidation.MinPasswordLenght)]
            [Compare(nameof(Password), ErrorMessage = "Password and password confirmation must be the same.")]
            public string ConfirmPassword { get; set; }
        }
    }
}