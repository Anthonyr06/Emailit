using Emailit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services.Data
{
    public interface IAttachedFileRepository
    {
        Task<AttachedFile> GetAsync(int fileId);
        Task<AttachedFile> AddAsync(AttachedFile file);
    }
}
