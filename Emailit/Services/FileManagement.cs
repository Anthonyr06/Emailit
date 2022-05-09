using Emailit.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Emailit.Services
{
    public class FileManagement : IFileManagement, IEnlistmentNotification
    {
        private readonly ILogger<FileManagement> _Logger;
        private readonly IWebHostEnvironment _appEnvironment;

        private readonly string _directory;
        private const string _FolderName = "FilesUploads";

        private readonly IList<FileData> _filesData;

        public FileManagement(ILogger<FileManagement> logger, IWebHostEnvironment webHostEnvironment)
        {
            _Logger = logger;
            _appEnvironment = webHostEnvironment;

            _directory = Path.Combine(_appEnvironment.ContentRootPath, _FolderName);
            if (!Directory.Exists(_directory)) Directory.CreateDirectory(_directory);

            _filesData = new List<FileData>();
        }
        public async Task<IList<FileData>> SaveFilesAsync(IList<IFormFile> formFiles)
        {

            if (Transaction.Current != null)
                Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);

            IList<FileData> files = new List<FileData>();

            foreach (IFormFile formFile in formFiles)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(formFile.FileName);
                    string fullPath = Path.Combine(_directory, fileName);

                    var file = new Models.FileData()
                    {
                        Path = Path.Combine(_FolderName, fileName),
                        OriginalName = Path.GetFileName(formFile.FileName),
                        Extension = Path.GetExtension(formFile.FileName),
                        ContentType = formFile.ContentType,
                        LengthInBytes = formFile.Length
                    };

                    files.Add(file);

                    if (Transaction.Current != null)//This avoids deleting files created outside of a transaction.
                        _filesData.Add(file);

                    using var fileStream = new FileStream(fullPath, FileMode.Create);
                    await formFile.CopyToAsync(fileStream);
                }
            }

            return files;
        }

        public async Task<FileData> SaveFileAsync(IFormFile formFile)
        {
            if (Transaction.Current != null)
                Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);

            FileData file = new FileData();

            if (formFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(formFile.FileName);
                string fullPath = Path.Combine(_directory, fileName);

                file = new FileData()
                {
                    Path = Path.Combine(_FolderName, fileName),
                    OriginalName = Path.GetFileName(formFile.FileName),
                    Extension = Path.GetExtension(formFile.FileName),
                    ContentType = formFile.ContentType,
                    LengthInBytes = formFile.Length
                };

                if (Transaction.Current != null)//This avoids deleting files created outside of a transaction.
                    _filesData.Add(file);

                using var fileStream = new FileStream(fullPath, FileMode.Create);
                await formFile.CopyToAsync(fileStream);
            }

            return file;
        }


        public void Commit(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Rollback(Enlistment enlistment)
        {
            try
            {
                foreach (var item in _filesData)
                {
                    string path = Path.Join(_appEnvironment.ContentRootPath, item.Path);

                    if (File.Exists(path))
                        File.Delete(path);

                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                _Logger.LogError("Error when trying to delete files via RollBack");
                throw;
            }
            finally
            {
                enlistment.Done();
            }
        }

    }
}
