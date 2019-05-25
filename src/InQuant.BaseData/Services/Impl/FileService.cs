using InQuant.BaseData.Models;
using InQuant.BaseData.Models.Entities;
using InQuant.BaseData.Models.Files;
using InQuant.Framework.Data.Core.Repositories;
using InQuant.Framework.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.BaseData.Services.Impl
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly BaseDataOptions _options;
        private readonly IRepository<FileContent> _fileRepository;
        private readonly IFileStorageProvider _fileStorageProvider;

        public FileService(IOptions<BaseDataOptions> options,
            ILogger<FileService> logger,
            IFileStorageProvider fileStorageProvider,
            IRepository<FileContent> fileRepository)
        {
            _options = options.Value;
            _logger = logger;
            _fileStorageProvider = fileStorageProvider;
            _fileRepository = fileRepository;
        }

        private static string Combine(string path, string fileName)
        {
            return Path.Combine(path, fileName).Replace('\\', '/');
        }

        public async Task<string> GetPublicUrl(string relativePath)
        {
            return await _fileStorageProvider.GetPublicUrl(relativePath);
        }

        public async Task<FileModel> GetById(int fileId)
        {
            var r = await _fileRepository.GetAsync(fileId);

            return new FileModel()
            {
                Id = r.Id,
                FileName = r.FileName,
                Folder = r.Folder,
                PublicUrl = await GetPublicUrl(r.ReletivePath),
                ReletivePath = r.ReletivePath
            };
        }

        public async Task<Stream> Open(string relativePath)
        {
            if (!await _fileStorageProvider.Exists(relativePath))
                throw new HopexException("file not exists.");

            return await _fileStorageProvider.Open(relativePath);
        }

        public async Task<Stream> Open(string path, string fileName)
        {
            return await Open(Combine(path, fileName));
        }

        public async Task<FileContent> Save(string path, string fileName, Stream fileStream)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

            fileName = GetUniqueFileName(fileName);
            string relativePath = Combine(path, fileName);

            //ensure to begin
            fileStream.Seek(0, SeekOrigin.Begin);
            await _fileStorageProvider.Save(relativePath, fileStream);

            var r = new FileContent()
            {
                FileName = fileName,
                ReletivePath = relativePath,
                Folder = path,
                LastModifiedTime = DateTime.Now,
            };
            await _fileRepository.InsertAsync(r);

            return r;
        }

        private string GetUniqueFileName(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName.Replace(' ', '_'));
            return $"{name}-{Thread.CurrentThread.ManagedThreadId}-{DateTime.Now.Ticks}{Path.GetExtension(fileName)}";
        }

        public async Task<IEnumerable<FileModel>> Gets(params int[] ids)
        {
            var data = new List<FileModel>();

            if (ids.Length == 0) return data;

            var rs = await _fileRepository.Query(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var r in rs)
            {
                data.Add(new FileModel()
                {
                    Id = r.Id,
                    FileName = r.FileName,
                    Folder = r.Folder,
                    PublicUrl = await GetPublicUrl(r.ReletivePath),
                    ReletivePath = r.ReletivePath
                });
            }

            return data;
        }
    }
}
