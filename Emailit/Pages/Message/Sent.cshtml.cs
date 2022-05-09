using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Emailit.Models.Pagination;
using Emailit.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Emailit.Pages.Message
{
    public class SentModel : PageModel
    {

        private readonly ILogger<SentModel> _logger;
        private readonly IMessageRepository _messageData;
        private readonly IActionContextAccessor _ActionContextAccessor;

        public SentModel(ILogger<SentModel> logger, IMessageRepository messageRepository, IActionContextAccessor actionContextAccessor)
        {
            _logger = logger;
            _messageData = messageRepository;
            _ActionContextAccessor = actionContextAccessor;
        }

        public PagedList<Models.Message> Sent { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery] SentMessagesPaginationParameters param)
        {
            if (!string.IsNullOrEmpty(param.Search) && (param.Search.Length < 0 || param.Search.Length > 100))
            {
                param.Search = null;
            }

            //Getting UserId of the current user, from the cookie
            var userIdClaim = _ActionContextAccessor.ActionContext.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            //Validating User Claim
            if (!int.TryParse(userIdClaim, out int Id))
            {
                _logger.LogError($"An error occurred while obtaining the user ID.");
                return Page();
            }

            Sent = await _messageData.GetAllAsync(Id, param);

            Response.Headers.Add("pagination-current-page", Sent.CurrentPage.ToString());
            Response.Headers.Add("pagination-has-next", Sent.HasNext.ToString());


            return Page();
        }

    }
}
