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
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.BranchOffice
{
    [HasPermission(Permissions.ReadBranchOffices)]
    public class DetailsModel : PageModel
    {
        private readonly IBranchOfficeRepository _branchOfficesData;
        private readonly IAuthorizationService _AuthorizationService;

        public DetailsModel(IBranchOfficeRepository branchOfficeReporitory, IAuthorizationService authorizationService)
        {
            _branchOfficesData = branchOfficeReporitory;
            _AuthorizationService = authorizationService;
        }

        public Models.BranchOffice BranchOffice { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateBranchOffices.ToString())).Succeeded)
            {
                BranchOffice = await _branchOfficesData.GetEvenDeactivatedAsync(id.GetValueOrDefault());

            }
            else
            {
                BranchOffice = await _branchOfficesData.GetAsync(id.GetValueOrDefault());

            }

            if (BranchOffice == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
