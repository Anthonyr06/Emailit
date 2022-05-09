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

namespace Emailit.Pages.Admin.BranchOffice
{
    [HasPermission(Permissions.ReadBranchOffices)]
    public class IndexModel : PageModel
    {
        private readonly IBranchOfficeRepository _branchOfficesData;
        private readonly IAuthorizationService _AuthorizationService;

        public IndexModel(IBranchOfficeRepository officeReposiroty, IAuthorizationService authorizationService)
        {
            _branchOfficesData = officeReposiroty;
            _AuthorizationService = authorizationService;
        }

        public PagedList<Models.BranchOffice> BranchOffice { get; set; }

        public async Task OnGetAsync([FromQuery] BranchOfficePaginationParameters param)
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateBranchOffices.ToString())).Succeeded)
            {
                BranchOffice = await _branchOfficesData.GetAllEvenDeactivatedAsync(param);
            }
            else
            {
                BranchOffice = await _branchOfficesData.GetAllAsync(param);
            }
        }
    }
}
