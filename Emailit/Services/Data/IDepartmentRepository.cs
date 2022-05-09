using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IDepartmentRepository
    {
        Task<Department> GetAsync(int DepartmentId);
        Task<Department> GetEvenDeactivatedAsync(int DepartmentId);
        Task<IList<Department>> GetAllAsync();
        Task<IList<Department>> GetAllEvenDeactivatedAsync();
        Task<Department> AddAsync(Department department);
        Task Delete(Department department);
        Task<PagedList<Department>> GetAllAsync(DepartmentPaginationParameters param);
        Task<PagedList<Department>> GetAllEvenDeactivatedAsync(DepartmentPaginationParameters param);
        Task<Department> UpdateAsync(Department department);
    }
}
