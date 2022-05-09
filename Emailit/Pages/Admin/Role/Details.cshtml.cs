using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.Role
{
    [HasPermission(Permissions.ReadRoles)]
    public class DetailsModel : PageModel
    {
        private readonly IRoleRepository _rolesData;
        private readonly IAuthorizationService _AuthorizationService;

        public DetailsModel(IRoleRepository roleRepository, IAuthorizationService authorizationService)
        {
            _rolesData = roleRepository;
            _AuthorizationService = authorizationService;
        }

        public Models.Role Role { get; set; }
        public IList<string> PermissionsList { get; private set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if ((await _AuthorizationService.AuthorizeAsync(User, Models.Permissions.DeactivateRoles.ToString())).Succeeded)
            {
                Role = await _rolesData.GetEvenDeactivatedAsync(id.GetValueOrDefault());

            }
            else
            {
                Role = await _rolesData.GetAsync(id.GetValueOrDefault());

            }

            if (Role == null)
            {
                return NotFound();
            }

            var Permissions = Enum.GetValues(typeof(Permissions))
                            .Cast<Permissions>()
                            .Where(p => Role.Permissions.HasFlag(p))
                            .ToList();

            foreach (var item in Permissions)
            {
                var PermissionName = item.GetType()
                                       .GetMember(item.ToString())
                                       .First()
                                       .GetCustomAttribute<DisplayAttribute>()
                                       .GetName();

                PermissionsList.Add(PermissionName);
            }

            return Page();
        }
    }
}
