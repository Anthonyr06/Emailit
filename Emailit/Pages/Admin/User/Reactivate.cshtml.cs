using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Admin.User
{
    [HasPermission(Permissions.DeactivateUsers)]
    public class ReactivateModel : PageModel
    {

        private readonly IUserModificationRepository _userModificationsData;
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly ILogger<ReactivateModel> _Logger;
        private readonly IUserRepository _usersData;

        public ReactivateModel(IUserModificationRepository userModificationRepository, IActionContextAccessor contextAccessor,
            ILogger<ReactivateModel> logger, IUserRepository userRepository)
        {
            _userModificationsData = userModificationRepository;
            _ActionContextAccessor = contextAccessor;
            _Logger = logger;
            _usersData = userRepository;
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Models.User user = await _usersData.GetEvenDeactivatedAsync(id);

            if (user == null) return Page();

            if (user.Active)
            {
                _Logger.LogInformation($"User [UserID: {id}] is already activated.");
                return RedirectToPage("./Index");
            }


            //Getting UserId of user modifier from cookie
            var Claims = _ActionContextAccessor.ActionContext.HttpContext.User.Claims;
            if (!int.TryParse(Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
                out int ModifierId))
            {
                _Logger.LogError($"An error occurred while getting the modifier user id." +
                    $"User that was attempted to be modified was [UserID: {id}]");
                return RedirectToPage("./Index");
            }

            UserModification modification = new UserModification
            {
                Date = DateTime.UtcNow,
                ModificationType = ModificationType.REACTIVATED,
                ModifiedUserId = id,
                ModifierId = ModifierId
            };

            user.Active = true;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _usersData.UpdateAsync(user);
                    await _userModificationsData.AddAsync(modification);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _Logger.LogDebug($"An error occurred while editing user: {ex}");
                    _Logger.LogError($"An error occurred while deactivating user [UserId: {user.UserId}].");
                    _Logger.LogInformation($"Applying RollBack Users and UsersModifications tables");
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
