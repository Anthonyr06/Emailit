using Emailit.Models;
using Emailit.Models.Pagination;
using Emailit.Pages.Admin.BranchOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IBranchOfficeRepository
    {
        Task<BranchOffice> AddAsync(BranchOffice branchOffice);
        Task Delete(BranchOffice branchOffice);
        Task<IList<BranchOffice>> GetAllAsync();
        Task<IList<BranchOffice>> GetAllEvenDeactivatedAsync();
        Task<PagedList<BranchOffice>> GetAllAsync(BranchOfficePaginationParameters param);
        Task<PagedList<BranchOffice>> GetAllEvenDeactivatedAsync(BranchOfficePaginationParameters param);
        Task<BranchOffice> GetAsync(int branchOfficeId);
        Task<BranchOffice> GetEvenDeactivatedAsync(int branchOfficeId);
        Task<BranchOffice> UpdateAsync(BranchOffice branchOffice);
    }
}
