using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IEmailSender _EmailSender;
        private readonly IUserRepository _userData;
        private readonly ITokenProvider _TokenProvider;
        private readonly ILogger<ForgotPasswordModel> _Logger;
        private readonly IWebHostEnvironment _env;

        public ForgotPasswordModel(IEmailSender emailSender, IUserRepository userRepository, ITokenProvider tokenProvider,
            ILogger<ForgotPasswordModel> logger, IWebHostEnvironment hostEnvironment)
        {
            _EmailSender = emailSender;
            _userData = userRepository;
            _TokenProvider = tokenProvider;
            _Logger = logger;
            _env = hostEnvironment;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPagePermanent("/Index");
            }
            return Page();
        }

        [BindProperty, Display(Name = "Email"), DataType(DataType.EmailAddress),
            EmailAddress(ErrorMessage = "You must introduce a valid {0}."), Required(ErrorMessage = "Enter a {0}."),
            StringLength(UserDataValidation.MaxEmailLenght, ErrorMessage = "The {0} must have a minimum of {2} and a maximum of {1} characters.", 
            MinimumLength = UserDataValidation.MinEmailLenght)]
        public string Email { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                User user = await _userData.GetByEmailAsync(Conversions.ExtraSpaceRemover(Email.ToLower()));

                if (user != null)
                {
                    string issuer = "./ForgotPassword";

                    var token = _TokenProvider.GenerateForgotPasswordToken(user.UserId, user.Password, issuer);

                    var tittle = "Reset password";
                    var link = Url.PageLink("./ResetPassword", null, new { i = issuer, t = token });

                    var iconPath = _env.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "favicon.ico";

                    var files = new FormFileCollection();

                    var stream = System.IO.File.OpenRead(iconPath);

                    files.Add(new FormFile(stream, stream.Position, stream.Length, "Logo", Path.GetFileName(stream.Name))
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/ico"
                    });


                    var message = new EmailMessage(new string[] { user.Email }, tittle, HtlmTemplateResetPwd(link), files);

                    await _EmailSender.SendEmailAsync(message);

                    _Logger.LogInformation($"A password recovery email has been sent to the user [UserId: {user.UserId}] ");
                }

                return RedirectToPagePermanent("./ForgotPasswordConfirmation");

            }

            return Page();
        }

        private string HtlmTemplateResetPwd(string link)
        {
            //getting EmailTemplate.html file
            var pathToFile = _env.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplate.html";

            string htmlBody;

            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                htmlBody = SourceReader.ReadToEnd();
            }


            string urlIco = $"{Request.Scheme}://{Request.Host}{Request.PathBase}" +
                $"/favicon.ico";

            return string.Format(htmlBody
                , link, DateTime.Now.Year, urlIco);
        }

    }
}