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

namespace Emailit.Pages.Admin.Role
{
    [HasPermission(Permissions.WriteRoles)]
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _Logger;
        private readonly IRoleRepository _rolesData;
        private readonly IAuthorizationService _AuthorizationService;


        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(RoleDataValidation.MaxNameLenght, ErrorMessage = "The {0} must have {1} characters as maximum")]
        public string Name { get; set; }

        [BindProperty, StringLength(RoleDataValidation.MaxDescriptionLenght, ErrorMessage = "The {0} must have {1} characters as maximum")]
        public string Description { get; set; }

        [BindProperty, Display(Name = "Active role")]
        public bool Active { get; set; }

        [BindProperty, Display(Name = "Permissions list")]
        public IList<ulong> PermissionsList { get; set; }

        public EditModel(IRoleRepository roleRepository, ILogger<EditModel> logger, IAuthorizationService authorizationService)
        {
            _Logger = logger;
            _rolesData = roleRepository;
            _AuthorizationService = authorizationService;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.Role role = null;

            if (await CanDeactivateRoles())
            {
                role = await _rolesData.GetEvenDeactivatedAsync(id.GetValueOrDefault());

            }
            else
            {
                role = await _rolesData.GetAsync(id.GetValueOrDefault());

            }


            if (role == null)
            {
                return NotFound();
            }

            Name = role.Name;
            Description = role.Description;
            Active = role.Active;
            PermissionsList = Enum.GetValues(typeof(Permissions))
                            .Cast<Permissions>()
                            .Where(p => role.Permissions.HasFlag(p))
                            .Cast<ulong>()
                            .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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


            Models.Role role = null;

            if (await CanDeactivateRoles())
            {
                role = await _rolesData.GetEvenDeactivatedAsync(id);

            }
            else
            {
                role = await _rolesData.GetAsync(id);

            }

            if (role == null)
            {
                return NotFound();
            }

            //Name must be unique
            if (!role.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase) &
                (await _rolesData.GetAllEvenDeactivatedAsync()).Any(r => r.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "The name is not available.");
                _Logger.LogInformation($"Name: {Conversions.ExtraSpaceRemover(Name)} already exist in the DB.");
                return Page();
            }


            ulong sumPermissionValues = PermissionsList.Aggregate((a, c) => a + c);

            var flags = (Permissions)sumPermissionValues;

            role.Name = Name;
            role.Description = Description;
            role.Permissions = flags;
            if (await CanDeactivateRoles())
            {
                role.Active = Active;
            }

            Models.Role savedRole = null;

            try
            {
                savedRole = await _rolesData.UpdateAsync(role);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Failed to update role [RoleId: {role.RoleId}].");
                _Logger.LogDebug($"Error details: {ex}");
                ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
            }

            if (savedRole == null)
            {
                return Page();
            }

            _Logger.LogInformation($"Role [RoleId: {role.RoleId}] has been edited successfully..");

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
