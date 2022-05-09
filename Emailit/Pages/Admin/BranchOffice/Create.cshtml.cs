using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Admin.BranchOffice
{
    [HasPermission(Permissions.WriteBranchOffices)]
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _Logger;
        private readonly IUserRepository _usersData;
        private readonly IBranchOfficeRepository _branchOfficesData;
        private readonly IAuthorizationService _AuthorizationService;

        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(BranchOfficeDataValidation.MaxNameLenght, 
            ErrorMessage = "The {0} must have {1} characters maximum")]
        public string Name { get; set; }

        [BindProperty, Display(Name = "Manager ID Card"), Required(ErrorMessage = "Enter office manager ID card."),
        StringLength(13, ErrorMessage = "The {0} is incomplete."), RegularExpression(@"^\d{3}-\d{7}-\d$", ErrorMessage = "The {0} is not valid.")]
        public string ManagerIdCard { get; set; }

        [BindProperty, Display(Name = "Active office.")]
        public bool Active { get; set; }

        public CreateModel(IBranchOfficeRepository branchOfficeReporitory, ILogger<CreateModel> logger, IUserRepository userRepository, IAuthorizationService authorizationService)
        {
            _Logger = logger;
            _usersData = userRepository;
            _branchOfficesData = branchOfficeReporitory;
            _AuthorizationService = authorizationService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Models.User user = await _usersData.GetByIdCardAsync(ManagerIdCard.Replace("-", ""));

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "It was not possible to find a user with this id card. Try it again.");
                return Page();
            }

            if ((await _branchOfficesData.GetAllEvenDeactivatedAsync())
                .Any(e => e.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "The name is not available.");
                _Logger.LogInformation($"Name: {Conversions.ExtraSpaceRemover(Name)} already exist in the DB.");
                return Page();
            }

            //Verify if the ID entered belongs to a person in charge of another BranchOffice
            if ((await _branchOfficesData.GetAllEvenDeactivatedAsync()).Any(e => e.ManagerId == user.UserId))
            {
                ModelState.AddModelError(string.Empty, "The introduced id card belongs to a manager from another office. Try it again.");
                return Page();
            }

            var office = new Models.BranchOffice
            {
                Name = Name,
                ManagerId = user.UserId,
                Active = !await CanDeactivateBranchOffices() || Active
            };


            Models.BranchOffice savedOffice = null;

            try
            {
                savedOffice = await _branchOfficesData.AddAsync(office);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Error creating BranchOffice.");
                _Logger.LogDebug($"Error details: {ex}");
                ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
            }

            if (savedOffice == null)
            {
                return Page();
            }

            _Logger.LogInformation($" BranchOffice [ID: {office.BranchOfficeId}] has been successfully created.");

            return RedirectToPage("./Index");
        }

        private async Task<bool> CanDeactivateBranchOffices()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateBranchOffices.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }
    }
}