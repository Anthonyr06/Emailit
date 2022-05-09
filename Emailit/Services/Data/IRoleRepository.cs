using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IRoleRepository
    {
        Task<Role> AddAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<Role> GetAsync(int roleId);
        Task<Role> GetEvenDeactivatedAsync(int roleId);
        Task<IList<Role>> GetAllAsync();
        Task<IList<Role>> GetAllEvenDeactivatedAsync();
        Task<PagedList<Role>> GetAllEvenDeactivatedAsync(RolePaginationParameters param);
        Task<PagedList<Role>> GetAllAsync(RolePaginationParameters param);
        Task Delete(Role role);
    }
}
