using Emailit.Data;
using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class UserRoleRepositoryEF : IUserRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRoleRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAsync(UserRole userRole)
        {
            _dbContext.Attach(userRole).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }
    }
}
