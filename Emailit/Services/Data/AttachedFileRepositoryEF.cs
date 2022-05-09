using Emailit.Data;
using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class AttachedFileRepositoryEF : IAttachedFileRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AttachedFileRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AttachedFile> GetAsync(int fileId)
        {
            return await _dbContext.AttachedFiles
                    .AsNoTracking()
                    .Include(am => am.Message).ThenInclude(m => m.Recipients)
                    .Include(am => am.File)
                    .FirstOrDefaultAsync(am => am.FileId == fileId);
        }

        public async Task<AttachedFile> AddAsync(AttachedFile file)
        {
            await _dbContext.AttachedFiles.AddAsync(file);
            await _dbContext.SaveChangesAsync();
            return file;
        }

    }
}
