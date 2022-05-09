using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.BranchOffice
{
    [HasPermission(Permissions.WriteBranchOffices)]
    public class EditModel : PageModel
    {
        private readonly ILogger<CreateModel> _Logger;
        private readonly IUserRepository _usersData;
        private readonly IBranchOfficeRepository _branchOfficesData;
        private readonly IAuthorizationService _AuthorizationService;

        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(BranchOfficeDataValidation.MaxNameLenght, ErrorMessage = "The {0} must have {1} maximum characters")]
        public string Name { get; set; }

        [BindProperty, Display(Name = "Manager ID Card"), Required(ErrorMessage = "Enter office manager ID card."),
        StringLength(13, ErrorMessage = "The {0} is incomplete."), RegularExpression(@"^\d{3}-\d{7}-\d$", ErrorMessage = "The {0} is invalid.")]
        public string ManagerIdCard { get; set; }

        [BindProperty, Display(Name = "Active office.")]
        public bool Active { get; set; }

        public EditModel(IBranchOfficeRepository branchOfficeReporitory, ILogger<CreateModel> logger, IUserRepository userRepository, IAuthorizationService authorizationService)
        {
            _Logger = logger;
            _usersData = userRepository;
            _branchOfficesData = branchOfficeReporitory;
            _AuthorizationService = authorizationService;
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.BranchOffice office;
            if (await CanDeactivateBranchOffices())
            {
                office = await _branchOfficesData.GetEvenDeactivatedAsync(id.GetValueOrDefault());

            }
            else
            {
                office = await _branchOfficesData.GetAsync(id.GetValueOrDefault());

            }

            if (office == null)
            {
                return NotFound();
            }

            Name = office.Name;
            Active = office.Active;
            if (office.Manager != null)
                ManagerIdCard = office.Manager.IdCard;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Models.BranchOffice office;
            if (await CanDeactivateBranchOffices())
            {
                office = await _branchOfficesData.GetEvenDeactivatedAsync(id);

            }
            else
            {
                office = await _branchOfficesData.GetAsync(id);

            }

            if (office == null)
            {
                return NotFound();
            }

            Models.User user = await _usersData.GetByIdCardAsync(ManagerIdCard.Replace("-", ""));

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "It was not possible to find a user with this id card. Try it again.");
                return Page();
            }

            if (!office.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase)
                & (await _branchOfficesData.GetAllEvenDeactivatedAsync()).Any(e => e.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "The name is not available.");
                _Logger.LogInformation($"Name: {Conversions.ExtraSpaceRemover(Name)} already exist in the DB.");
                return Page();
            }

            //Verify if the ID entered belongs to a person in charge of another BranchOffice
            if ((office.Manager == null || !ManagerIdCard.Equals(office.Manager.IdCard)) & (await _branchOfficesData.GetAllEvenDeactivatedAsync()).Any(e => e.ManagerId == user.UserId))
            {
                ModelState.AddModelError(string.Empty, "The introduced id card belongs to a manager from another office. Try it again.");
                return Page();
            }

            office.Name = Name;
            office.Manager = user;
            if (await CanDeactivateBranchOffices())
            {
                office.Active = Active;
            }


            Models.BranchOffice savedOffice = null;

            try
            {
                savedOffice = await _branchOfficesData.UpdateAsync(office);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Failed to update BranchOffice [ID: {office.BranchOfficeId}].");
                _Logger.LogDebug($"Error details: {ex}");
                ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
            }

            if (savedOffice == null)
            {
                return Page();
            }

            _Logger.LogInformation($"BranchOffice [ID: {office.BranchOfficeId}] has been edited successfully.");

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
