using Emailit.Data;
using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Message;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class ReceivedMessageRepositoryEF : IReceivedMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ReceivedMessageRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> CountNotSeenAsync(int userId)
        {
            return _dbContext.ReceivedMessages.Where(mr => !mr.Deleted && mr.UserId == userId && !mr.States.Any(e => e.State == MessageState.seen))
                .CountAsync();
        }

        public async Task<PagedList<ReceivedMessage>> GetAllAsync(int userId, ReceivedMessagesPaginationParameters parameters)
        {
            IQueryable<ReceivedMessage> messages;

            if (!string.IsNullOrEmpty(parameters.Search))
            {

                //var text = Conversions.ExtraSpaceRemover(parameters.Busqueda);
                //char[] delimiterChars = { ' ', ',', '.', ':' };
                //var searchWords = text.ToLower().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);


                messages = _dbContext.ReceivedMessages.Where(mr => mr.Message.Body.Contains(parameters.Search) || mr.Message.Tittle.Contains(parameters.Search)
                                                                        || mr.Message.Sender.Email.IndexOf(parameters.Search) >= 0);
            }
            else
            {
                messages = _dbContext.ReceivedMessages;
                // messages = _dbContext.ReceivedMessages.Where(m => recibidos.Select(mr => mr.MessageId).Contains(m.MessageId));
            }

            return await PagedList<ReceivedMessage>.ToPagedListAsync(messages
                // .Where(mr => mr.UserId == UserId & !mr.Message.States.Any(e => e.State == MessageState.Deleted))
                .Where(mr => mr.UserId == userId & !mr.Deleted)
                //  .OrderByDescending(mr => mr)
                .OrderByDescending(mr => mr.States.First(e => e.State == MessageState.received).Date)
                .Include(mr => mr.Message).ThenInclude(m => m.AttachedFiles)
                .Include(mr => mr.States)
                .AsNoTracking(),
                parameters.PageNumber,
                parameters.PageSize,
                parameters.NavPaginationMaxNumber);
        }

        public async Task<ReceivedMessage> GetAsync(int userId, int messageId)
        {
            return await _dbContext.ReceivedMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(mr => mr.UserId == userId && mr.MessageId == messageId);
        }

        public async Task<ReceivedMessage> UpdateAsync(ReceivedMessage message)
        {
            _dbContext.Attach(message).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return message;
        }
    }
}
