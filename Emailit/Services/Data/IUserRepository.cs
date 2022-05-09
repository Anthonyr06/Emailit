using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<User> GetAsync(int userId);
        Task<bool> CheckAsync(int userId);
        Task<User> GetEvenDeactivatedAsync(int userId);
        Task<User> GetByEmailAsync(string userEmail);
        Task<User> GetByIdCardAsync(string userIdCard);
        Task<bool> CheckByEmailAsync(string userEmail);
        Task<bool> CheckByIdCardAsync(string userIdCard);
        Task<User> UpdateAsync(User user);
        Task<PagedList<User>> GetAllAsync(UserPaginationParameters param);
        Task<PagedList<User>> GetAllEvenDeactivatedAsync(UserPaginationParameters param);
        Task<List<User>> GetAllAsync(string search, int qty);
    }
}
