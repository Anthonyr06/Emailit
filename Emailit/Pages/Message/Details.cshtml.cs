using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Emailit.Models;
using Emailit.Services.Data;

namespace Emailit.Pages.Message
{
    public class DetailsModel : PageModel
    {
        private readonly ILogger<DetailsModel> _logger;
        private readonly IMessageRepository _messageData;
        private readonly IReceivedMessageStateRepository _receivedMessageStateData;
        private readonly IActionContextAccessor _ActionContextAccessor;

        public DetailsModel(ILogger<DetailsModel> logger, IMessageRepository messageRepository, 
            IActionContextAccessor actionContextAccessor, IReceivedMessageStateRepository receivedMessageStateRepository)
        {
            _logger = logger;
            _messageData = messageRepository;
            _receivedMessageStateData = receivedMessageStateRepository;
            _ActionContextAccessor = actionContextAccessor;
        }

        public Models.Message Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Getting UserId of the current user, from the cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogError($"An error occurred while obtaining the user ID.");
                return Page();
            }

            Message = await _messageData.GetAsync(id.GetValueOrDefault());

            if (Message == null || !UserIsAllow(userId))
            {
                return NotFound();
            }

            bool error = false;

            if (Message.Recipients.SingleOrDefault(mr => mr.UserId == userId) != null && 
                !Message.Recipients.SingleOrDefault(mr => mr.UserId == userId).States.Any(e => e.State == MessageState.seen))
            {
                try
                {
                    await _receivedMessageStateData.AddAsync(new ReceivedMessageState
                    {
                        UserId = userId,
                        MessageId = Message.MessageId,
                        State = MessageState.seen,
                        Date = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogDebug($"Error opening message: {ex}");
                    error = true;
                }
            }

            if (error)
            {
                return StatusCode(500);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetJsonAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogError($"An error occurred while obtaining the user ID.");
                return Page();
            }

            Message = await _messageData.GetAsync(id.GetValueOrDefault());

            if (Message == null || !UserIsAllow(userId))
            {
                return NotFound();
            }

            var json = new JsonResult(
                new
                {
                    Message.Tittle,
                    Body = Message.BodyInHtml,
                    Message.Confidential,
                    Message.Priority,
                    Attachments = Message.AttachedFiles.Select(a => new
                    {
                        name = a.File.OriginalName,
                        url = Url.Page("GetFile", new { id = a.FileId })
                    })
                });

            return json;
        }
        private bool UserIsAllow(int userId)
        {
            if (Message.SenderId == userId & !Message.Deleted)
                return true;

            if (Message.Recipients.Any(mr => mr.UserId == userId & !mr.Deleted))
                return true;

            return false;
        }
    }
}
