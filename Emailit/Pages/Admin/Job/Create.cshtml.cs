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

namespace Emailit.Pages.Admin.Job
{
    [HasPermission(Permissions.WriteJobs)]
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _Logger;
        private readonly IJobRepository _jobsData;
        private readonly IAuthorizationService _AuthorizationService;

        public CreateModel(IJobRepository jobRepository, ILogger<CreateModel> logger, IAuthorizationService authorizationService)
        {
            _Logger = logger;
            _jobsData = jobRepository;
            _AuthorizationService = authorizationService;

        }


        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(JobDataValidation.MaxNameLenght, 
        ErrorMessage = "The {0} must have {1} maximum characters")]
        public string Name { get; set; }

        [BindProperty, Display(Name = "Description"), StringLength(JobDataValidation.MaxDescriptionLenght, 
        ErrorMessage = "The {0} must have {1} maximum characters")]
        public string Description { get; set; }

        [BindProperty, Display(Name = "Active job.")]
        public bool Active { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if ((await _jobsData.GetAllEvenDeactivatedAsync())
                .Any(e => e.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "The name is not available.");
                _Logger.LogInformation($"Name: {Name} already exist in the DB.");
                return Page();
            }

            var job = new Models.Job
            {
                Name = Name,
                Description = Description,
                Active = !await CanDeactivateJobs() || Active
            };


            Models.Job savedJob = null;

            try
            {
                savedJob = await _jobsData.AddAsync(job);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Error creating job.");
                _Logger.LogDebug($"Error details: {ex}");
                ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
            }

            if (savedJob == null)
            {
                return Page();
            }

            _Logger.LogInformation($"Job [ID: {job.JobId}] has been successfully created.");

            return RedirectToPage("./Index");
        }
        private async Task<bool> CanDeactivateJobs()
        {
            if ((await _AuthorizationService.AuthorizeAsync(User, Permissions.DeactivateJobs.ToString())).Succeeded)
            {
                return true;
            }

            return false;
        }
    }
}
