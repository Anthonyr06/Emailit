using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emailit.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
            Response.Headers.Add("REFRESH", "5;URL=/Index");
        }
    }
}