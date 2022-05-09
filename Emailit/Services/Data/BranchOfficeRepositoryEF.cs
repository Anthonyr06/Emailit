using Emailit.Data;
using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.BranchOffice;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class BranchOfficeRepositoryEF : IBranchOfficeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BranchOfficeRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BranchOffice> AddAsync(BranchOffice branchOffice)
        {
            await _dbContext.BranchOffices.AddAsync(branchOffice);
            await _dbContext.SaveChangesAsync();
            return branchOffice;
        }

        public async Task Delete(BranchOffice branchOffice)
        {
            _dbContext.Attach(branchOffice).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IList<BranchOffice>> GetAllAsync()
        {
            return await _dbContext.BranchOffices
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IList<BranchOffice>> GetAllEvenDeactivatedAsync()
        {
            return await _dbContext.BranchOffices
                .AsNoTracking()
                .IgnoreQueryFilters()
                .ToListAsync();
        }

        public async Task<PagedList<BranchOffice>> GetAllAsync(BranchOfficePaginationParameters parameters)
        {
            return await PagedList<BranchOffice>.ToPagedListAsync(_dbContext.BranchOffices
                            .Include(e => e.Manager)
                            .OrderBy(e => e.Name)
                            .AsNoTracking(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }


        public async Task<PagedList<BranchOffice>> GetAllEvenDeactivatedAsync(BranchOfficePaginationParameters parameters)
        {
            return await PagedList<BranchOffice>.ToPagedListAsync(_dbContext.BranchOffices
                            .Include(e => e.Manager)
                            .OrderBy(e => e.Name)
                            .AsNoTracking()
                            .IgnoreQueryFilters(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }

        public async Task<BranchOffice> GetAsync(int branchOfficeId)
        {
            return await _dbContext.BranchOffices
                    .Include(e => e.Manager)
                    .Include(e => e.Departments)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.BranchOfficeId == branchOfficeId);
        }

        public async Task<BranchOffice> GetEvenDeactivatedAsync(int branchOfficeId)
        {
            return await _dbContext.BranchOffices
                    .Include(e => e.Manager)
                    .Include(e => e.Departments)
                    .AsNoTracking()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => e.BranchOfficeId == branchOfficeId);
        }

        public async Task<BranchOffice> UpdateAsync(BranchOffice branchOffice)
        {
            _dbContext.Attach(branchOffice).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return branchOffice;
        }
    }
}
