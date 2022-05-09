using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Message
{
    public class GetFileModel : PageModel
    {
        private readonly ILogger<GetFileModel> _Logger;
        private readonly IActionContextAccessor _ActionContextAccessor;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IAttachedFileRepository _fileDataMensaje;

        public GetFileModel(ILogger<GetFileModel> logger, IActionContextAccessor actionContextAccessor, IWebHostEnvironment webHostEnvironment,
            IAttachedFileRepository attachedFileRepository)
        {
            _Logger = logger;
            _ActionContextAccessor = actionContextAccessor;
            _appEnvironment = webHostEnvironment;
            _fileDataMensaje = attachedFileRepository;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var fileData = await _fileDataMensaje.GetAsync(id);

            if (fileData == null)
                return NotFound();

            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _Logger.LogError($"An error occurred while obtaining the user ID.");
                return Page();
            }

            if (!UserIsAllow(fileData.Message, userId))
            {
                _Logger.LogInformation($"User [UserId: {userId}] has tried to access a file from an already deleted message.");
                return NotFound();
            }

            string path = Path.Join(_appEnvironment.ContentRootPath, fileData.File.Path);

            return PhysicalFile(path, fileData.File.ContentType, fileData.File.OriginalName);
        }

        private bool UserIsAllow(Models.Message message, int userId)
        {
            if (message.SenderId == userId & !message.Deleted)
                return true;

            if (message.Recipients.Any(mr => mr.UserId == userId & !mr.Deleted))
                return true;

            return false;
        }
    }
}
