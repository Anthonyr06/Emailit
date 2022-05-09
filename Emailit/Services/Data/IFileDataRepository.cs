using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IFileDataRepository
    {
        Task<FileData> GetAsync(int fileId);
        Task<FileData> AddAsync(FileData file);
        Task<IList<FileData>> AddAsync(IList<FileData> files);
    }
}
