using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Emailit.Services.Data;

namespace Emailit.Pages.Message
{
    public class DeleteModel : PageModel
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly IMessageRepository _messageData;
        private readonly IReceivedMessageRepository _receivedMessageData;
        private readonly IActionContextAccessor _ActionContextAccessor;

        public DeleteModel(ILogger<DeleteModel> logger, IMessageRepository messageRepository, IReceivedMessageRepository receivedMessageRepository,
            IActionContextAccessor actionContextAccessor)
        {
            _logger = logger;
            _messageData = messageRepository;
            _receivedMessageData = receivedMessageRepository;
            _ActionContextAccessor = actionContextAccessor;
        }


        public enum CurrentView
        {
            receivedView = 1,
            sentView = 2
        }

        public async Task<IActionResult> OnPostAsync(int messageId, CurrentView currentView)
        {
            var message = await _messageData.GetAsync(messageId);

            if (message == null)
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

            bool error = false;

            if (currentView == CurrentView.receivedView) //if we are on the Received page
            {
                var receivedMessage = message.Recipients.Find(mr => mr.UserId == userId & !mr.Deleted);

                //var ReceivedMessage = await _MensajeRecibido.GetAsync(UserId, MessageId);

                if (receivedMessage != null)
                {
                    try
                    {
                        // ReceivedMessage.States = null; //nav property null to prevent the user entity from being instantiated in the current dbcontext
                        receivedMessage.Deleted = true;
                        await _receivedMessageData.UpdateAsync(receivedMessage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug($"error deleting receivedMessage: {ex}");
                        error = true;
                    }
                }
            }
            else if (currentView == CurrentView.sentView)//if we are on the Sent page
            {
                if (message.SenderId == userId & !message.Deleted)
                {
                    try
                    {
                        message.Deleted = true;
                        await _messageData.UpdateAsync(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug($"error deleting message: {ex}");
                        error = true;
                    }
                }
            }

            if (error)
            {
                return StatusCode(500);
            }

            return Page();
        }
    }
}
