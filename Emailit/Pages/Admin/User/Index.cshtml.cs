using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Models.Pagination;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.User
{
    [HasPermission(Permissions.ReadUsers)]
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _usersData;
        private readonly IAuthorizationService _AuthorizationService;

        public IndexModel(IUserRepository userRepository, IAuthorizationService authorizationService)
        {
            _usersData = userRepository;
            _AuthorizationService = authorizationService;
        }

        [BindProperty(SupportsGet = true), Display(Name = "Search by id card number:"), Required(ErrorMessage = "Enter id card"),
        StringLength(13, ErrorMessage = "The {0} is incomplete."), RegularExpression(@"^\d{3}-\d{7}-\d$", ErrorMessage = "The {0} is invalid.")]
        public string IdCard { get; set; }
        public PagedList<Models.User> Users { get; set; }
        public async Task<PageResult> OnGetAsync([FromQuery] UserPaginationParameters param)
        {
            if (ModelState.IsValid)
            {
                param.IdCard = IdCard;
            }
            else
            {
                param.IdCard = null;
            }

            Users = await CanDeactivateUsers() ? await _usersData.GetAllEvenDeactivatedAsync(param) : await _usersData.GetAllAsync(param);

            return Page();
        }

        private async Task<bool> CanDeactivateUsers()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateUsers.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }
    }

}
