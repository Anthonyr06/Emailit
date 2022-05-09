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
using Emailit.Models.Pagination;

namespace Emailit.Pages.Admin.Department
{
    [HasPermission(Permissions.ReadDepartments)]
    public class IndexModel : PageModel
    {
        private readonly IDepartmentRepository _departmentsData;
        private readonly IAuthorizationService _AuthorizationService;

        public IndexModel(IDepartmentRepository department, IAuthorizationService authorizationService)
        {
            _departmentsData = department;
            _AuthorizationService = authorizationService;
        }

        public PagedList<Models.Department> Department { get; set; }

        public async Task OnGetAsync([FromQuery] DepartmentPaginationParameters param)
        {

            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateDepartments.ToString())).Succeeded)
            {
                Department = await _departmentsData.GetAllEvenDeactivatedAsync(param);
            }
            else
            {
                Department = await _departmentsData.GetAllAsync(param);
            }

        }

    }
}
