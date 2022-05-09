using Emailit.Data;
using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class UserModificationRepositoryEF : IUserModificationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserModificationRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserModification> AddAsync(UserModification modification)
        {
            await _dbContext.UsersModifications.AddAsync(modification);
            await _dbContext.SaveChangesAsync();
            return modification;
        }

        public async Task<UserModification> GetAsync(int modificationId)
        {
            return await _dbContext.UsersModifications
                .Include(mu => mu.ModifiedUser)
                .Include(mu => mu.Modifier)
                .AsNoTracking()
                .FirstOrDefaultAsync(mu => mu.UserModificationId == modificationId);
        }
    }
}
