using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;
using Microsoft.Extensions.Configuration;

namespace Emailit.Pages.Admin.User
{
    [HasPermission(Permissions.ReadUsers)]
    public class DetailsModel : PageModel
    {
        private readonly IUserRepository _usersData;
        private readonly IRoleRepository _rolesData;
        private readonly IAuthorizationService _AuthorizationService;
        private readonly IConfiguration _Configuration;

        public Models.User UserData { get; set; }
        [Display(Name = "User locked")]
        public bool Blocked { get; set; }

        [Display(Name = "Roles")]
        public IList<string> Roles { get; private set; } = new List<string>();

        [Display(Name = "Special permissions")]
        public IList<string> PermissionsList { get; private set; } = new List<string>();

        public DetailsModel(IUserRepository userRepository, IRoleRepository roleRepository, IAuthorizationService authorizationService, 
            IConfiguration configuration)
        {
            _usersData = userRepository;
            _rolesData = roleRepository;
            _AuthorizationService = authorizationService;
            _Configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserData = await CanDeactivateUsers() ? 
                await _usersData.GetEvenDeactivatedAsync(id.GetValueOrDefault()) : await _usersData.GetAsync(id.GetValueOrDefault());

            if (UserData == null)
            {
                return NotFound();
            }

            int loginAllowedAttempts = _Configuration.GetValue<int>("User:LoginFailedAttemps");

            Blocked = UserData.LoginAttempts >= loginAllowedAttempts;

            var roles = await _rolesData.GetAllAsync();

            foreach (var item in roles)
            {
                if (UserData.Roles.Any(r => r.RoleId == item.RoleId))
                {
                    Roles.Add(item.Name);
                }
            }

            var permissions = Enum.GetValues(typeof(Permissions))
                            .Cast<Permissions>()
                            .Where(p => p != 0 && (p & UserData.Permission) == p)
                            .ToList();

            foreach (var item in permissions)
            {
                var permissionName = item.GetType()
                                       .GetMember(item.ToString())
                                       .First()
                                       .GetCustomAttribute<DisplayAttribute>()
                                       .GetName();

                PermissionsList.Add(permissionName);
            }

            return Page();
        }

        private async Task<bool> CanDeactivateUsers()
        {
            if ((await _AuthorizationService.AuthorizeAsync(base.User, Permissions.DeactivateUsers.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }
    }
}
