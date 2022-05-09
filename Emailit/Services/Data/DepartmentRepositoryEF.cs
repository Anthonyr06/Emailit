using Emailit.Data;
using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.Department;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class DepartmentRepositoryEF : IDepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DepartmentRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IList<Department>> GetAllAsync()
        {
            return await _dbContext.Departments
                .Include(d => d.BranchOffice)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IList<Department>> GetAllEvenDeactivatedAsync()
        {
            return await _dbContext.Departments
                .AsNoTracking()
                .IgnoreQueryFilters()
                .ToListAsync();
        }
        public async Task<PagedList<Department>> GetAllAsync(DepartmentPaginationParameters parameters)
        {
            return await PagedList<Department>.ToPagedListAsync(_dbContext.Departments
                            .Include(d => d.Manager)
                            .Include(d => d.BranchOffice)
                            .OrderBy(d => d.Name)
                            .AsNoTracking(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }

        public async Task<PagedList<Department>> GetAllEvenDeactivatedAsync(DepartmentPaginationParameters parameters)
        {
            return await PagedList<Department>.ToPagedListAsync(_dbContext.Departments
                            .Include(d => d.Manager)
                            .Include(d => d.BranchOffice)
                            .OrderBy(d => d.Name)
                            .AsNoTracking()
                            .IgnoreQueryFilters(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }
        public async Task<Department> GetAsync(int DepartmentId)
        {
            return await _dbContext.Departments
                    .Include(d => d.BranchOffice)
                    .Include(d => d.Manager)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DepartmentId == DepartmentId);
        }
        public async Task<Department> GetEvenDeactivatedAsync(int DepartmentId)
        {
            return await _dbContext.Departments
                    .Include(d => d.BranchOffice)
                    .Include(d => d.Manager)
                    .AsNoTracking()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(d => d.DepartmentId == DepartmentId);
        }
        public async Task<Department> UpdateAsync(Department department)
        {
            _dbContext.Attach(department).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return department;
        }
        public async Task<Department> AddAsync(Department department)
        {
            await _dbContext.Departments.AddAsync(department);
            await _dbContext.SaveChangesAsync();
            return department;
        }

        public async Task Delete(Department department)
        {
            _dbContext.Attach(department).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }
    }
}
