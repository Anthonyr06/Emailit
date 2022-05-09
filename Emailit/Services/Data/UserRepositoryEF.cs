using Emailit.Data;
using Emailit.Models;
using Emailit.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class UserRepositoryEF : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<PagedList<User>> GetAllAsync(UserPaginationParameters parameters)
        {
            IQueryable<User> users;

            if (parameters.IdCard != null)
            {
                users = _dbContext.Users.Where(u => u.IdCard == parameters.IdCard);
            }
            else
            {
                users = _dbContext.Users;
            }

            return await PagedList<User>.ToPagedListAsync(users.OrderByDescending(u => u.Created)
                .Include(u => u.Department)
                .Include(u => u.Job)
                .AsNoTracking(),
                parameters.PageNumber,
                parameters.PageSize,
                parameters.NavPaginationMaxNumber);
        }
        public async Task<PagedList<User>> GetAllEvenDeactivatedAsync(UserPaginationParameters parameters)
        {
            IQueryable<User> users;

            if (parameters.IdCard != null)
            {
                users = _dbContext.Users.Where(u => u.IdCard == parameters.IdCard);
            }
            else
            {
                users = _dbContext.Users;
            }

            return await PagedList<User>.ToPagedListAsync(users.OrderByDescending(u => u.Created)
                .Include(u => u.Department)
                .Include(u => u.Job)
                .AsNoTracking()
                .IgnoreQueryFilters(),
                parameters.PageNumber,
                parameters.PageSize,
                parameters.NavPaginationMaxNumber);
        }

        public async Task<List<User>> GetAllAsync(string search, int qty)
        {
            return await _dbContext.Users
                .Where(u => u.Name.Contains(search) || u.Lastname.Contains(search) || u.Email.IndexOf(search) >= 0)
                .Take(qty)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetAsync(int userId)
        {
            return await _dbContext.Users
                .Include(u => u.Department)
                .Include(u => u.Job)
                .Include(u => u.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<bool> CheckAsync(int userId)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .AnyAsync(u => u.UserId == userId);
        }
        public async Task<User> GetEvenDeactivatedAsync(int userId)
        {

            return await _dbContext.Users
                .Include(u => u.Department)
                .Include(u => u.Job)
                .Include(u => u.Roles)
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<User> GetByEmailAsync(string userEmail)
        {
            return await _dbContext.Users
                .Include(u => u.ModificationsReceived)
                .Include(u => u.Roles)
                .Include(u => u.ManagedDepartment)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == userEmail);
        }
        public async Task<User> GetByIdCardAsync(string userIdCard)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdCard == userIdCard);
        }
        public async Task<bool> CheckByIdCardAsync(string userIdCard)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .AnyAsync(u => u.IdCard == userIdCard);
        }

        public async Task<bool> CheckByEmailAsync(string userEmail)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .IgnoreQueryFilters()
                .AnyAsync(u => u.Email == userEmail);
        }

        public async Task<User> UpdateAsync(User user)
        {
            _dbContext.Attach(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return user;
        }

    }
}
