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

namespace Emailit.Pages.Admin.Department
{
    [HasPermission(Permissions.ReadDepartments)]
    public class DetailsModel : PageModel
    {
        private readonly IDepartmentRepository _departmentsData;
        private readonly IAuthorizationService _AuthorizationService;
        public Models.Department Department { get; set; }

        public DetailsModel(IDepartmentRepository department, IAuthorizationService authorizationService)
        {
            _departmentsData = department;
            _AuthorizationService = authorizationService;
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateDepartments.ToString())).Succeeded)
            {
                Department = await _departmentsData.GetEvenDeactivatedAsync(id.GetValueOrDefault());

            }
            else
            {
                Department = await _departmentsData.GetAsync(id.GetValueOrDefault());

            }

            if (Department == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
