using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IReceivedMessageRepository
    {
        Task<ReceivedMessage> GetAsync(int userId, int messageId);
        Task<PagedList<ReceivedMessage>> GetAllAsync(int userId, ReceivedMessagesPaginationParameters parameters);
        Task<ReceivedMessage> UpdateAsync(ReceivedMessage message);
        Task<int> CountNotSeenAsync(int userId);
    }
}
