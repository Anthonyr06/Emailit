using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emailit.Models;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly ILogger<IndexModel> _Logger;
        private readonly IUserRepository _usersData;

        public IndexModel(ILogger<IndexModel> logger, IUserRepository userRepository, IActionContextAccessor actionContextAccessor)
        {
            _Logger = logger;
            _usersData = userRepository;
            _ActionContextAccessor = actionContextAccessor;
        }

        [BindProperty, StringLength(UserDataValidation.MaxNameLenght, ErrorMessage = "The {0} must have {1} characters as maximum"), Required(ErrorMessage = "Enter a {0}"),
            Display(Name = "Names")]
        public string Name { get; set; }

        [BindProperty, StringLength(UserDataValidation.MaxLastNameLenght, ErrorMessage = "The {0} must have {1} characters as maximum"),
            Display(Name = "Surnames"), Required(ErrorMessage = "Enter a {0}")]
        public string LastName { get; set; }

        [BindProperty, Display(Name = "Gender"), Required(ErrorMessage = "Select a {0}")]
        public Gender Gender { get; set; }

        [BindProperty, Display(Name = "Email"), DataType(DataType.EmailAddress), EmailAddress(ErrorMessage = "You must introduce a valid {0}."),
            StringLength(UserDataValidation.MaxEmailLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1}.", MinimumLength = UserDataValidation.MinEmailLenght),
            Required(ErrorMessage = "Enter a {0}.")]
        public string Email { get; set; }

        public string SuccessfulStatusMessage { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
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

            Name = user.Name;
            LastName = user.Lastname;
            Gender = user.Gender;
            Email = user.Email;

            return Page();
        }

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

            User user = await _usersData.GetAsync(Id);

            if (user == null)
            {
                _Logger.LogInformation($"User: {Email} doesn't exist anymore.");
                return RedirectToPage("./Index");
            }

            if (!Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) & await _usersData.CheckByEmailAsync(Email.ToLower()))
            {
                ModelState.AddModelError(string.Empty, $"There is already a user with email: {Email}.");
                return Page();
            }

            user.Name = Name;
            user.Lastname = LastName;
            user.Gender = Gender;
            user.Email = Email;


            User savedUser = null;

            try
            {
                savedUser = await _usersData.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _Logger.LogDebug($"An error occurred while editing user: {ex}");
                _Logger.LogError($"An error occurred while editing user ID: {user.UserId}.");
                ModelState.AddModelError(string.Empty, $"An error occurred when changing configuration. Try it again." +
                    $" If this continues, please contact the IT department");
            }

            if (savedUser == null)
            {
                return Page();
            }

            SuccessfulStatusMessage = "Data updated correctly!";
            return Page();
        }
    }
}