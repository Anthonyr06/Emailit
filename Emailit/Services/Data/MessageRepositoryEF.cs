using Emailit.Data;
using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Message;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class MessageRepositoryEF : IMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MessageRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Message> AddAsync(Message message)
        {
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }

        public async Task<PagedList<Message>> GetAllAsync(int userId, SentMessagesPaginationParameters parameters)
        {
            IQueryable<Message> messages;

            if (!string.IsNullOrEmpty(parameters.Search))
            {
                messages = _dbContext.Messages.Where(m => m.Body.Contains(parameters.Search) || m.Tittle.Contains(parameters.Search)
                                                                        || m.Sender.Email.IndexOf(parameters.Search) >= 0);
            }
            else
            {
                messages = _dbContext.Messages;
            }

            return await PagedList<Message>.ToPagedListAsync(messages
                .Where(m => m.SenderId == userId & !m.Deleted)
                .OrderByDescending(m => m.Date)
                //.OrderByDescending(m => m.States.First(e => e.State == MessageState.received).Date) //check
                .Include(m => m.Sender)
                .Include(m => m.Recipients).ThenInclude(mr => mr.States)
                .Include(m => m.AttachedFiles)
                .AsNoTracking(),
                parameters.PageNumber,
                parameters.PageSize,
                parameters.NavPaginationMaxNumber);
        }

        public async Task<Message> GetAsync(int messageId)
        {
            return await _dbContext.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Department).ThenInclude(d => d.BranchOffice)
                .Include(m => m.Recipients).ThenInclude(mr => mr.User)
                .Include(m => m.Recipients).ThenInclude(mr => mr.States)
                .Include(m => m.AttachedFiles).ThenInclude(am => am.File)
                //  .Include(m => m.States)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MessageId == messageId);
        }

        public async Task<Message> UpdateAsync(Message message)
        {
            _dbContext.Attach(message).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return message;
        }
    }
}
