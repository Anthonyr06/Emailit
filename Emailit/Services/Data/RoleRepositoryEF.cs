using Emailit.Data;
using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.Role;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class RoleRepositoryEF : IRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role> AddAsync(Role role)
        {
            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            return role;
        }

        public async Task Delete(Role role)
        {
            _dbContext.Attach(role).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IList<Role>> GetAllAsync()
        {
            return await _dbContext.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedList<Role>> GetAllAsync(RolePaginationParameters parameters)
        {
            return await PagedList<Role>.ToPagedListAsync(_dbContext.Roles
                            .OrderBy(r => r.Name)
                            .AsNoTracking(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }

        public async Task<IList<Role>> GetAllEvenDeactivatedAsync()
        {
            return await _dbContext.Roles
                .AsNoTracking()
                .IgnoreQueryFilters()
                .ToListAsync();
        }

        public async Task<PagedList<Role>> GetAllEvenDeactivatedAsync(RolePaginationParameters parameters)
        {
            return await PagedList<Role>.ToPagedListAsync(_dbContext.Roles
                            .OrderBy(r => r.Name)
                            .AsNoTracking()
                            .IgnoreQueryFilters(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }

        public async Task<Role> GetAsync(int roleId)
        {
            return await _dbContext.Roles
               .AsNoTracking()
               .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<Role> GetEvenDeactivatedAsync(int roleId)
        {
            return await _dbContext.Roles
               .AsNoTracking()
               .IgnoreQueryFilters()
               .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            _dbContext.Attach(role).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return role;
        }
    }
}
