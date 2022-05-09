using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.User
{
    [HasPermission(Permissions.UpdateUsers)]
    public class EditModel : PageModel
    {
        private readonly IUserRepository _usersData;
        private readonly IDepartmentRepository _departmentsData;
        private readonly IJobRepository _jobsData;
        private readonly IUserModificationRepository _userModificationsData;
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly IRoleRepository _rolesData;
        private readonly IUserRoleRepository _usersRolesData;
        private readonly ILogger<EditModel> _Logger;
        private readonly IAuthorizationService _AuthorizationService;
        private readonly IConfiguration _Configuration;

        public EditModel(IUserRepository userRepository, IDepartmentRepository departmentRepository, IJobRepository jobRepository,
            ILogger<EditModel> logger, IUserModificationRepository userModificationRepository, IActionContextAccessor actionContextAccessor,
            IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IAuthorizationService authorizationService, IConfiguration configuration)
        {
            _Logger = logger;
            _usersData = userRepository;
            _departmentsData = departmentRepository;
            _jobsData = jobRepository;
            _userModificationsData = userModificationRepository;
            _ActionContextAccessor = actionContextAccessor;
            _rolesData = roleRepository;
            _usersRolesData = userRoleRepository;
            _AuthorizationService = authorizationService;
            _Configuration = configuration;
        }

        [BindProperty, Display(Name = "Identification card"), StringLength(13, ErrorMessage = "The {0} is incomplete."), Required(ErrorMessage = "Enter id card"),
            RegularExpression(@"^\d{3}-\d{7}-\d$", ErrorMessage = "The {0} is invalid.")]
        public string IdCard { get; set; }

        [BindProperty, StringLength(UserDataValidation.MaxNameLenght, ErrorMessage = "The {0} must have {1} characters as maximum"), Required(ErrorMessage = "Enter a {0}"),
            Display(Name = "Names")]
        public string Name { get; set; }

        [BindProperty, StringLength(UserDataValidation.MaxLastNameLenght, ErrorMessage = "The {0} must have {1} characters as maximum"),
            Display(Name = "Surnames"), Required(ErrorMessage = "Enter a {0}")]
        public string Lastname { get; set; }

        [BindProperty, Display(Name = "Gender"), Required(ErrorMessage = "Select a {0}")]
        public Gender Gender { get; set; }

        [BindProperty, Display(Name = "Email"), DataType(DataType.EmailAddress), EmailAddress(ErrorMessage = "You must introduce a valid {0}."),
            StringLength(UserDataValidation.MaxEmailLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1}.", MinimumLength = UserDataValidation.MinEmailLenght), Required(ErrorMessage = "Enter a {0}.")]
        public string Email { get; set; }

        [BindProperty, Display(Name = "Job"), Required(ErrorMessage = "Select a {0}")]
        public int JobId { get; set; }
        public SelectList JobList { get; private set; }

        [BindProperty, Display(Name = "Department"), Required(ErrorMessage = "Select a {0}")]
        public int DepartmentId { get; set; }
        public SelectList DeparmentsList { get; private set; }

        [BindProperty, Display(Name = "Active user.")]
        public bool Active { get; set; }

        [BindProperty, Display(Name = "Lock user.")]
        public bool Blocked { get; set; }

        [BindProperty, Display(Name = "Must change password.")]
        public bool MustChangePassword { get; set; }

        [BindProperty, Display(Name = "Select the roles")]
        public IList<int> RolesIdsSelected { get; set; }
        public IList<Models.Role> RolesList { get; private set; }

        [BindProperty]
        public IList<ulong> PermissionsList { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Getting UserId of user modifier from cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int ModifierId))
            {
                _Logger.LogError($"An error occurred while getting the modifier user id." +
                    $"User that was attempted to be modified was [UserID: {id}]");
                await GetViewDataFromDB();
                return Page();
            }

            //User will not be able to auto modify from this page
            if (ModifierId == id)
            {
                return RedirectToPage("/Account/Manage/Index");
            }

            var user = await CanDeactivateUsers() ? await _usersData.GetEvenDeactivatedAsync(id.GetValueOrDefault()) : await _usersData.GetAsync(id.GetValueOrDefault());

            if (user == null)
            {
                return NotFound();
            }


            await GetViewDataFromDB();

            int loginAllowedAttempts = _Configuration.GetValue<int>("User:LoginFailedAttemps");


            IdCard = user.IdCard;
            Name = user.Name;
            Lastname = user.Lastname;
            Gender = user.Gender;
            Email = user.Email;
            JobId = user.JobId.GetValueOrDefault();
            DepartmentId = user.DepartmentId.GetValueOrDefault();
            Active = user.Active;
            Blocked = user.LoginAttempts >= loginAllowedAttempts;
            MustChangePassword = user.MustChangePassword;

            if (await CanWriteUsersRoles())
            {
                RolesIdsSelected = new List<int>();
                foreach (var item in user.Roles)
                    RolesIdsSelected.Add(item.RoleId);
            }

            if (await CanWriteUsersPermissions())
            {
                PermissionsList = Enum.GetValues(typeof(Permissions))
                            .Cast<Permissions>()
                            .Where(p => p != 0 && (p & user.Permission) == p)
                            .Cast<ulong>()
                            .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await GetViewDataFromDB();
                return Page();
            }


            //Getting UserId of user modifier from cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int ModifierId))
            {
                _Logger.LogError($"An error occurred while getting the modifier user id." +
                    $"User that was attempted to be modified was [UserID: {id}]");
                await GetViewDataFromDB();
                return Page();
            }


            if (ModifierId == id)//User will not be able to auto modify from this page
                return RedirectToPage("/Account/Manage/Index");

            Models.User user = await CanDeactivateUsers() ? await _usersData.GetEvenDeactivatedAsync(id) : await _usersData.GetAsync(id);

            if (user == null)
            {
                _Logger.LogInformation($"User: {Email} no longer exists.");
                await GetViewDataFromDB();
                return RedirectToPage("./Index");
            }


            if (!IdCard.Equals(user.IdCard) & await _usersData.CheckByIdCardAsync(IdCard.Replace("-", "")))
            {

                ModelState.AddModelError(string.Empty, $"There is already a user with the id card: {IdCard}.");
                await GetViewDataFromDB();
                return Page();
            }

            if (!Email.Equals(Conversions.ExtraSpaceRemover(user.Email), StringComparison.OrdinalIgnoreCase) & await _usersData.CheckByEmailAsync(Email.ToLower()))
            {
                ModelState.AddModelError(string.Empty, $"There is already a user with email: {Email}.");
                await GetViewDataFromDB();
                return Page();
            }

            if (await CanWriteUsersPermissions())
            {
                if (PermissionsList.Any())
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
                else
                {
                    //if no permission is selected
                    user.Permission = null;
                }
            }

            int loginAllowedAttempts = _Configuration.GetValue<int>("User:LoginFailedAttemps");

            user.IdCard = IdCard;
            user.Name = Name;
            user.Lastname = Lastname;
            user.Gender = Gender;
            user.Email = Email;
            user.LoginAttempts = Blocked ? loginAllowedAttempts : 0;
            user.MustChangePassword = MustChangePassword;

            //If for some reason the user does not have any positions stored in the DB
            if (user.Job != null)
            {
                user.Job.JobId = JobId;
            }
            else
            {
                user.JobId = JobId;
            }
            //If for some reason the user does not have any department stored in the DB
            if (user.Department != null)
            {
                user.Department.DepartmentId = DepartmentId;
            }
            else
            {
                user.DepartmentId = DepartmentId;
            }

            if (await CanDeactivateUsers())
            {
                user.Active = Active;
            }


            if (MustChangePassword)
            {
                var passwordHasher = new PasswordHasher<string>();
                //IdCard is the default password
                var hashedPassword = passwordHasher.HashPassword(null, IdCard.Replace("-", ""));

                user.Password = hashedPassword;
            }

            UserModification modification = new UserModification
            {
                Date = DateTime.UtcNow,
                ModificationType = ModificationType.MODIFIED,
                ModifiedUserId = user.UserId,
                ModifierId = ModifierId
            };

            Models.User savedUser = null;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    if (await CanWriteUsersRoles())
                    {
                        foreach (var userRole in user.Roles.ToList())
                        {
                            //Removing the roles that were deselected in the view
                            if (!RolesIdsSelected.Contains(userRole.RoleId))
                            {
                                user.Roles.Remove(userRole);
                                await _usersRolesData.DeleteAsync(userRole);
                            }
                        }

                        foreach (var newRoleId in RolesIdsSelected)
                        {
                            // Adding new roles
                            if (!user.Roles.Any(r => r.RoleId == newRoleId))
                            {
                                user.Roles.Add(new UserRole { RoleId = newRoleId });
                            }
                        }
                    }


                    await _userModificationsData.AddAsync(modification);
                    savedUser = await _usersData.UpdateAsync(user);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _Logger.LogDebug($"An error occurred while editing user: {ex}");
                    _Logger.LogError($"An error occurred while editing user [UserID: {user.UserId}].");
                    _Logger.LogInformation($"Applying RollBack Users and UsersModifications tables");
                    ModelState.AddModelError(string.Empty, $"An error occurred when editing user. Try it again.");
                }
            }

            if (savedUser == null)
            {
                await GetViewDataFromDB();
                return Page();
            }

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


            DeparmentsList = new SelectList(departments, nameof(DepartmentId), nameof(Models.Department.Name));
            JobList = new SelectList(await _jobsData.GetAllAsync(), nameof(JobId), nameof(Models.Job.Name));

            if (await CanWriteUsersRoles())
                RolesList = await _rolesData.GetAllAsync();
        }
    }
}
