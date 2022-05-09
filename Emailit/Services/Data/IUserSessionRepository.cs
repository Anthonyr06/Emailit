using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IUserSessionRepository
    {
        Task<UserSession> AddAsync(UserSession session);
        Task<UserSession> UpdateAsync(UserSession session);
        Task<UserSession> GetAsync(int userSessionId);
    }
}
