using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using System.Transactions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.User
{
    [HasPermission(Permissions.CreateUsers)]
    public class CreateModel : PageModel
    {
        private readonly IUserRepository _usersData;
        private readonly IDepartmentRepository _departmentsData;
        private readonly IJobRepository _jobsData;
        private readonly IRoleRepository _rolesData;
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly IUserModificationRepository _userModificationsData;
        private readonly ILogger<CreateModel> _Logger;
        private readonly IAuthorizationService _AuthorizationService;


        [BindProperty, Display(Name = "Identification card"), Required(ErrorMessage = "Enter id card"),
            StringLength(13, ErrorMessage = "The {0} is incomplete."), RegularExpression(@"^\d{3}-\d{7}-\d$", 
            ErrorMessage = "The {0} is invalid.")]
        public string IdCard { get; set; }

        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(UserDataValidation.MaxNameLenght, 
            ErrorMessage = "The {0} must have {1} characters as maximum"), Display(Name = "Names")]
        public string Name { get; set; }

        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(UserDataValidation.MaxLastNameLenght, 
        ErrorMessage = "The {0} must have {1} characters as maximum"), Display(Name = "Surnames")]
        public string Lastname { get; set; }

        [BindProperty, Required(ErrorMessage = "Select a {0}"), Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [BindProperty, Display(Name = "Email"), DataType(DataType.EmailAddress),
            EmailAddress(ErrorMessage = "You must introduce a valid {0}."), Required(ErrorMessage = "Enter a {0}."),
            StringLength(UserDataValidation.MaxEmailLenght, ErrorMessage = "El {0} debe de tener un mínimo de {2} y un maximo de {1}.", 
            MinimumLength = UserDataValidation.MinEmailLenght)]
        public string Email { get; set; }

        [BindProperty, Required(ErrorMessage = "Select a {0}"), Display(Name = "Job")]
        public int JobId { get; set; }
        public SelectList JobList { get; private set; }

        [BindProperty, Required(ErrorMessage = "Select a {0}"), Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public SelectList DepartmentsList { get; private set; }

        [BindProperty, Display(Name = "Roles")]
        public IList<int> RolesIdsSelected { get; set; }
        public IList<Models.Role> RolesList { get; private set; }

        [BindProperty, Display(Name = "Permissions (optional)")]
        public IList<ulong> PermissionsList { get; set; }

        [BindProperty, Display(Name = "Active User")]
        public bool Active { get; set; }

        public CreateModel(IUserRepository userRepository, IDepartmentRepository departmentRepository,
            IJobRepository jobRepository, IRoleRepository roleRepository, ILogger<CreateModel> logger,
            IAuthorizationService authorizationService, IUserModificationRepository userModificationRepository,
            IActionContextAccessor actionContextAccessor)
        {
            _Logger = logger;
            _usersData = userRepository;
            _departmentsData = departmentRepository;
            _jobsData = jobRepository;
            _rolesData = roleRepository;
            _ActionContextAccessor = actionContextAccessor;
            _userModificationsData = userModificationRepository;
            _AuthorizationService = authorizationService;
        }

        public async Task<IActionResult> OnGet()
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

            if (await _usersData.CheckByIdCardAsync(IdCard.Replace("-", "")))
            {
                await GetViewDataFromDB();
                ModelState.AddModelError(string.Empty, "The id card is not available.");
                _Logger.LogInformation($"IdCard: {IdCard} already exist in the DB.");
                return Page();

            }
            if (await _usersData.CheckByEmailAsync(Conversions.ExtraSpaceRemover(Email.ToLower())))
            {
                await GetViewDataFromDB();
                ModelState.AddModelError(string.Empty, "Email is not available.");
                _Logger.LogInformation($"Email: {Email.Trim()} already exist in the DB.");
                return Page();
            }

            //Getting UserId of user modifier from cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int ModifierId))
            {
                _Logger.LogError($"An error occurred while getting the modifier user id.");
                await GetViewDataFromDB();
                return Page();
            }

            var passwordHasher = new PasswordHasher<string>();
            var hashedPassword = passwordHasher.HashPassword(null, IdCard.Replace("-", "")); //The IDCard is the default password (just the numbers)

            var user = new Models.User
            {
                IdCard = IdCard,
                Name = Name,
                Lastname = Lastname,
                Gender = Gender,
                Email = Email,
                Password = hashedPassword,
                JobId = JobId,
                DepartmentId = DepartmentId,
                MustChangePassword = true,
                Active = true
            };

            if (await CanDeactivateUsers())
            {
                user.Active = Active;
            }

            if (await CanWriteUsersPermissions() && PermissionsList.Any())
            {
                foreach (var item in PermissionsList)
                {
                    //validating if the selected permissions exist
                    if (!Enum.IsDefined(typeof(Permissions), item))
                    {
                        return Page();
                    }
                }

                ulong sumPermissionValues = PermissionsList.Aggregate((a, c) => a + c);
                var flags = (Permissions)sumPermissionValues;

                user.Permission = flags;
            }


            if (await CanWriteUsersRoles())
            {
                user.Roles = new List<UserRole>();

                foreach (var item in RolesIdsSelected)
                {
                    user.Roles.Add(new UserRole { RoleId = item });
                }
            }


            Models.User savedUser = null;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    savedUser = await _usersData.AddAsync(user);


                    UserModification modification = new UserModification
                    {
                        Date = DateTime.UtcNow,
                        ModificationType = ModificationType.CREATED,
                        ModifiedUserId = savedUser.UserId,
                        ModifierId = ModifierId
                    };

                    await _userModificationsData.AddAsync(modification);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _Logger.LogError($"Error creating user [IdCard : {IdCard}].");
                    _Logger.LogDebug($"Error details: {ex}");
                    _Logger.LogInformation($"Applying RollBack Users and UsersModifications tables");
                    ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
                }
            }

            if (savedUser == null)
            {
                await GetViewDataFromDB();
                return Page();
            }

            _Logger.LogInformation($"User [UserID: {user.UserId}] has been successfully created.");

            return RedirectToPage("./Details", new { Id = user.UserId });
        }
        private async Task<bool> CanWriteUsersRoles()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.WriteUsersRoles.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> CanWriteUsersPermissions()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.WriteUsersPermissions.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> CanDeactivateUsers()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateUsers.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }

        private async Task GetViewDataFromDB()
        {
            var departments = new List<Models.Department>();
            foreach (var item in await _departmentsData.GetAllAsync())
            {
                item.Name += " - " + item.BranchOffice.Name;
                departments.Add(item);
            }

            DepartmentsList = new SelectList(departments, nameof(DepartmentId), nameof(Models.Department.Name));
            JobList = new SelectList(await _jobsData.GetAllAsync(), nameof(JobId), nameof(Models.Job.Name));

            if (await CanWriteUsersRoles())
            {
                RolesList = await _rolesData.GetAllAsync();
            }
        }

    }
}
