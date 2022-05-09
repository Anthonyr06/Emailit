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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Emailit.Data;
using Emailit.Models;
using Emailit.Services.Data;
using Emailit.Services.Policies;

namespace Emailit.Pages.Admin.Job
{
    [HasPermission(Permissions.WriteJobs)]
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _Logger;
        private readonly IJobRepository _jobsData;
        private readonly IAuthorizationService _AuthorizationService;

        [BindProperty, Required(ErrorMessage = "Enter a {0}"), StringLength(JobDataValidation.MaxNameLenght, 
            ErrorMessage = "The {0} must have {1} maximum characters")]
        public string Name { get; set; }

        [BindProperty, Display(Name = "Description"), StringLength(JobDataValidation.MaxDescriptionLenght, 
            ErrorMessage = "The {0} must have {1} maximum characters")]
        public string Description { get; set; }

        [BindProperty, Display(Name = "Active job.")]
        public bool Active { get; set; }

        public EditModel(IJobRepository jobRepository, ILogger<EditModel> logger, IAuthorizationService authorizationService)
        {
            _Logger = logger;
            _jobsData = jobRepository;
            _AuthorizationService = authorizationService;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Models.Job job;
            if (await CanDeactivateJobs())
            {
                job = await _jobsData.GetEvenDeactivatedAsync(id.GetValueOrDefault());

            }
            else
            {
                job = await _jobsData.GetAsync(id.GetValueOrDefault());

            }


            if (job == null)
            {
                return NotFound();
            }

            Name = job.Name;
            Description = job.Description;
            Active = job.Active;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Models.Job job;
            if (await CanDeactivateJobs())
            {
                job = await _jobsData.GetEvenDeactivatedAsync(id);

            }
            else
            {
                job = await _jobsData.GetAsync(id);

            }

            if (job == null)
            {
                return NotFound();
            }

            if (!job.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase) &
                (await _jobsData.GetAllEvenDeactivatedAsync()).Any(e => e.Name.Equals(Conversions.ExtraSpaceRemover(Name), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "The name is not available.");
                _Logger.LogInformation($"Name: {Name} already exist in the DB.");
                return Page();
            }

            job.Name = Name;
            job.Description = Description;
            if (await CanDeactivateJobs())
            {
                job.Active = Active;
            }


            Models.Job savedJob = null;

            try
            {
                savedJob = await _jobsData.UpdateAsync(job);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Failed to update job [ID: {job.JobId}].");
                _Logger.LogDebug($"Error details: {ex}");
                ModelState.AddModelError(string.Empty, "An error has occurred, please try again. If the error continues contact the IT department.");
            }

            if (savedJob == null)
            {
                return Page();
            }

            _Logger.LogInformation($"Job [ID: {job.JobId}] has been edited successfully..");

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
