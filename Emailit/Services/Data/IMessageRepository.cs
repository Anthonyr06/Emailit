using Emailit.Models;
using Emailit.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Emailit.Pages.Message.SentModel;

namespace Emailit.Services.Data
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message message);
        Task<Message> GetAsync(int messageId);
        Task<PagedList<Message>> GetAllAsync(int userId, SentMessagesPaginationParameters param);
        Task<Message> UpdateAsync(Message message);
    }
}
