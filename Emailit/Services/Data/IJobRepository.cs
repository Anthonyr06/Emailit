using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IJobRepository
    {
        Task<Job> AddAsync(Job job);
        Task Delete(Job job);
        Task<IList<Job>> GetAllAsync();
        Task<PagedList<Job>> GetAllAsync(JobPaginationParameters param);
        Task<IList<Job>> GetAllEvenDeactivatedAsync();
        Task<PagedList<Job>> GetAllEvenDeactivatedAsync(JobPaginationParameters param);
        Task<Job> GetAsync(int jobId);
        Task<Job> GetEvenDeactivatedAsync(int jobId);
        Task<Job> UpdateAsync(Job job);

    }
}
