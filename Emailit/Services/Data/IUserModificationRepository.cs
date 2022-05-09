using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IUserModificationRepository
    {
        Task<UserModification> AddAsync(UserModification modification);
        Task<UserModification> GetAsync(int modificationId);
    }
}
