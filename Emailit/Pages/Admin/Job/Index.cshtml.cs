using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Emailit.Models;
using Emailit.Services.Policies;
using Emailit.Services.Data;
using Emailit.Models.Pagination;

namespace Emailit.Pages.Admin.Job
{
    [HasPermission(Permissions.ReadJobs)]
    public class IndexModel : PageModel
    {
        private readonly IJobRepository _jobsData;
        private readonly IAuthorizationService _AuthorizationService;

        public PagedList<Models.Job> Job { get; set; }

        public IndexModel(IJobRepository jobsRepository, IAuthorizationService authorizationService)
        {
            _jobsData = jobsRepository;
            _AuthorizationService = authorizationService;
        }

        public async Task OnGetAsync([FromQuery] JobPaginationParameters param)
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateJobs.ToString())).Succeeded)
            {
                Job = await _jobsData.GetAllEvenDeactivatedAsync(param);
            }
            else
            {
                Job = await _jobsData.GetAllAsync(param);
            }
        }
    }
}
