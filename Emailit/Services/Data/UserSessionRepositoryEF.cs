using Emailit.Data;
using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class UserSessionRepositoryEF : IUserSessionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserSessionRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserSession> AddAsync(UserSession session)
        {
            await _dbContext.UsersSessions.AddAsync(session);
            await _dbContext.SaveChangesAsync();
            return session;
        }

        public async Task<UserSession> GetAsync(int userSessionId)
        {
            return await _dbContext.UsersSessions
                .Include(su => su.User).ThenInclude(u => u.ModificationsReceived)
                .Include(su => su.User).ThenInclude(u => u.Roles)
                .Include(su => su.User).ThenInclude(u => u.ManagedDepartment)
                .AsNoTracking()
                .FirstOrDefaultAsync(su => su.UserSessionId == userSessionId);
        }

        public async Task<UserSession> UpdateAsync(UserSession session)
        {
            _dbContext.Attach(session).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return session;
        }
    }
}
