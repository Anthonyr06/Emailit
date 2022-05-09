using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emailit.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordExpiredModel : PageModel
    {
        public string TokenIssuer { get; set; }
        public IActionResult OnGet(string i)
        {
            if (User.Identity.IsAuthenticated || string.IsNullOrEmpty(i))
            {
                return RedirectToPagePermanent("/Index");
            }

            TokenIssuer = i;

            return Page();
        }
    }
}