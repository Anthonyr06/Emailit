using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Message
{
    public class ReceivedModel : PageModel
    {
        private readonly ILogger<ReceivedModel> _Logger;
        private readonly IReceivedMessageRepository _receivedMessageData;
        private readonly IReceivedMessageStateRepository _receivedMessageStateData;
        private readonly IActionContextAccessor _ActionContextAccessor;

        public ReceivedModel(ILogger<ReceivedModel> logger, IReceivedMessageRepository receivedMessageRepository, 
            IActionContextAccessor actionContextAccessor, IReceivedMessageStateRepository receivedMessageStateRepository)
        {
            _Logger = logger;
            _receivedMessageData = receivedMessageRepository;
            _receivedMessageStateData = receivedMessageStateRepository;
            _ActionContextAccessor = actionContextAccessor;
        }

        public PagedList<ReceivedMessage> Received { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery] ReceivedMessagesPaginationParameters param)
        {
            if (!string.IsNullOrEmpty(param.Search) && (param.Search.Length < 0 || param.Search.Length > 100))
            {
                param.Search = null;
            }

            //Getting UserId of the current user, from the cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _Logger.LogError($"An error occurred while obtaining the user ID.");
                return Page();
            }

            Received = await _receivedMessageData.GetAllAsync(userId, param);


            bool error = false;

            foreach (var item in Received)
            {
                //If messages were received while the user was offline
                if (!item.States.Any(e => e.State == MessageState.received))
                {
                    try
                    {
                        await _receivedMessageStateData.AddAsync(new ReceivedMessageState
                        {
                            UserId = userId,
                            MessageId = item.MessageId,
                            State = MessageState.received,
                            Date = DateTime.UtcNow
                        });

                    }
                    catch (Exception ex)
                    {
                        _Logger.LogDebug($"Error saving status received, in inbox: {ex}");
                        error = true;
                    }
                }

            }

            if (error) return StatusCode(500);

            Response.Headers.Add("pagination-current-page", Received.CurrentPage.ToString());
            Response.Headers.Add("pagination-has-next", Received.HasNext.ToString());


            return Page();
        }

        public async Task<JsonResult> OnGetNotSeenAsync()
        {
            //Getting UserId of the current user, from the cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int userId))
            {
                _Logger.LogError($"An error occurred while obtaining the user ID.");
                return new JsonResult("");
            }

            return new JsonResult(await _receivedMessageData.CountNotSeenAsync(userId));
        }

    }
}