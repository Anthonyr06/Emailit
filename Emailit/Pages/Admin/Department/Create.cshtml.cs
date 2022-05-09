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

namespace Emailit.Pages.Admin.Department
{
    [HasPermission(Permissions.WriteDepartments)]
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _Logger;
        private readonly IDepartmentRepository _departmentsData;
        private readonly IUserRepository _usersData;
        private readonly IBranchOfficeRepository _branchOfficesData;
        private readonly IAuthorizationService _AuthorizationService;


        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(DepartmentDataValidation.MaxNameLenght, ErrorMessage = "The {0} must have {1} maximum characters")]
        public string Name { get; set; }

        [BindProperty, Display(Name = "Manager ID Card"), Required(ErrorMessage = "Enter department manager ID card"),
        StringLength(13, ErrorMessage = "The {0} is incomplete."), RegularExpression(@"^\d{3}-\d{7}-\d$", ErrorMessage = "The {0} is invalid.")]
        public string ManagerIdCard { get; set; }

        [BindProperty, Required(ErrorMessage = "Select one {0}"), Display(Name = "Office")]
        public int BranchOfficeId { get; set; }
        public SelectList BranchOfficesList { get; private set; }

        [BindProperty, Display(Name = "Active department.")]
        public bool Active { get; set; }

        public CreateModel(ILogger<CreateModel> logger, IDepartmentRepository departmentRepository, IBranchOfficeRepository branchOfficeReporitory,
            IUserRepository userRepository, IAuthorizationService authorizationService)
        {
            _Logger = logger;
            _departmentsData = departmentRepository;
            _usersData = userRepository;
            _branchOfficesData = branchOfficeReporitory;
            _AuthorizationService = authorizationService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await GetViewDataFromDB();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await GetViewDataFromDB();
                return Page();
            }

            Models.User user = await _usersData.GetByIdCardAsync(ManagerIdCard.Replace("-", ""));

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "It was not possible to find a user with this id card. Try it again.");
                await GetViewDataFromDB();
                return Page();
            }

            //Check if the entered name already exists in the selected department.
            if ((await _departmentsData.GetAllEvenDeactivatedAsync())
                .Any(d => d.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase) & d.BranchOfficeId == BranchOfficeId))
            {
                ModelState.AddModelError(string.Empty, $"Department {Conversions.ExtraSpaceRemover(Name)} already exists in the selected office.");
                await GetViewDataFromDB();
                return Page();
            }

            //Verify if the ID entered belongs to a person in charge of another department.
            if ((await _departmentsData.GetAllEvenDeactivatedAsync()).Any(d => d.ManagerId == user.UserId))
            {
                ModelState.AddModelError(string.Empty, "The introduced id card belongs to a manager from another department. Try it again.");
                await GetViewDataFromDB();
                return Page();
            }


            var depto = new Models.Department
            {
                Name = Name,
                ManagerId = user.UserId,
                BranchOfficeId = BranchOfficeId,
                Active = !await CanDeactivateDepartments() || Active
            };


            Models.Department savedDepartment = null;

            try
            {
                savedDepartment = await _departmentsData.AddAsync(depto);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Error creating department.");
                _Logger.LogDebug($"Error details: {ex}");
                ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
            }

            if (savedDepartment == null)
            {
                await GetViewDataFromDB();
                return Page();
            }

            _Logger.LogInformation($"Department [ID: {depto.DepartmentId}] has been successfully created.");

            return RedirectToPage("./Details", new { id = depto.DepartmentId });
        }
        private async Task GetViewDataFromDB()
        {
            BranchOfficesList = new SelectList(await _branchOfficesData.GetAllAsync(), nameof(BranchOfficeId), nameof(Models.BranchOffice.Name));
        }
        private async Task<bool> CanDeactivateDepartments()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateDepartments.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }
    }
}
