using Emailit.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Services
{
    public interface IFileManagement
    {
        /// <summary>
        /// Method used to save files locally. 
        /// [In case of an error, this method is able to delete the files (Must be within a transaction)].
        /// </summary>
        /// <param name="files">FilesData list</param>
        /// <returns>A list of type FileData</returns>
        Task<IList<FileData>> SaveFilesAsync(IList<IFormFile> files);

        /// <summary>
        /// Method used to save one file locally. 
        /// [In case of an error, this method is able to delete the files (Must be within a transaction)].
        /// </summary>
        /// <param name="file">FileData</param>
        /// <returns>An object of type FileData</returns>
        Task<FileData> SaveFileAsync(IFormFile file);
    }
}
