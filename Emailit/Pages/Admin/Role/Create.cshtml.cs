using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.Role
{
    [HasPermission(Permissions.WriteRoles)]
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _Logger;
        private readonly IRoleRepository _rolesData;
        private readonly IAuthorizationService _AuthorizationService;

        public CreateModel(IRoleRepository roleRepository, ILogger<CreateModel> logger, IAuthorizationService authorizationService)
        {
            _Logger = logger;
            _rolesData = roleRepository;
            _AuthorizationService = authorizationService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(RoleDataValidation.MaxNameLenght, 
            ErrorMessage = "The {0} must have {1} characters as maximum")]
        public string Name { get; set; }

        [BindProperty, Display(Name = "Description"), StringLength(RoleDataValidation.MaxDescriptionLenght, 
            ErrorMessage = "The {0} must have {1} characters as maximum")]
        public string Description { get; set; }

        [BindProperty, Display(Name = "Active role.")]
        public bool Active { get; set; }

        [BindProperty, Display(Name = "Permissions list")]
        public IList<ulong> PermissionsList { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!PermissionsList.Any())
            {
                ModelState.AddModelError(string.Empty, "Select at least one permission!");
                return Page();
            }

            foreach (var item in PermissionsList)
            {
                //validating if the selected permissions exist
                if (!Enum.IsDefined(typeof(Permissions), item))
                {
                    return Page();
                }
            }

            if ((await _rolesData.GetAllEvenDeactivatedAsync())
                .Any(r => r.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "The name is not available.");
                _Logger.LogInformation($"Name: {Conversions.ExtraSpaceRemover(Name)} already exist in the DB.");
                return Page();
            }

            ulong sumPermissionValues = PermissionsList.Aggregate((a, c) => a + c);

            var flags = (Permissions)sumPermissionValues;

            var role = new Models.Role
            {
                Name = Name,
                Description = Description,
                Permissions = flags,
                Active = !await CanDeactivateRoles() || Active
            };

            Models.Role savedRole = null;

            try
            {
                savedRole = await _rolesData.AddAsync(role);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Error creating role.");
                _Logger.LogDebug($"Error details: {ex}");
                ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
            }

            if (savedRole == null)
            {
                return Page();
            }



            _Logger.LogInformation($"Role [ID: {role.RoleId}] has been successfully created.");

            return RedirectToPage("./Details", new { Id = role.RoleId });
        }
        private async Task<bool> CanDeactivateRoles()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateRoles.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }
    }
}
