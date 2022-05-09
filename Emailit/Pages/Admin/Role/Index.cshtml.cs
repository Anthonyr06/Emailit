using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Models.Pagination;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.Role
{
    [HasPermission(Permissions.ReadRoles)]
    public class IndexModel : PageModel
    {
        private readonly IRoleRepository _rolesData;
        private readonly IAuthorizationService _AuthorizationService;

        public IndexModel(IRoleRepository roleRepository, IAuthorizationService authorizationService)
        {
            _rolesData = roleRepository;
            _AuthorizationService = authorizationService;
        }

        public PagedList<Models.Role> Role { get; set; }

        public async Task OnGetAsync([FromQuery] RolePaginationParameters param)
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateRoles.ToString())).Succeeded)
            {
                Role = await _rolesData.GetAllEvenDeactivatedAsync(param);
            }
            else
            {
                Role = await _rolesData.GetAllAsync(param);
            }
        }
    }
}
