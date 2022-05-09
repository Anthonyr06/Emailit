using Emailit.Data;
using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.Job;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class JobRepositoryEF : IJobRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public JobRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Job> AddAsync(Job job)
        {
            await _dbContext.Jobs.AddAsync(job);
            await _dbContext.SaveChangesAsync();
            return job;
        }

        public async Task Delete(Job job)
        {
            _dbContext.Attach(job).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IList<Job>> GetAllAsync()
        {
            return await _dbContext.Jobs
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<PagedList<Job>> GetAllAsync(JobPaginationParameters parameters)
        {
            return await PagedList<Job>.ToPagedListAsync(_dbContext.Jobs
                            .OrderBy(r => r.Name)
                            .AsNoTracking(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }

        public async Task<IList<Job>> GetAllEvenDeactivatedAsync()
        {
            return await _dbContext.Jobs
                .AsNoTracking()
                .IgnoreQueryFilters()
                .ToListAsync();
        }
        public async Task<PagedList<Job>> GetAllEvenDeactivatedAsync(JobPaginationParameters parameters)
        {
            return await PagedList<Job>.ToPagedListAsync(_dbContext.Jobs
                            .OrderBy(r => r.Name)
                            .AsNoTracking()
                            .IgnoreQueryFilters(),
                            parameters.PageNumber,
                            parameters.PageSize,
                            parameters.NavPaginationMaxNumber);
        }

        public async Task<Job> GetAsync(int jobId)
        {
            return await _dbContext.Jobs
                    .Include(pu => pu.Users)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pu => pu.JobId == jobId);
        }

        public async Task<Job> GetEvenDeactivatedAsync(int jobId)
        {
            return await _dbContext.Jobs
                    .Include(pu => pu.Users)
                    .AsNoTracking()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(pu => pu.JobId == jobId);
        }
        public async Task<Job> UpdateAsync(Job job)
        {
            _dbContext.Attach(job).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return job;
        }
    }
}
