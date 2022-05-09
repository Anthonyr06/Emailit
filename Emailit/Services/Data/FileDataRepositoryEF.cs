using Emailit.Data;
using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public class FileDataRepositoryEF : IFileDataRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FileDataRepositoryEF(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FileData> AddAsync(FileData file)
        {
            await _dbContext.FilesData.AddAsync(file);
            await _dbContext.SaveChangesAsync();
            return file;
        }

        public async Task<IList<FileData>> AddAsync(IList<FileData> files)
        {
            await _dbContext.FilesData.AddRangeAsync(files);
            await _dbContext.SaveChangesAsync();
            return files;
        }

        public async Task<FileData> GetAsync(int fileId)
        {
            return await _dbContext.FilesData
                      .AsNoTracking()
                      .FirstOrDefaultAsync(a => a.FileId == fileId);
        }
    }
}
